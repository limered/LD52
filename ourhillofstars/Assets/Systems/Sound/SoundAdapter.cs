using System;
using SystemBase.CommonSystems.Audio.Actions;
using SystemBase.CommonSystems.Audio.Helper;
using SystemBase.Utils;

namespace Systems.Sound
{
    public class SFXComparer : ISFXComparer
    {
        public bool Equals(AudioActSFXPlay x, AudioActSFXPlay y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name && x.Tag == y.Tag;
        }

        public int GetHashCode(AudioActSFXPlay obj)
        {
            return HashCode.Combine(obj.Name, obj.Tag);
        }
    }
    
    public class SoundAdapterRegistrations : IIocRegistration
    {
        public void Register()
        {
            IoC.RegisterType<ISFXComparer, SFXComparer>();
        }
    }
}