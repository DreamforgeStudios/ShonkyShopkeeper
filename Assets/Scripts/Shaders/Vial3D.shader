﻿Shader "Unlit/Vial3D"
{
	Properties {
	    _Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_EmptyTex ("Empty Texture", 2D) = "white" {}
		_FillAmount ("Fill Amount", Range(-1, 1)) = 0.0
		[HideInInspector] _WobbleX ("WobbleX", Range(-1, 1)) = 0.0
		[HideInInspector] _WobbleZ ("WobbleZ", Range(-1, 1)) = 0.0
		_TopColor ("Top Color", Color) = (1,1,1,1)
		_FoamColor ("Foam Line Color", Color) = (1,1,1,1)
		_Rim ("Foam Line Width", Range(0,0.1)) = 0.0
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0,10)) = 0.0
		
		//_UpDirection ("Up Direction", Vector) = (0,1,0,1)
	}
	
	SubShader {
		Tags { "Queue"="Geometry" "DisableBatching" = "True" }

		Pass
		{
		    ZWrite On
		    Cull Off
		    AlphaToMask On
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewDir : COLOR;
				float3 normal : COLOR2;
				float fillEdge : TEXCOORD2;
			};

			sampler2D _MainTex, _EmptyTex;
			half _FillAmount, _WobbleX, _WobbleZ;
			fixed4 _TopColor, _RimColor, _FoamColor, _Tint;
			fixed _Rim, _RimPower;
			
			fixed4 _UpDirection;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				
				/*
				// world pos of vertex.
				// rotate around xy.
				float3 worldPosX = RotateAroundYInDegrees(float4(worldPos,0), 360);
				// rotate around xz.
				float3 worldPosZ = float3(worldPosX.y, worldPosX.z, worldPosX.x);
				// combine rotations with worldPos, based on sine wave from script.
				float3 worldPosAdjusted = worldPos + (worldPosX * _WobbleX) + (worldPosZ * _WobbleZ);
				// how high up the liquid is.
				*/
				//o.fillEdge = worldPosAdjusted.y + _FillAmount;
				
				// find vector origin in world space.
				//fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				//fixed3 worldOriginPos = mul(unity_ObjectToWorld, fixed4(0,0,0,1));
				//fixed3 realWorld = worldPos - worldOriginPos;
				
				// It should actually be the UpDirection which is flipped, but this is easier to change if
				//  we decide to go back to world positioning.
				fixed3 realWorld = fixed3(-v.vertex.y, v.vertex.x, -v.vertex.z);
				 
				fixed3 normalizedUp = normalize(_UpDirection.xyz);
				
				fixed worldPosX = realWorld.x * _WobbleX;
				fixed worldPosZ = realWorld.z * _WobbleZ;
				
				fixed likeness = dot(normalizedUp, realWorld + worldPosX + worldPosZ);
				o.fillEdge = likeness - _FillAmount;
				
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				o.normal = v.normal;
				
				return o;
			}
			
			fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target {
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Tint;
                fixed4 emptyCol = tex2D(_EmptyTex, i.uv) * _Tint;
                
                // rim light
                fixed dotProduct = 1 - pow(dot(i.normal, i.viewDir), _RimPower);
                fixed4 RimResult = smoothstep(0.5, 1.0, dotProduct);
                RimResult *= _RimColor;
                
                // foam edge
                fixed4 foam = (step(i.fillEdge, 0.5) - step(i.fillEdge, (0.5 - _Rim)))  ;
                fixed4 foamColored = foam * _FoamColor;
                // rest of the liquid
                fixed4 result = step(i.fillEdge, 0.5) - foam;
                fixed4 resultColored = result * col + (1-(result + foam)) * emptyCol;
                
                // both together, with the texture
                fixed4 finalResult = resultColored + foamColored;               
                finalResult.rgb += RimResult;
 
                // color of backfaces/ top
                fixed4 topColor = _TopColor * (foam + result);
                
                /*
                if (i.fillEdge > .5) {
                    return emptyCol;
                }
                */
                
                //VFACE returns positive for front facing, negative for backfacing
                return facing > 0 ? finalResult : topColor;
			}
			
			ENDCG
		}
	}
}
