using System;
using System.Collections.Generic;
using System.Text;

namespace Smallifier.Model.VideoProcess
{
    class FFmpegHandlerConfig
    {
        public string filePath;
        public string destFilePath;
        public string targetSize;
        public string targetStartTime;
        public string targetEndTime;

        public FFmpegHandlerConfig(string filePath, string destFilePath, string targetSize, string targetStartTime, string targetEndTime)
        {
            this.filePath = filePath;
            this.destFilePath = destFilePath;
            this.targetSize = targetSize;
            this.targetStartTime = targetStartTime;
            this.targetEndTime = targetEndTime;
        }

        public string toString()
        {
            return $"{filePath}, {destFilePath}, {targetSize}, {targetStartTime}, {targetEndTime}";
        }
    }
}

