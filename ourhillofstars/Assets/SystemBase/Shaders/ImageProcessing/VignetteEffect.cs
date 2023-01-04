using UniRx;
using UnityEngine;

namespace SystemBase.Shaders.ImageProcessing
{
    public class VignetteEffect : MonoBehaviour
    {
        public Material material;

        public FloatReactiveProperty Size = new FloatReactiveProperty(0.75f);
        public FloatReactiveProperty Brightness = new FloatReactiveProperty(15);

        private void OnEnable()
        {
            material = new Material(Shader.Find("Hidden/Vignette"));

            Size.Subscribe(f => material.SetFloat("_Size", f));
            Brightness.Subscribe(f => material.SetFloat("_Brightness", f));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}
