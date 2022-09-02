

using SaveSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileHandler
{
    private readonly static string saveDirectory = Path.Combine(Application.persistentDataPath, "saves");
    private readonly static string save_extension = ".sav";
    private readonly static string meta_extension = ".meta";

    public static List<SaveGameMetaData> GetSaveGameFileList()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
            return new List<SaveGameMetaData>();
        }

        var fileList = new List<SaveGameMetaData>();
        foreach (var filePath in Directory.GetFiles(saveDirectory).Where(x => x.Contains(meta_extension)))
        {
            using (FileStream file = File.OpenRead(filePath))
            {
                var bf = GetBinaryFormatter();
                var saveGameMetaData = (SaveGameMetaData)bf.Deserialize(file);
                saveGameMetaData.date = File.GetLastWriteTime(filePath).ToString();
                fileList.Add(saveGameMetaData);
            }
        }
        return fileList;
    }

    public static void SaveGame(Dictionary<string, object> gameState, string fileName)
    {
        fileName += save_extension;

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        using (FileStream file = File.Create(Path.Combine(saveDirectory, fileName)))
        {
            var bf = GetBinaryFormatter();
            bf.Serialize(file, gameState);
            Debug.Log("Save to file " + Path.Combine(saveDirectory, fileName));
        }
    }

    public static void SaveMetaData(SaveGameMetaData saveGameMetaData)
    {
        var fileName = saveGameMetaData.name + meta_extension;

        using (FileStream file = File.Create(Path.Combine(saveDirectory, fileName)))
        {
            var bf = GetBinaryFormatter();
            bf.Serialize(file, saveGameMetaData);
            Debug.Log("Save to file " + Path.Combine(saveDirectory, fileName));
        }
    }

    public static Dictionary<string, object> LoadGame(string fileName)
    {
        fileName += save_extension;

        var path = Path.Combine(saveDirectory, fileName);
        if (!File.Exists(path))
        {
            Debug.Log("File " + path + " does not exist");
            return null;
        }

        using (FileStream file = File.OpenRead(path))
        {
            Debug.Log("Load from file " + path);
            var bf = GetBinaryFormatter();
            return (Dictionary<string, object>)bf.Deserialize(file);
        }
    }

    private static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());

        binaryFormatter.SurrogateSelector = selector;

        return binaryFormatter;
    }
}
