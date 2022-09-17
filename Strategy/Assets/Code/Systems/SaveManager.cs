using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace SaveSystem
{
    [System.Serializable]
    public struct SaveGameMetaData
    {
        public string name;
        public string date;
        public byte[] screenShot;
    }

    public interface ISaveSystem
    {
        public void SaveGame(string fileName);

        public void LoadGame(string fileName);
    }

    public class SaveManager : MonoBehaviour, ISaveSystem
    {
        public static ISaveSystem instance;

        private void Awake()
        {
            instance = this;
        }
        public void SaveGame(string fileName)
        {
            StartCoroutine(TakeScreenShot((result) => {
                ContinueSaving(fileName, result);
            }));
        }

        private void ContinueSaving(string fileName, Texture2D screenShot)
        {
            var state = new Dictionary<string, object>();
            GameManager.instance.SaveGame(ref state);
            SaveState(ref state);

            SaveGameMetaData saveGameMetaData = new SaveGameMetaData();
            saveGameMetaData.screenShot = ImageConversion.EncodeToPNG(screenShot);
            saveGameMetaData.name = fileName;

            FileHandler.SaveGame(state, fileName);
            FileHandler.SaveMetaData(saveGameMetaData);
        }

        public void LoadGame(string fileName)
        {
            var state = FileHandler.LoadGame(fileName);
            GameManager.instance.LoadGame(state, (state) => LoadState(state));
        }

        private void SaveState(ref Dictionary<string, object> state)
        {
            foreach(var saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.gameObject.GetComponent<PersistentId>().Id] = saveable.SaveState();
            }
        }

        private void LoadState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                if(state.TryGetValue(saveable.gameObject.GetComponent<PersistentId>().Id, out object savedState))
                {
                    saveable.LoadState(savedState);
                }
            }
        }

        IEnumerator TakeScreenShot(System.Action<Texture2D> callback)
        {
            yield return new WaitForEndOfFrame();
            callback(ScreenCapture.CaptureScreenshotAsTexture());
        }
    }
}

