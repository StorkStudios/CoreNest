using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public abstract class ScriptGeneratorBase
    {
        protected readonly string name;
        private readonly string extension;
        private readonly string assetPath;

        private string currentFileContent = "";
        private string pathToTargetFolder = "";

        private string FilePath => Path.Combine(pathToTargetFolder, name + extension);

        public ScriptGeneratorBase(string name, string assetPath, string extension = ".cs")
        {
            this.name = name;
            this.extension = extension;
            this.assetPath = assetPath;
        }

        public void CreateAndUpdateFile()
        {
            if (TryGetPathToTargetFolder() && !string.IsNullOrEmpty(pathToTargetFolder) && File.Exists(FilePath))
            {
                Debug.LogWarning($"{name + extension} file already exists");
                SaveContentToFile(GetNewFileContent(), null);
                return;
            }

            Directory.CreateDirectory(Path.Combine(Application.dataPath, assetPath));
            FileStream file = File.Create(Path.Combine(Application.dataPath, assetPath, name + extension));
            SaveContentToFile(GetNewFileContent(), file);
        }

        private void SaveContentToFile(string content, FileStream file)
        {
            currentFileContent = content;

            file ??= File.OpenWrite(FilePath);

            file.SetLength(0);
            file.Write(System.Text.Encoding.UTF8.GetBytes(content));

            file.Close();

            //force unity to reimport this script so it recompiles it and doesn't throw warnings that "the file has been modified and unity doesn't know when"
            AssetDatabase.ImportAsset(Path.Combine("Assets", assetPath, name + extension));
        }

        public void UpdateFile()
        {
            if (!TryGetPathToTargetFolder() || string.IsNullOrEmpty(pathToTargetFolder) || !File.Exists(FilePath))
            {
                return;
            }

            FileStream file = null;
            if (string.IsNullOrEmpty(currentFileContent))
            {
                file = File.Open(FilePath, FileMode.Open, FileAccess.ReadWrite);
                using (StreamReader reader = new StreamReader(file))
                {
                    currentFileContent = reader.ReadToEnd();
                }
            }

            string newFileContent = GetNewFileContent();
            if (newFileContent != currentFileContent)
            {
                SaveContentToFile(newFileContent, file);
            }
            else
            {
                file?.Close();
            }
        }

        protected abstract string GetNewFileContent();

        private bool TryGetPathToTargetFolder()
        {
            try
            {
                pathToTargetFolder = Directory.GetDirectories(Application.dataPath, assetPath).FirstOrDefault();
                return !string.IsNullOrEmpty(pathToTargetFolder);
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
        }

        public static string ReplaceSpecialCharacters(string str, string specialCharacterRegex = "[^a-zA-Z0-9]", string replacement = "_")
        {
            return Regex.Replace(str, specialCharacterRegex, replacement);
        }
    }
}