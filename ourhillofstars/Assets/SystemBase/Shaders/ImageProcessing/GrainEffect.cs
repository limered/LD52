using UnityEngine;

namespace SystemBase.Shaders.ImageProcessing
{
    public class GrainEffect : MonoBehaviour
    {
        private Material _material;

        private void Awake()
        {
            _material = new Material(Shader.Find("Hidden/Grain"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _material);
        }
    }
}
