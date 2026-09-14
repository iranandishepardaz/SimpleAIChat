using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ApcoAgentCore.Services
{
    public static class PathService
    {
        private static readonly string _companyName;
        private static readonly string _productName;

        static PathService()
        {
            Assembly asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            AssemblyCompanyAttribute? companyAttr =
                asm.GetCustomAttribute<AssemblyCompanyAttribute>();
            AssemblyProductAttribute? productAttr =
                asm.GetCustomAttribute<AssemblyProductAttribute>();

            _companyName = companyAttr?.Company ?? "Apco";
            _productName = productAttr?.Product ?? asm.GetName().Name ?? "ApcoAgentCore";
        }

        public static string CompanyName
        {
            get { return _companyName; }
        }

        public static string ProductName
        {
            get { return _productName; }
        }

        public static string AppDataFolder
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    _companyName,
                    _productName);
            }
        }

        public static string ModelsFilePath
        {
            get
            {
                return Path.Combine(AppDataFolder, "llms.json");
            }
        }

        public static string PresetsFilePath
        {
            get
            {
                return Path.Combine(AppDataFolder, "presets.json");
            }
        }

        public static string LogsFolder
        {
            get
            {
                return Path.Combine(AppDataFolder, "Logs");
            }
        }

        public static string CombineAppData(params string[] parts)
        {
            List<string> all = new List<string>();
            all.Add(AppDataFolder);
            all.AddRange(parts);
            return Path.Combine(all.ToArray());
        }

        public static void EnsureAppDataFolderExists()
        {
            Directory.CreateDirectory(AppDataFolder);
        }

        public static string GetLogFilePath()
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
            return Path.Combine(LogsFolder, fileName);
        }
    }
}