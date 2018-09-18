Shader "Custom/Toon"
{
	Properties {
		_MultiLight("Multiple Lights?", float) = 0
		_Color ("Main Color", Color) = (0.5,0.5,0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "gray" {}
	}

	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }

		Pass {
			Name "TOON"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "ShaderLib.cginc"
			

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 diffuse : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _Ramp;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				//half d = dot(worldNormal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;
				o.diffuse = diffuse_directional(_WorldSpaceLightPos0.xyz, v.normal);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				half3 ramp = tex2D(_Ramp, float2(i.diffuse.x, 0));

				col.rgb *= ramp * _Color;

				return col;
			}

			ENDCG
		}
		
		Pass {
		    NAME "TOONADD"
		    Tags { "LightMode"="ForwardAdd" }
		    
		    Lighting On
		    Blend One One
		    
		    CGPROGRAM
		    
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd

			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "ShaderLib.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float  diff : TEXCOORD1; 
				float3 worldNormal : TEXCOORD2;
				float4 originalVert : TEXCOORD3;
				LIGHTING_COORDS(5,6)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Ramp;

			float4 _Color;
			
			v2f vert (appdata v) {
				v2f o;
				o.originalVert = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				float nl = 0;

                o.diff = diffuse_point(v.vertex, _WorldSpaceLightPos0.xyz, v.normal);

				TRANSFER_VERTEX_TO_FRAGMENT(o)
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);

                float atten = LIGHT_ATTENUATION(i);
				float diff_ramp = tex2D(_Ramp, float2(i.diff * atten, 0)).r;

				float4 diffuse = lerp(_Color, _LightColor0, diff_ramp) * diff_ramp;

                col.rgb *= diffuse.rgb;
                return col;
			}

			ENDCG
		}
		
		
	}
}
