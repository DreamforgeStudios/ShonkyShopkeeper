Shader "Custom/Sketch"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SketchTex ("Sketch Texture", 2D) = "white" {}
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_Multiplier ("Sketch Cutoff", Range(0, 1)) = .5
		_OffsetVal ("Offset Value", Range(0, 0.2)) = .05
	}
	
	SubShader
	{
		Tags { "LightMode"="ForwardBase" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 diff : COLOR0;
			};

			sampler2D _MainTex;
			sampler2D _SketchTex;
			
			float4 _MainTex_ST;
			float4 _SketchTex_ST;
			
			float4 _MainColor;
			float _Multiplier;
			
			float _OffsetVal;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _SketchTex);
				
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				//o.diff = nl * _LightColor0;
				o.diff = nl;
				return o;
			}
			
			float exp_step(float x, float k, float n) {
			    return exp(-k*pow(x, n));
			}
			
			float gain(float x, float k) {
			    float a = 0.5 * pow(2.0 * ((x < 0.5) ? x : 1.0 - x) , k);
			    return (x < 0.5) ? a : 1.0 - a;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    float2 offsets[4] = {
			        float2(_OffsetVal, 0),
			        float2(-_OffsetVal, 0),
			        float2(0, _OffsetVal),
			        float2(0, -_OffsetVal)
			    };
			
				// sample the texture
				fixed4 ramp = tex2D(_MainTex, float2(i.diff.x, i.diff.x));
				fixed4 sketch = tex2D(_SketchTex, i.uv + offsets[_Time.y * 12 % 4]);
				sketch = 1-sketch;
				sketch = gain(sketch, (1-i.diff) * 3);
				//sketch *= _MainColor;
				
				
				float diff = i.diff;
				float diffcurve = 1-exp_step(diff, 10, 2);
				sketch *= ramp;
				sketch = 1-exp_step(sketch, 50, 1);
				sketch *= _MainColor;
				
				return lerp(sketch, _MainColor, ramp);
			}
			
			ENDCG
		}
	}
}
