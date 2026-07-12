using System;
using System.IO;
using UnityEngine;

namespace CleanRoomArcade.Data
{
    public static class SettingsStore
    {
        public const string DirectoryName = "CleanRoomArcadeScreensaver";
        public const string FileName = "settings.json";

        public static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            DirectoryName,
            FileName);

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return AppSettings.Defaults();
                var settings = JsonUtility.FromJson<AppSettings>(File.ReadAllText(SettingsPath));
                if (settings == null) return AppSettings.Defaults();
                settings.Sanitize();
                return settings;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Settings were unreadable; safe defaults will be used. {exception.Message}");
                return AppSettings.Defaults();
            }
        }

        public static bool TrySave(AppSettings settings, out string error)
        {
            try
            {
                settings ??= AppSettings.Defaults();
                settings.Sanitize();
                var directory = Path.GetDirectoryName(SettingsPath);
                if (string.IsNullOrEmpty(directory)) throw new IOException("Settings directory could not be resolved.");
                Directory.CreateDirectory(directory);
                var temporary = SettingsPath + ".tmp";
                File.WriteAllText(temporary, JsonUtility.ToJson(settings, true));
                if (File.Exists(SettingsPath)) File.Replace(temporary, SettingsPath, null);
                else File.Move(temporary, SettingsPath);
                error = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }
        }
    }
}
