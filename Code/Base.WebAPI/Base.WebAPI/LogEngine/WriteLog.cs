using System;
using System.IO;

namespace Base.WebAPI.LogEngine
{
    public class WriteLog
    {
        public static void Write(string logFolder, string msg) => File.WriteAllText(validatePath(logFolder), msg);

        public static void Write(string msg) => File.WriteAllText(validatePath(""), msg);

        public static void Write(string fileAppendName, string logFolder, string msg) => File.WriteAllText(validatePath(logFolder, fileAppendName), msg);

        private static string validatePath(string logFolder, string fileAppendName ="")
        {
            if (!Path.IsPathRooted(logFolder))
                logFolder = AppDomain.CurrentDomain.BaseDirectory + @"log";

            var date = DateTime.Now;
            var day = date.Day;
            var month = date.Month;
            var year = date.Year;

            var path = $@"{logFolder}\{year}\{month}\{day}\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var hour = date.Hour;
            var min = date.Minute;
            var seg = date.Second;

            var file = $@"{path}{fileAppendName}{year}{month}{day}{hour}{min}{seg}-{Guid.NewGuid()}.json";
            return file;
        }
    }
}
