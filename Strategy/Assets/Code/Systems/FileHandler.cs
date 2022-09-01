

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public struct SaveGameFile
{
    public string name;
    public string date;
}

public static class FileHandler
{
    private readonly static string saveDirectory = Path.Combine(Application.persistentDataPath, "save");
    private readonly static string extension = ".sav";

    public static List<SaveGameFile> GetSaveGameFileList()
    {
        var fileList = new List<SaveGameFile>();
        foreach (var filePath in Directory.GetFiles(saveDirectory).Where(x => x.Contains(extension)))
        {
            Debug.Log(filePath);
            fileList.Add(new SaveGameFile { name = filePath.Substring(filePath.LastIndexOf("\\")).Trim('\\'), date = File.GetLastWriteTime(filePath).ToString() });
        }
        return fileList;
    }

    public static void SaveGame(Dictionary<string, object> gameState, string fileName)
    {
        if(!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        using (FileStream file = File.Create(Path.Combine(saveDirectory, fileName)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, gameState);
            Debug.Log("Save to file " + Path.Combine(saveDirectory, fileName));
        }
    }

    public static Dictionary<string, object> LoadGame(string fileName)
    {
        var path = Path.Combine(saveDirectory, fileName);
        if (!File.Exists(path))
        {
            Debug.Log("File " + path + " does not exist");
            return null;
        }

        using (FileStream file = File.OpenRead(path))
        {
            Debug.Log("Load from file " + path);
            BinaryFormatter bf = new BinaryFormatter();
            return (Dictionary<string, object>)bf.Deserialize(file);
        }
    }
}
