using UniRx;
using UnityEngine;

namespace SystemBase.Shaders.ImageProcessing
{
    public class SimpleToonEffekt : MonoBehaviour
    {
        private Material material;
        public FloatReactiveProperty Offset = new FloatReactiveProperty(0.0002f);
        public FloatReactiveProperty Threshold = new FloatReactiveProperty(0.01f);
        public FloatReactiveProperty ColorDarken = new FloatReactiveProperty(0.6f);

        private void Awake()
        {
            material = new Material(Shader.Find("Hidden/SimpleToon"));

            Offset.Subscribe(OnOffsetChange);
            Threshold.Subscribe(OnThresholdChange);
            ColorDarken.Subscribe(OnColorDarkenChange);
        }

        private void OnColorDarkenChange(float value)
        {
            material.SetFloat("_ColorMultiplier", value);
        }

        private void OnThresholdChange(float value)
        {
            material.SetFloat("_Threshold", value);
        }

        private void OnOffsetChange(float value)
        {
            material.SetFloat("_Offset", value);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}
