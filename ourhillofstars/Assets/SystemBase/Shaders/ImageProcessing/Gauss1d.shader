Shader "Hidden/Gauss1d"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_PixelStepX("Pixel Step X", Float) = 0
		_PixelStepY("Pixel Step Y", Float) = 1
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
	{
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag

		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform float _PixelStepX;
		uniform float _PixelStepY;

		float4 frag(v2f_img i) : COLOR
		{
			float2 _PixelStep = float2(_PixelStepX, _PixelStepY) / _ScreenParams.xy;		
			fixed3 thisPixel = tex2D(_MainTex, i.uv).rgb;
			float3 sum = 0.25f*(tex2D(_MainTex, i.uv - _PixelStep).rgb + thisPixel + thisPixel + tex2D(_MainTex, i.uv + _PixelStep).rgb);
			return float4(sum.r, sum.g, sum.b, tex2D(_MainTex, i.uv).a);
		}
		ENDCG
		}
	}
}
