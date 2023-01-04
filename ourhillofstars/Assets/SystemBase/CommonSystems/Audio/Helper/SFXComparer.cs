using SystemBase.CommonSystems.Audio.Actions;

namespace SystemBase.CommonSystems.Audio.Helper
{
    public class SFXComparer : ISFXComparer
    {
        public bool Equals(AudioActSFXPlay x, AudioActSFXPlay y)
        {
            return x != null &&
                   y != null &&
                   !string.IsNullOrEmpty(x.Tag) &&
                   !string.IsNullOrEmpty(y.Tag) &&
                   x.Tag.Equals(y.Tag);
        }

        public int GetHashCode(AudioActSFXPlay obj)
        {
            return obj.Tag.GetHashCode();
        }
    }
}