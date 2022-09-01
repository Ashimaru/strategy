

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileHandler
{
    public static void SaveGame(Dictionary<string, object> gameState, string fileName)
    {
        using (FileStream file = File.Create(Path.Combine(Application.persistentDataPath, fileName)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, gameState);
            Debug.Log("Save to file " + Path.Combine(Application.persistentDataPath, fileName));
        }
    }

    public static Dictionary<string, object> LoadGame(string fileName)
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);
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
