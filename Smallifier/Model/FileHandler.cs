using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using Smallifier.Model.VideoProcess;
using System.Threading.Tasks;

namespace Smallifier.Model
{
    class FileHandler
    {
        private String _filePath = "";
        public String fileName = "";
        public String fileExt = "";
        public String targetPath = "";

        public bool isInputReady = false;
        public bool isOutputReady = false;

        public TimeSpan duration;

        public String filePath
        {
            get {
                return _filePath;
            }
            set{
                if (!this.validateFile(value))
                {
                    Alert.PopUp("Selected file is invalid or not supported!");
                    return;
                }

                this._filePath = value;
                this.fileName = Path.GetFileNameWithoutExtension(value);
                this.fileExt = Path.GetExtension(value);
            }
        }

        public bool validateFile(string path)
        {
            string extension = Path.GetExtension(path);
            string[] allowedExtensions = {  ".mp4",
                                            ".mkv",
                                            ".avi",
                                            ".mpeg",
                                            ".flv",
                                            ".webm",
                                            ".mpg",
                                            ".mp2",
                                            ".mpe",
                                            ".mpv",
                                            ".m4p",
                                            ".m4v",
                                            ".wmv",
                                            ".mov"  };

            if (!Array.Exists(allowedExtensions, str => str == extension)){
                return false;
            };

            return File.Exists(path);
        }

        public async Task showOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Media Files|*.mp4; *.mkv; *.avi; *.mpeg; *.flv; *.webm; *.mpg; *.mp2; *.mpe; *.mpv; *.m4p; *.m4v; *.wmv; *.mov; *.MP4; *.MKV; *.AVI; *.MPEG; *.FLV; *.WEBM; *.MPG; *.MP2; *.MPE; *.MPV; *.M4P; *.M4V; *.WMV; *.MOV";
            if (openFileDialog.ShowDialog() == true)
            {
                this.filePath = openFileDialog.FileName;
                this.duration = await FFmpegHandler.getVideoDuration(filePath);
                isInputReady = true;
            }
        }

        public void showSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MP4 (MPEG-4 Part 14)|*.mp4 |Matroska|*.mkv |Audio Video Interleave|*.avi |Other Media Files|*.mp4; *.mkv; *.avi; *.mpeg; *.flv; *.webm; *.mpg; *.mp2; *.mpe; *.mpv; *.m4p; *.m4v; *.wmv; *.mov; *.MP4; *.MKV; *.AVI; *.MPEG; *.FLV; *.WEBM; *.MPG; *.MP2; *.MPE; *.MPV; *.M4P; *.M4V; *.WMV; *.MOV";
            saveFileDialog.Title = "Save a Video File";
            saveFileDialog.ShowDialog();

            string targetPath = saveFileDialog.FileName;

            if (saveFileDialog.FileName != "")
            {
                isOutputReady = true;

                if (Path.GetExtension(targetPath) == null)
                {
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            {
                                this.targetPath = Path.GetFileName(targetPath) + ".mkv";
                                break;
                            }
                        case 2:
                            {
                                this.targetPath = Path.GetFileName(targetPath) + ".avi";
                                break;
                            }

                        case 0:
                        default:
                            {
                                this.targetPath = Path.GetFileName(targetPath) + ".mp4";
                                break;
                            }
                    }
                    return;
                }

                this.targetPath = targetPath.Trim();
                return;
            }
            isOutputReady = false;
        }

        public async Task handleDragEvent(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                filePath = Path.GetFullPath(droppedFilePaths[0]);
                duration = await FFmpegHandler.getVideoDuration(filePath);
                isInputReady = true;
            }

        }
    }
}
