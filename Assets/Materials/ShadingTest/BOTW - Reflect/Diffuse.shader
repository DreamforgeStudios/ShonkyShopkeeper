// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/GemReflect"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "white" {}
		_SpecRamp ("Specular Ramp", 2D) = "white" {}
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_DiffuseMaxColor ("Diffuse Max Color", Color) = (1,1,1,1)
		_SpecularColor ("Specular Color", Color) = (1,1,1,1)
		_Specular ("Specular Power", Range(0, 5)) = 0
	}
	
	SubShader
	{
		Pass
		{
			Name "LIGHTING"
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "ShaderLib.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float  diff : TEXCOORD1; 
				float3 worldNormal : TEXCOORD2;
				float4 originalVert : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Ramp;
			sampler2D _SpecRamp;

			float4 _MainColor;
			float4 _DiffuseMaxColor;
			float4 _SpecularColor;
			float  _Specular;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.originalVert = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				o.diff = diffuse_directional(_WorldSpaceLightPos0.xyz, v.normal);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float3 viewDir = normalize(WorldSpaceViewDir(i.originalVert));
				float3 reflection = normalize(reflect(-viewDir, i.worldNormal));
				
				float4 colReflect = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflection);

                float spec = specular_directional(i.originalVert, _WorldSpaceLightPos0.xyz, i.worldNormal, _Specular);
				float diff_ramp = tex2D(_Ramp, float2(i.diff.x, 0)).r;
				float spec_ramp = tex2D(_SpecRamp, float2(spec, 0)).r;
				
				float4 diffuse = _MainColor * diff_ramp;
				float4 specular = _SpecularColor * spec_ramp;

                // TODO: there's probably a way to optimize out this if.
                if (diff_ramp >= 1) {
                    col = _DiffuseMaxColor + specular;
                } else {
                    col = col * diffuse + specular;
                }

				return col * _LightColor0 + colReflect;
			}

			ENDCG
		}
		
		
		/*
		Pass
		{
			Name "LIGHTINGADD"
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
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
			sampler2D _SpecRamp;

			float4 _MainColor;
			float4 _SpecularColor;
			float _Specular;
			
			v2f vert (appdata v)
			{
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
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

                float spec = specular_point(i.originalVert, _WorldSpaceLightPos0.xyz, i.worldNormal, _Specular);

                float atten = LIGHT_ATTENUATION(i);
				float diff_ramp = tex2D(_Ramp, float2(i.diff, 0)).r;
				float spec_ramp = tex2D(_SpecRamp, float2(spec, 0)).r;
				//float4 ramp = tex2D(_Ramp, float2(spec + i.diff.x, 0)).rrrr;


				float4 diffuse = lerp(_MainColor, _LightColor0, atten) * diff_ramp * atten;
				float4 specular = lerp(_SpecularColor, _LightColor0, atten) * spec_ramp;

                col = col * diffuse + specular;
                
                return col;
				//return col + lerp(float4(0,0,0,0), _LightColor0, atten);
			}

			ENDCG
		}
	*/
	}
	
	Fallback "Diffuse"
}
