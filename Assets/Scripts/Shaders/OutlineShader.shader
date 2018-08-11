Shader "Toon/Outline"
{
	Properties
	{
		_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineThickness ("Outline Thickness", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
		    Name "ONE"
		
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 _GlowColor;
			
			fixed4 frag (v2f i) : SV_Target
			{
			    return float4(0,0,0,1);
			    
			    //return (step(float2(.1,.1), i.uv) - step(float2(.9, .9), i.uv)).rrrr;
			    //return i.uv.rgrg;
			}
			
			ENDCG
		}
		
		Pass
		{
		    Name "TWO"
		    
		    Cull Front
		    
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			float _OutlineThickness;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex*_OutlineThickness);
				o.uv = v.uv;
				return o;
			}
			
			float4 _OutlineColor;
			
			fixed4 frag (v2f i) : SV_Target
			{
			    return _OutlineColor;
			    
			    //return (step(float2(.1,.1), i.uv) - step(float2(.9, .9), i.uv)).rrrr;
			    //return i.uv.rgrg;
			}
			
			ENDCG
		}
	}
}
