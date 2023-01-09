using Unity.Mathematics;
using UnityEngine;

namespace Systems.Drescher
{
    public class Wobbler : MonoBehaviour
    {
        private void Update()
        {
            var scaleX = math.sin(Time.realtimeSinceStartup * 25) * 0.002f - 0.001f;
            var scaleY = math.cos(Time.realtimeSinceStartup * 20) * 0.002f - 0.001f;
            transform.localScale = new Vector3(scaleX + 0.1f, 1f, scaleY + 0.1f);
        }
    }
}