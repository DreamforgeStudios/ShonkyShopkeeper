Shader "Custom/Toon"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "gray" {}
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }

		Pass
		{
			Name "TOON"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 diffuse : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _Ramp;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half d = dot(worldNormal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;
				o.diffuse = d;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				half3 ramp = tex2D(_Ramp, float2(i.diffuse.x, 0));

				col.rgb *= ramp * _Color;

				return col;
			}

			ENDCG
		}
	}
}
