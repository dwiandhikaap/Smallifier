using System;
using System.Collections.Generic;
using Xabe.FFmpeg;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Smallifier.Model.VideoProcess
{
    class FFmpegHandler
    {
        private FFmpegHandlerConfig handlerConfig;

        private List<Action> listenerOnStart = new List<Action>();
        private List<Action> listenerOnProgress = new List<Action>();
        private List<Action> listenerOnFinish = new List<Action>();

        public int handlerProgressPercentage = 0;

        public FFmpegHandler()
        {
            string currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string ffmpegPath = Path.Combine(currentDir, "ffmpeg");
            FFmpeg.SetExecutablesPath(ffmpegPath);
        }

        public void SetConfig(FFmpegHandlerConfig handlerConfig)
        {
            this.handlerConfig = handlerConfig;
        }

        public async Task start()
        {
            ExecuteOnStartListener();

            string inputFileDir = this.handlerConfig.filePath;
            string outputFileDir = this.handlerConfig.destFilePath;

            if (File.Exists(outputFileDir)) File.Delete(outputFileDir);

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFileDir);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            
            UInt64 duration = (ulong)mediaInfo.Duration.TotalSeconds;
            UInt64 audioBitrate = (ulong)audioStream.Bitrate;
            UInt64 targetBitrate = Util.ParseFileSize(this.handlerConfig.targetSize) / (duration + 1) - audioBitrate;

            Console.WriteLine(duration);
            Console.WriteLine(audioBitrate);
            Console.WriteLine(targetBitrate);
            //TODO: Add maxrate ffmpeg

            double startTime = Util.ParseTime(this.handlerConfig.targetStartTime);
            double endTime = Util.ParseTime(this.handlerConfig.targetEndTime);

            string timeRangeParam = Util.CreateTimeSpanRange(startTime, endTime);

            IVideoStream outStream = videoStream.SetBitrate((long)targetBitrate);


            IConversion conversion = FFmpeg.Conversions.New()
                .AddStream(outStream)
                .AddStream(audioStream)
                .SetOutput(outputFileDir);

            if (timeRangeParam != "") conversion.AddParameter(timeRangeParam);

            conversion.OnProgress += (sender, args) =>
            {
                double totalLength = 0;
                if(endTime == -1)
                {
                    totalLength = args.TotalLength.TotalSeconds;
                }
                else
                {
                    totalLength = endTime;
                }

                if(startTime != -1)
                {
                    totalLength -= startTime;
                }


                this.handlerProgressPercentage = (int)(Math.Round(args.Duration.TotalSeconds / totalLength, 2) * 100);
                ExecuteOnProgressListener();
            };

            await conversion.Start();
            ExecuteOnFinishListener();
        }

        public static async Task<TimeSpan> getVideoDuration(string filePath)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filePath);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            return videoStream.Duration;
        }

        public delegate void Action();
        public void addEventListener(string Event, Action func)
        {
            switch (Event)
            {
                case "onStart":
                    {
                        listenerOnStart.Add(func);
                        break;
                    }
                case "onProgress":
                    {
                        listenerOnProgress.Add(func);
                        break;
                    }
                case "onFinish":
                    {
                        listenerOnFinish.Add(func);
                        break;
                    }
            }
        }

        private void ExecuteOnStartListener()
        {
            foreach(Action func in listenerOnStart)
            {
                func();
            }
        }

        private void ExecuteOnProgressListener()
        {
            foreach (Action func in listenerOnProgress)
            {
                func();
            }
        }

        private void ExecuteOnFinishListener()
        {
            foreach (Action func in listenerOnFinish)
            {
                func();
            }
        }
    }
}
