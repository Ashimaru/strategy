using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem 
{
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField]
        private string id;

        public string Id => id;

        [ContextMenu("Generate Id")]
        private void GenerateId()
        {
            id = Guid.NewGuid().ToString();
        }

        public object SaveState()
        {
            var state = new Dictionary<string, object>();
            foreach (var saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.SaveState();
            }
            return state;
        }

        public void LoadState(object state)
        {
            var stateDictionary = (Dictionary<string, object>)state;
            foreach (var saveable in GetComponents<ISaveable>())
            {
                string typename = saveable.GetType().ToString();
                if(stateDictionary.TryGetValue(typename, out object savedState))
                {
                    saveable.LoadState(savedState);
                }
            }
        }
    }
}