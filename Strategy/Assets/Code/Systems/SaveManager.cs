using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SaveSystem
{
    public interface ISaveSystem
    {
        public void SaveGame(string fileName);

        public void LoadGame(string fileName);
    }

    public class SaveManager : MonoBehaviour, ISaveSystem
    {
        private Dictionary<string, object> savedGame;

        private void Awake()
        {
            Systems.RegisterSystem<ISaveSystem>(this);
        }

        public void SaveGame(string fileName)
        {
            var state = new Dictionary<string, object>();
            SaveState(ref state);
            FileHandler.SaveGame(state, fileName);
        }

        public void LoadGame(string fileName)
        {
            var state = FileHandler.LoadGame(fileName);
            LoadState(state);
        }

        private void SaveState(ref Dictionary<string, object> state)
        {
            foreach(var saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.Id] = saveable.SaveState();
            }
        }

        private void LoadState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                if(state.TryGetValue(saveable.Id, out object savedState))
                {
                    saveable.LoadState(savedState);
                }
            }
        }
    }
}

