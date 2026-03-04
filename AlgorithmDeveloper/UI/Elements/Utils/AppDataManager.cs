using System;
using System.IO;

namespace AlgorithmDeveloper.UI.Elements.Utils
{
    public static class AppDataManager
    {
        private static string _appDataFolder = string.Empty;

        public static string AppDataFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_appDataFolder))
                {
                    string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    _appDataFolder = Path.Combine(localAppData, "AlgorithmDeveloper");
                    if (!Directory.Exists(_appDataFolder))
                    {
                        Directory.CreateDirectory(_appDataFolder);
                    }
                }
                return _appDataFolder;
            }
        }

        public static string GetPath(string fileName)
        {
            return Path.Combine(AppDataFolder, fileName);
        }

        public static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }
        }
    }
}
