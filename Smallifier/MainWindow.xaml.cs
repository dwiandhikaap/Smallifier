using System;
using System.Diagnostics;
using System.Windows;
using Smallifier.Model;
using Smallifier.Model.VideoProcess;
using Smallifier.Model.ConsoleHelper;
using System.IO;
using System.Threading.Tasks;

namespace Smallifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isConversionRunning = false;

        FileHandler fileHandler = new FileHandler();
        FFmpegHandler ffmpegHandler = new FFmpegHandler();

        public MainWindow()
        {
            InitializeComponent();
            FFmpegDownloader ffmpegDownloader = new FFmpegDownloader();
            ffmpegDownloader.start();
        }

        private async void FileDropStackPanel_Drop(object sender, DragEventArgs e)
        {
            if (isConversionRunning) return;

            await fileHandler.handleDragEvent(e);

            if (fileHandler.isInputReady)
            {
                StartTimeTextBox.Text = "00:00:00";
                EndTimeTextBox.Text = $"{this.fileHandler.duration:hh\\:mm\\:ss}";
            }
            RefreshUI();
        }

        private async void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            await fileHandler.showOpenFileDialog();

            if (fileHandler.isInputReady)
            {
                StartTimeTextBox.Text = "00:00:00";
                EndTimeTextBox.Text = $"{this.fileHandler.duration:hh\\:mm\\:ss}";
            }
            RefreshUI();
        }

        private void UpdateFileNameLabel()
        {
            fileNameLabel.Content = fileHandler.fileName + fileHandler.fileExt;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Util.ValidateTimeRange(StartTimeTextBox.Text, EndTimeTextBox.Text);
                Util.ValidateFileSize(FileTargetSizeBox.Text);
            }
            catch(Exception err)
            {
                Alert.PopUp(err);
                return;
            }

            fileHandler.showSaveFileDialog();
            if (!fileHandler.isOutputReady)
            {
                return;
            }

            string targetFilePath = fileHandler.targetPath;
            string filePath = fileHandler.filePath;
            string targetSize = FileTargetSizeBox.Text;
            string startTime = StartTimeTextBox.Text;
            string endTime = EndTimeTextBox.Text;


            FFmpegHandlerConfig handlerConfig = new FFmpegHandlerConfig(filePath, targetFilePath, targetSize, startTime, endTime);
            ffmpegHandler.SetConfig(handlerConfig);

            ffmpegHandler.addEventListener("onStart", OnConversionStart);
            ffmpegHandler.addEventListener("onProgress", OnConversionProgress);
            ffmpegHandler.addEventListener("onFinish", OnConversionFinish);

            await ffmpegHandler.start();
        }

        private void RefreshUI()
        {
            if (fileHandler.isInputReady && !isConversionRunning)
            {
                SaveButton.IsEnabled = true;
                StartTimeTextBox.IsEnabled = true;
                EndTimeTextBox.IsEnabled = true;
            }
            else
            {
                SaveButton.IsEnabled = false;
                StartTimeTextBox.IsEnabled = false;
                EndTimeTextBox.IsEnabled = false;
            }

            if (isConversionRunning)
            {
                SelectFileButton.IsEnabled = false;
                ProgressBarWrapper.Visibility = Visibility.Visible;
            }
            else
            {
                SelectFileButton.IsEnabled = true;
                ProgressBarWrapper.Visibility = Visibility.Collapsed;
            }

            UpdateFileNameLabel();
        }

        private void OnConversionStart()
        {
            Trace.WriteLine("CONVERSION START");
            isConversionRunning = true;
            RefreshUI();
        }

        private void OnConversionFinish()
        {
            Trace.WriteLine("CONVERSION FINISH");

            isConversionRunning = false;
            RefreshUI();
        }

        private void OnConversionProgress()
        {
            this.Dispatcher.Invoke(() =>
            {
                ProgessBarStatus.Value = ffmpegHandler.handlerProgressPercentage;
            });
        }
    }

    // Console version of the app, handles file drag and drop to the icon
    // A really handy feature...
    public class MainConsole
    {
        FFmpegHandler ffmpegHandler = new FFmpegHandler();
        FFmpegDownloader ffmpegDownloader = new FFmpegDownloader();
        ConsoleProgressBar progress;
        public MainConsole(string[] Args)
        {
            ArgumentParser argParser = new ArgumentParser(Args);

            string inputPath = argParser.retrieve(ArgumentKey.InputPath);
            string targetSize = argParser.retrieve(ArgumentKey.TargetSize);
            string startTime = argParser.retrieve(ArgumentKey.StartTime);
            string endTime = argParser.retrieve(ArgumentKey.EndTime);
            string filenameFormat = argParser.retrieve(ArgumentKey.Filename);

            string outputDir = argParser.retrieve(ArgumentKey.OutputDir);

            string inputFilename = Path.GetFileNameWithoutExtension(inputPath);
            string outputFilename = Util.FormatFilename(inputFilename, filenameFormat);
            string inputExt = Path.GetExtension(inputPath);

            if (!Path.HasExtension(outputFilename))
            {
                outputFilename += inputExt;
            }

            string outputPath = Path.Combine(outputDir, outputFilename);

            ffmpegDownloader.start();

            progress = new ConsoleProgressBar();

            FFmpegHandlerConfig handlerConfig = new FFmpegHandlerConfig(inputPath, outputPath, targetSize, startTime, endTime);
            ffmpegHandler.SetConfig(handlerConfig);

            ffmpegHandler.addEventListener("onProgress", updateProgressBar);
        }

        public void start()
        {
            Console.WriteLine("Processing video... ");
            Task.Run(ffmpegHandler.start).Wait();
            Console.WriteLine("\nDone..!");
        }

        private void updateProgressBar()
        {
            double percentage = ffmpegHandler.handlerProgressPercentage;
            this.progress.Report((double)percentage / 100);
        }
    }
}
