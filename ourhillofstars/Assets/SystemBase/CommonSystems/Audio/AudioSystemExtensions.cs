using SystemBase.CommonSystems.Audio.Actions;
using UniRx;
using UnityEngine;

namespace SystemBase.CommonSystems.Audio
{
    public static class AudioSystemExtensions
    {
        public static void Play(this string soundName, PlaySFXParameters parameters = null, string tag = null)
        {
            MessageBroker.Default.Publish(new AudioActSFXPlay { Name = soundName, Tag = tag, Parameters = parameters});
        }
        
        public static void PlayRandom(this string[] soundArray, PlaySFXParameters parameters = null, string tag = null)
        {
            MessageBroker.Default.Publish(new AudioActSFXPlay { Name = soundArray[Random.Range(0, soundArray.Length)], Tag = tag, Parameters = parameters });
        }
    }
}