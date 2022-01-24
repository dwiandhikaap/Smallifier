using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Smallifier.Model
{
    class Util
    {
        public static string FormatFilename(string inputName, string outputName)
        {
            return outputName
                .Replace("%name%", inputName)
                .Replace("%date%", DateTime.Now.ToShortDateString())
                .Replace("%hour%", DateTime.Now.Hour.ToString())
                .Replace("%minute%", DateTime.Now.Minute.ToString())
                .Replace("%second%", DateTime.Now.Second.ToString());
        }
        public static ulong ParseFileSize(string fileSize)
        {
            ValidateFileSize(fileSize);

            string splitter = @"\d+|KB|MB|GB|B|kb|mb|gb|b";
            Regex regex = new Regex(splitter);

            MatchCollection groups = regex.Matches(fileSize);

            string sizeNumberStr = groups[0].Value;
            string unitStr = groups[1].Value;

            ulong sizeNumber = ulong.Parse(sizeNumberStr);
            uint multiplier;

            switch (unitStr.ToLower())
            {
                case "k":
                case "kb":
                    {
                        multiplier = 1000;
                        break;
                    }
                case "m":
                case "mb":
                    {
                        multiplier = 1000000;
                        break;
                    }
                case "g":
                case "gb":
                    {
                        multiplier = 1000000000;
                        break;
                    }
                default:
                case "b":
                    {
                        multiplier = 1;
                        break;
                    }
            }

            int bitMultiplier = unitStr[unitStr.Length - 1] == 'B' ? 8 : 1;

            return sizeNumber * multiplier * (uint)bitMultiplier;
        }

        public static double ParseTime(string timeString)
        {
            if(timeString == "")
            {
                return -1;
            }

            return TimeSpan.Parse(timeString).TotalSeconds;
        }

        public static string CreateTimeSpanRange(double startTime, double endTime)
        {
            string start = startTime == -1 ? "" : $"-ss {TimeSpan.FromSeconds(startTime)}";
            string end = endTime == -1 ? "" : $" -t {TimeSpan.FromSeconds(endTime)}";

            return start + end;
        }

        public static bool IsFileSizeValid(string fileSize)
        {
            string validator = @"\d+ *?(KB|MB|GB|B|kb|mb|gb|b)";
            Regex regex = new Regex(validator);
            return regex.IsMatch(fileSize);
        }

        public static bool IsTimeValid(string time)
        {
            TimeSpan outTime;
            return TimeSpan.TryParse(time, out outTime);
        }
        public static void ValidateFileSize(string fileSize)
        {
            if (!IsFileSizeValid(fileSize))
            {
                throw new Exception("Invalid File Size Format! Example : \"900KB\", \"50 MB\"");
            }
        }

        public static void ValidateTime(string time)
        {
            if (!IsTimeValid(time)) throw new FormatException("Invalid time format!");
        }
        public static void ValidateTimeRange(string startTime, string endTime)
        {
            
            if (!IsTimeValid(startTime)) throw new FormatException("Invalid start time format!");
            if (!IsTimeValid(endTime)) throw new FormatException("Invalid end time format!");
            if (TimeSpan.Parse(startTime).TotalSeconds >= TimeSpan.Parse(endTime).TotalSeconds) throw new Exception("Start Time should be less than End Time!");
        }
    }
}
