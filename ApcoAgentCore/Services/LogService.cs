using System;
using System.IO;
using System.Text;

namespace ApcoAgentCore.Services
{

    public enum DebugLevel { Off = 0, Normal = 1, Verbose = 2 }

    public static class LogService
    {
        public static string SaveText(string content)
        {
            return SaveText(content, "");
        }

        public static string SaveText(string content, string prefix)
        {
            string savedPath = "";
            try
            {

                Directory.CreateDirectory(PathService.LogsFolder);

                string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                if (prefix.Length > 0)
                    fileName += prefix;

                savedPath = Path.Combine(PathService.LogsFolder, fileName);
                File.WriteAllText(savedPath, content, Encoding.UTF8);

            }
            catch
            {

            }
            return savedPath;
        }
    }

}