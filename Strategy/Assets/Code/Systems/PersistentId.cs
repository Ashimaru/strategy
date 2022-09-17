using UnityEngine;
using System;

namespace SaveSystem
{
    public class PersistentId : MonoBehaviour
    {
        [SerializeField]
        private string id;

        public string Id => id;

        [ContextMenu("Generate Id")]
        private void GenerateId()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
