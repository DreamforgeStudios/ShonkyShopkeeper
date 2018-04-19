Shader "Custom/CurveShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _CurveTex("Texture", 2D) = "white" {}
        _CurveMult("Curve Multiplier", float) = 0
	}
	SubShader
	{
		//Tags { "RenderType"="Transparent" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 cuv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _CurveTex;
			float4 _MainTex_ST;
			sampler2D _CurveTex_ST;
            float _CurveMult;
            float _UVOffset;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.cuv = v.uv;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
                half2 curve = half2(tex2D(_CurveTex, i.cuv).r, 0);
				fixed4 col = tex2D(_MainTex, i.uv+_UVOffset-(half2((1-curve.x)*_CurveMult, 0)));
                col.rgb -= curve.r;
                // return...
				return col;
			}
			ENDCG
		}
	}
}
