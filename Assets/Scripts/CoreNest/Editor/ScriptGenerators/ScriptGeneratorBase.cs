using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

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
            UpdateFile();
            return;
        }

        Directory.CreateDirectory(Path.Combine(Application.dataPath, assetPath));
        File.Create(Path.Combine(Application.dataPath, assetPath, name + extension));
        UpdateFile();
    }

    public void UpdateFile()
    {
        if (!TryGetPathToTargetFolder() || string.IsNullOrEmpty(pathToTargetFolder))
        {
            return;
        }

        if (string.IsNullOrEmpty(currentFileContent))
        {
            TryGetCurrentFileContent();
        }

        string newFileContent = GetNewFileContent();
        if (newFileContent != currentFileContent)
        {
            currentFileContent = newFileContent;
            File.WriteAllText(FilePath, currentFileContent);

            //force unity to reimport this script so it recompiles it and doesn't throw warnings that "the file has been modified and unity doesn't know when"
            AssetDatabase.ImportAsset(Path.Combine("Assets", assetPath, name + extension));
        }
    }

    protected abstract string GetNewFileContent();

    private bool TryGetPathToTargetFolder()
    {
        pathToTargetFolder = Directory.GetDirectories(Application.dataPath, assetPath).FirstOrDefault();
        return !string.IsNullOrEmpty(pathToTargetFolder);
    }

    private bool TryGetCurrentFileContent()
    {
        string filePath = FilePath;
        if (File.Exists(filePath))
        {
            currentFileContent = File.ReadAllText(filePath);
            return true;
        }
        return false;
    }

    public static string ReplaceSpecialCharacters(string str, string specialCharacterRegex = "[^a-zA-Z0-9]", string replacement = "_")
    {
        return Regex.Replace(str, specialCharacterRegex, replacement);
    }
}
