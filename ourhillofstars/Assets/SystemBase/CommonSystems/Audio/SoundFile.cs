using System;
using UnityEngine;

namespace SystemBase.CommonSystems.Audio
{
    [Serializable]
    public class SoundFile
    {
        public string Name;
        public AudioClip File;

        [Range(0f, 1f)]
        public float Volume;
    }
}