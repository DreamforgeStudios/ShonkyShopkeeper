Shader "Custom/ToonVacuum"
{
	Properties
	{
	    [MaterialToggle] _Enabled("Enabled", Float) = 0
		_MainTex ("Texture", 2D) = "white" {}
		_Noise ("Pull Noise", 2D) = "white" {}
		_FragNoise ("Fragment Noise", 2D) = "white" {}

		//_PullPos ("Pull Position", Vector) = (0,0,0,0)
		//_Strength ("Strength", Float) = 3
		//_Range ("Range", Range(0, 10)) = 2
		//_SoftRange ("Soft Range", Range(0, 12)) = 3

		_Color ("Main Color", Color) = (0.5,0.5,0.5, 1)
		_Ramp ("Ramp", 2D) = "gray" {}
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}

		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "VACUUM"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			//#include "ShaderLib.cginc"

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
				float3 worldVertex : TEXCOORD1;
				float3  diffuse : TEXCOORD2;
				float3 add : TEXCOORD3;
			};

			sampler2D _MainTex;
			sampler2D _Ramp;
			sampler2D _Noise;
			sampler2D _FragNoise;
			float4 _Noise_ST;
			//float4 _MainTex_ST;

			float4 _PullPos;
			float _Range;
			float _SoftRange;
			float4 _Color;
			float _Enabled;

			float _Strength;

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _Noise);
				
                float3 worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldOrigin = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                float distvert = distance(_PullPos, worldVertex);
                float distorigin = distance(_PullPos, worldOrigin);

                o.worldVertex = worldVertex;

                // Interp value between _Range and _SoftRange.
                float val = smoothstep(_Range, _SoftRange, distvert);
                val = lerp(1, 0, val);
                val = clamp(0, 1, val);

                // Sample noise.  (sampling in vertex shader requires )
                float4 noise = tex2Dlod(_Noise, float4(o.uv.x * _Time.x, o.uv.y * _Time.x, 0, 1));
                noise += ((1-noise) * (1-clamp(0, 1, distorigin / 3)));

                // Vertex to add...
                float3 toAdd = (_PullPos - worldVertex) * val * noise;
                toAdd = mul(unity_WorldToObject, float4(toAdd.xyz,0.0));
                // Pass it to the pixel shader.
                o.add = toAdd; 
                v.vertex += float4(toAdd, 1) * _Enabled;

				o.vertex = UnityObjectToClipPos(v.vertex);
				
				// DIFFUSE.
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = dot(worldNormal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;
				o.diffuse = nl;//_WorldSpaceLightPos0.xyz;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 noise = tex2D(_FragNoise, i.uv);
				fixed4 col = tex2D(_MainTex, i.uv);
				half3 ramp = tex2D(_Ramp, float2(i.diffuse.x, 0));

				col.rgb *= ramp * _Color;

				//col = i.diff;
				//col.a = 1;

				float dist = 1-smoothstep(0, length(_PullPos - i.worldVertex), length(i.add));
				col.a -= (1-pow(dist, _Strength)) * _Enabled;

				return col;
			}

			ENDCG
		}
	}
}
