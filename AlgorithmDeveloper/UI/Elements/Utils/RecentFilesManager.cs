using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AlgorithmDeveloper.UI.Elements.Utils
{
    public static class RecentFilesManager
    {
        private const string FileName = "recent_files.json";
        private const int MaxRecentFiles = 10;
        private static List<string> _recentFiles = new();

        public static IReadOnlyList<string> RecentFiles => _recentFiles;

        static RecentFilesManager()
        {
            Load();
        }

        public static void AddFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return;

            // Удаляем, если уже есть (чтобы переместить в начало)
            _recentFiles.RemoveAll(f => string.Equals(f, filePath, StringComparison.OrdinalIgnoreCase));
            
            // Добавляем в начало
            _recentFiles.Insert(0, filePath);

            // Обрезаем до максимума
            if (_recentFiles.Count > MaxRecentFiles)
            {
                _recentFiles = _recentFiles.Take(MaxRecentFiles).ToList();
            }

            Save();
        }

        private static void Load()
        {
            try
            {
                string path = AppDataManager.GetPath(FileName);
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<List<string>>(json);
                    if (list != null)
                    {
                        _recentFiles = list.Where(File.Exists).ToList(); // Фильтруем несуществующие
                    }
                }
            }
            catch { }
        }

        private static void Save()
        {
            try
            {
                string path = AppDataManager.GetPath(FileName);
                string json = JsonSerializer.Serialize(_recentFiles, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch { }
        }
    }
}
