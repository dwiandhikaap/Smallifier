using System;
using System.Collections.Generic;
using System.IO;

namespace Smallifier.Model.ConsoleHelper
{
    enum ArgumentKey
    {
        TargetSize,
        StartTime,
        EndTime,
        Filename,
        OutputDir,
        InputPath
    }
    class ArgumentParser
    {
        private Dictionary<ArgumentKey, string> parsedArgument = new Dictionary<ArgumentKey, string>();
        public ArgumentParser(string[] Args)
        {
            // worst args parser of 2022™
            int sizeArgIndex = Array.FindIndex(Args, str => str == "--size" || str == "-s");
            int startTimeArgIndex = Array.FindIndex(Args, str => str == "--start");
            int endTimeArgIndex = Array.FindIndex(Args, str => str == "--end");
            int filenameArgIndex = Array.FindIndex(Args, str => str == "--name" || str == "-n");
            int outputDirArgIndex = Array.FindIndex(Args, str => str == "--output" || str == "-o");

            string inputPath = Args[Args.Length - 1];
            parsedArgument.Add(ArgumentKey.InputPath, inputPath);

            if (sizeArgIndex != -1)
            {
                string targetSize = Args[sizeArgIndex + 1];
                Util.ValidateFileSize(targetSize);

                parsedArgument.Add(ArgumentKey.TargetSize, targetSize);
            }
            else
            {
                parsedArgument.Add(ArgumentKey.TargetSize, "8 MB");
            }

            if (startTimeArgIndex != -1)
            {
                string startTime = Args[startTimeArgIndex + 1];
                Util.ValidateTime(startTime);

                parsedArgument.Add(ArgumentKey.StartTime, startTime);
            }
            else
            {
                parsedArgument.Add(ArgumentKey.StartTime, "");
            }

            if (endTimeArgIndex != -1)
            {
                string endTime = Args[endTimeArgIndex + 1];
                Util.ValidateTime(endTime);

                parsedArgument.Add(ArgumentKey.EndTime, endTime);
            }
            else
            {
                parsedArgument.Add(ArgumentKey.EndTime, "");
            }

            if (filenameArgIndex != -1)
            {
                string filename = Args[filenameArgIndex + 1];

                parsedArgument.Add(ArgumentKey.Filename, filename);
            }
            else
            {
                parsedArgument.Add(ArgumentKey.Filename, "%name%_%hour%%minute%%second%");
            }

            if (outputDirArgIndex != -1)
            {
                string outputDir = Args[outputDirArgIndex + 1];

                parsedArgument.Add(ArgumentKey.OutputDir, outputDir);
            }
            else
            {
                string outputDir = Path.GetDirectoryName(inputPath);
                parsedArgument.Add(ArgumentKey.OutputDir, outputDir);
            }

            
        }

        public string retrieve(ArgumentKey argumentKey)
        {
            return parsedArgument[argumentKey];
        }
    }
}
