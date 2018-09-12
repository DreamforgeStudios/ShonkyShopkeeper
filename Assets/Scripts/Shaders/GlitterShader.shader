Shader "Custom/GlitterShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 color : COLOR;
				//float rot : TEXCOORD1;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD1;
				float rot : TEXCOORD2;
				float4 color : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			fixed4 _UpDirection;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.normal = UnityObjectToWorldNormal(v.normal);
				o.normal = v.normal;
				o.uv = v.uv.xy;
				o.rot = v.uv.z;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				float val = 0;
				val += round(_UpDirection.x * 3);
				
				// sample the texture
				fixed4 col = tex2D(_MainTex, (i.uv * .25f) + .25f * val) * i.color;
				return col * i.rot * 2;
				//return float4(i.rot.xxx, 1);
				//return col;
			}
			ENDCG
		}
	}
}
