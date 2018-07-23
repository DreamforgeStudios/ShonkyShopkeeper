// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Gem"
{
	Properties
	{
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
		Tags { "LightMode"="ForwardBase" }

		Pass
		{
			Name "LIGHTING"

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

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
				float4 diff : COLOR0; 
				float3 worldNormal : TEXCOORD1;
				float4 originalVert : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Ramp;
			sampler2D _SpecRamp;

			float4 _MainColor;
			float4 _DiffuseMaxColor;
			float4 _SpecularColor;
			float _Specular;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.originalVert = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = worldNormal;
				
				half nl;
				if (_WorldSpaceLightPos0.w == 0) {
				    nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				} else {
				    nl = max(0, dot(worldNormal, -_WorldSpaceLightPos0.xyz));
				}
				
				float4 lightPos = float4(unity_4LightPosX0[0], unity_4LightPosY0[0], unity_4LightPosZ0[0], 1);
				float distance = length(lightPos - mul(unity_ObjectToWorld, v.vertex));
				float attenuation = max(0, 1-(distance / ( 1 / _LightPositionRange.w)));
				
				nl += max(0, dot(worldNormal, lightPos)) * attenuation;
				
				o.diff = nl;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float3 viewDir = normalize(WorldSpaceViewDir(i.originalVert));
				float3 reflection = normalize(reflect(-_WorldSpaceLightPos0.xyz, i.worldNormal));
				float spec = pow(max(0, dot(reflection, viewDir)), _Specular);
				float4 lightPos = float4(unity_4LightPosX0[0], unity_4LightPosY0[0], unity_4LightPosZ0[0], 1);
				reflection = normalize(reflect(-lightPos, i.worldNormal));
				spec += pow(max(0, dot(reflection, viewDir)), _Specular);
				//spec += i.diff;

				float diff_ramp = tex2D(_Ramp, float2(i.diff.x, 0)).r;
				float spec_ramp = tex2D(_SpecRamp, float2(spec, 0)).r;
				//float4 ramp = tex2D(_Ramp, float2(spec + i.diff.x, 0)).rrrr;


				float4 diffuse = _MainColor * diff_ramp;
				float4 specular = _SpecularColor * spec_ramp;

                if (diff_ramp >= 1) {
                    col = _DiffuseMaxColor + specular;
                } else {
                    col = col * diffuse + specular;
                }

				return col * _LightColor0;
			}

			ENDCG
		}
		
		/*
		Tags { "LightMode"="ForwardAdd" }
		
		Pass
		{
			Name "LIGHTINGADD"
			
			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "Lighting.cginc"

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
				float4 diff : COLOR0; 
				float3 worldNormal : TEXCOORD1;
				float4 originalVert : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Ramp;
			sampler2D _SpecRamp;

			float4 _MainColor;
			float4 _DiffuseMaxColor;
			float4 _SpecularColor;
			float _Specular;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.originalVert = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = worldNormal;
				half nl;
				if (_WorldSpaceLightPos0.w == 0) {
				    nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				} else {
				    nl = max(0, dot(worldNormal, -_WorldSpaceLightPos0.xyz));
				}
				
				o.diff = nl;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    return _LightColor0;
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float3 viewDir = normalize(WorldSpaceViewDir(i.originalVert));
				float3 reflection = normalize(reflect(-_WorldSpaceLightPos0.xyz, i.worldNormal));
				float spec = pow(max(0, dot(reflection, viewDir)), _Specular);
				//spec += i.diff;

				float diff_ramp = tex2D(_Ramp, float2(i.diff.x, 0)).r;
				float spec_ramp = tex2D(_SpecRamp, float2(spec, 0)).r;
				//float4 ramp = tex2D(_Ramp, float2(spec + i.diff.x, 0)).rrrr;


				float4 diffuse = _MainColor * diff_ramp;
				float4 specular = _SpecularColor * spec_ramp;

                if (diff_ramp >= 1) {
                    col = _DiffuseMaxColor + specular;
                } else {
                    col = col * diffuse + specular;
                }

				return col * _LightColor0;
			}

			ENDCG
		}
	*/
	}
	
	Fallback "Diffuse"
}
