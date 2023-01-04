Shader "Hidden/Grain"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

#define SHOW_NOISE 0
#define SRGB 0
// 0: Addition, 1: Screen, 2: Overlay, 3: Soft Light, 4: Lighten-Only
#define BLEND_MODE 0
#define SPEED 1.0
#define INTENSITY 0.055
// What gray level noise should tend to.
#define MEAN 0.99
// Controls the contrast/variance of noise.
#define VARIANCE 0.65

float3 channel_mix(float3 a, float3 b, float3 w) {
	return float3(lerp(a.r, b.r, w.r), lerp(a.g, b.g, w.g), lerp(a.b, b.b, w.b));
}

float gaussian(float z, float u, float o) {
	return (1.0 / (o * sqrt(2.0 * 3.1415))) * exp(-(((z - u) * (z - u)) / (2.0 * (o * o))));
}

float3 madd(float3 a, float3 b, float w) {
	return a + a * b * w;
}

float3 screen(float3 a, float3 b, float w) {
	return lerp(a, float3(1.0, 1.0, 1.0) - (float3(1.0, 1.0, 1.0) - a) * (float3(1.0, 1.0, 1.0) - b), w);
}

float3 overlay(float3 a, float3 b, float w) {
	return lerp(a, channel_mix(
		2.0 * a * b,
		float3(1.0, 1.0, 1.0) - 2.0 * (float3(1.0, 1.0, 1.0) - a) * (float3(1.0, 1.0, 1.0) - b),
		step(float3(0.5, 0.5, 0.5), a)
	), w);
}

float3 soft_light(float3 a, float3 b, float w) {
	return lerp(a, pow(a, pow(float3(2.0, 2.0, 2.0), 2.0 * (float3(0.5, 0.5, 0.5) - b))), w);
}

            uniform sampler2D _MainTex;

			fixed4 frag(v2f_img i) : COLOR
			{
				//float2 ps = float2(1.0, 1.0) / _ScreenParams.xy;
				//float2 uv = i.uv * ps;
				float2 uv = i.uv;
				fixed4 color = tex2D(_MainTex, i.uv);
				#if SRGB
				color = pow(color, float4(2.2, 2.2, 2.2, 2.2));
				#endif

				float t = _Time.y * float(SPEED);
				float seed = dot(uv, float2(12.9898, 78.233));
				float noise = frac(sin(seed) * 43758.5453 + t);
				noise = gaussian(noise, float(MEAN), float(VARIANCE) * float(VARIANCE));
				
#if SHOW_NOISE
				color = float4(noise, noise, noise, noise);
#else
				float w = float(INTENSITY);

				float3 grain = float3(noise, noise, noise) * (1.0 - color.rgb);

#if BLEND_MODE == 0
				color.rgb += grain * w;
#elif BLEND_MODE == 1
				color.rgb = screen(color.rgb, grain, w);
#elif BLEND_MODE == 2
				color.rgb = overlay(color.rgb, grain, w);
#elif BLEND_MODE == 3
				color.rgb = soft_light(color.rgb, grain, w);
#elif BLEND_MODE == 4
				color.rgb = max(color.rgb, grain * w);
#endif

#if SRGB
				color = pow(color, float4(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));
#endif
				return color;
#endif
            }
            ENDCG
        }
    }
}
