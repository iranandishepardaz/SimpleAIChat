using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using ApcoAgentCore.Models;

namespace ApcoAgentCore.Services
{
    public static class PresetService
    {
        private static string FilePath => PathService.PresetsFilePath;

        public static List<Preset> LoadPresets()
        {
            EnsureFileExists();

            string json = File.ReadAllText(FilePath, Encoding.UTF8);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            List<Preset>? presets = JsonSerializer.Deserialize<List<Preset>>(json, options);
            return presets ?? new List<Preset>();
        }

        public static void SavePresets(List<Preset> presets)
        {
            if (presets == null)
                throw new ArgumentNullException(nameof(presets));

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(presets, options);
            File.WriteAllText(FilePath, json, Encoding.UTF8);
        }

        private static void EnsureFileExists()
        {
            if (File.Exists(FilePath))
                return;

            PathService.EnsureAppDataFolderExists();
            File.WriteAllText(FilePath, Preset.DefaultPresetsJson, Encoding.UTF8);
        }
    }
}