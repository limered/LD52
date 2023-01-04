Shader "Hidden/Vignette"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Size("Vignette Size", Float) = 0.75
		_Brightness("Window Brightness", Float) = 15
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
		uniform float _Size;
		uniform float _Brightness;

		fixed4 frag(v2f_img i) : COLOR
		{
			float2 uv = i.uv;
			uv *= 1.0 - uv.yx;
			float vig = uv.x*uv.y * _Brightness;
			vig = pow(vig, _Size);
			fixed4 base = tex2D(_MainTex, i.uv);
			fixed4 vign = fixed4(vig, vig, vig, vig);
			return base * vign;
        }
        ENDCG
        }
    }
}
