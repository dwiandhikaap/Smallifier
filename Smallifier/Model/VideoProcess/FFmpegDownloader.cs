using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Net;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Linq;

namespace Smallifier.Model.VideoProcess
{
    class FFmpegDownloader
    {
        public void start()
        {
            if (doesFFmpegExists())
            {
                return;
            }

            string currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string ffmpegDir = Path.Combine(currentDir, "ffmpeg");
            Directory.CreateDirectory(ffmpegDir);
            Task.Run(runDownloader).Wait();
            runExtractor();
            deleteExtracted();
        }
        public bool doesFFmpegExists()
        {
            string currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string ffmpegPath = Path.Combine(currentDir, "ffmpeg\\ffmpeg.exe");
            string ffprobePath = Path.Combine(currentDir, "ffmpeg\\ffprobe.exe");

            return File.Exists(ffmpegPath) && File.Exists(ffprobePath);
        }

        private async Task runDownloader()
        {

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

            if (Environment.Is64BitOperatingSystem)
            {
                await webClient.DownloadFileTaskAsync(new Uri("https://github.com/tomaszzmuda/Xabe.FFmpeg/releases/download/executables/ffmpeg-latest-win64-shared.zip"), "ffmpeg.zip");
            }
            else
            {
                await webClient.DownloadFileTaskAsync(new Uri("https://github.com/tomaszzmuda/Xabe.FFmpeg/releases/download/executables/ffmpeg-latest-win32-shared.zip"), "ffmpeg.zip");
            }

            return;
        }

        private void runExtractor()
        {
            string currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string ffmpegZipPath = Path.Combine(currentDir, "ffmpeg.zip");
            string ffmpegDestPath = Path.Combine(currentDir, "ffmpeg");
            string ffmpegExtractedPath = "";

            Console.WriteLine(ffmpegZipPath);
            Console.WriteLine(currentDir);

            ZipFile.ExtractToDirectory(ffmpegZipPath, currentDir);

            if (Environment.Is64BitOperatingSystem)
            {
                ffmpegExtractedPath = Path.Combine(currentDir, "ffmpeg-latest-win64-shared/bin");
            }

            else
            {
                ffmpegExtractedPath = Path.Combine(currentDir, "ffmpeg-latest-win32-shared/bin");
            }

            //Directory.Move(ffmpegExtractedPath, ffmpegDestPath);
            List<String> ffmpegFiles = Directory
                   .GetFiles(ffmpegExtractedPath, "*.*", SearchOption.AllDirectories).ToList();

            foreach (string file in ffmpegFiles)
            {
                FileInfo mFile = new FileInfo(file);
                // to remove name collisions
                string newFilePath = Path.Combine(ffmpegDestPath, mFile.Name);
                if (new FileInfo(newFilePath).Exists == false)
                {
                    mFile.MoveTo(newFilePath);
                }
            }
        }

        private void deleteExtracted()
        {
            string currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string ffmpegExtractedPath = Path.Combine(currentDir, "ffmpeg-latest-win64-shared");
            string ffmpegZipPath = Path.Combine(currentDir, "ffmpeg.zip");
            //Console.WriteLine(ffmpegExtractedPath);
            Directory.Delete(ffmpegExtractedPath, true);
            File.Delete(ffmpegZipPath);
        }
    }
}
