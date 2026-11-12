using System.IO;
using UnityEditor;
using UnityEngine;

namespace PiperiteGames.EditorTools
{
    public static class ProjectFolderCreator
    {
        private static readonly string[] Folders =
        {
            "Assets/_Project/Scripts",
            "Assets/_Project/Scenes",
            "Assets/_Project/Editor",
            "Assets/_Project/Prefabs",
            "Assets/_Project/Art",
            "Assets/_Project/Animations",
            "Assets/_Project/Audio",
            "Assets/_Project/Materials",
            "Assets/_Project/UI",
            "Assets/_ThirdParty",
        };

        [MenuItem("Tools/Create Project Folders")]
        public static void CreateFolders()
        {
            int created = 0;

            foreach (string folder in Folders)
            {
                if (Directory.Exists(folder))
                    continue;

                Directory.CreateDirectory(folder);

                // Git не хранит пустые папки, поэтому кладём .gitkeep.
                // Unity игнорирует файлы, начинающиеся с точки.
                File.WriteAllText(Path.Combine(folder, ".gitkeep"), string.Empty);
                created++;
            }

            AssetDatabase.Refresh();

            Debug.Log(created > 0
                ? $"Создано папок: {created}"
                : "Все папки уже существуют.");
        }
    }
}