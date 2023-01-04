Shader "Hidden/SimpleToon"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Offset("Color Offset", Float) = 0.0002
		_Threshold("Effect Threashold", Float) = 0.01
		_ColorMultiplier("Color Multiplier", Float) = 0.6
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float _Offset;
			uniform float _Threshold;
			uniform float _ColorMultiplier;

			fixed4 frag(v2f_img i) : COLOR
			{
				float4 col = tex2D(_MainTex, i.uv);
				float4 offsetCol = tex2D(_MainTex, i.uv - _Offset);
				if (length(col - offsetCol) > _Threshold) {
					col = col * _ColorMultiplier;
				}
				return col;
			}
			ENDCG
		}
	}
}
