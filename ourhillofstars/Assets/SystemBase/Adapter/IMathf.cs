using SystemBase.Utils;
using UnityEngine;

namespace SystemBase.Adapter
{
    public interface IMathf
    {
        float PerlinNoise(float x, float y);
    }

    public class MathfAdapter : IMathf
    {
        public float PerlinNoise(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
        }
    }
}