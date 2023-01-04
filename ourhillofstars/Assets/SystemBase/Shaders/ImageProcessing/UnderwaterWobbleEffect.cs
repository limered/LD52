using UnityEngine;

namespace SystemBase.Shaders.ImageProcessing
{
    public class UnderwaterWobbleEffect : MonoBehaviour
    {
        private Material RenderMaterial;

        private void OnEnable()
        {
            RenderMaterial = new Material(Shader.Find("Hidden/UnderwaterWobble"));
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, RenderMaterial);
        }
    }
}