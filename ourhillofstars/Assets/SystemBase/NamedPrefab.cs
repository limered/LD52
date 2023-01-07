using System;
using UnityEngine;

namespace SystemBase
{
    [Serializable]
    public struct NamedPrefab
    {
        public string name;
        public GameObject prefab;
    }
}