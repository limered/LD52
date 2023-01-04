using UnityEngine;

namespace SystemBase.CommonSystems.Audio.Actions
{
    public class AudioActMusicSetVolume
    {
        public AudioActMusicSetVolume(float volume)
        {
            Volume = Mathf.Clamp(volume, 0f, 1f);
        }

        public float Volume { get; private set; }
    }
}
