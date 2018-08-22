// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteGradient" {
Properties {
    _MainTex ("Main Texture", 2D) = "white" {}
    
    _MaxRadius ("Maximum Radius", Range(-180, 180)) = 180
    _MinRadius ("Maximum Radius", Range(-180, 180)) = -180
     
    _Radius("Radius", Range(0, 1)) = 0.5
    _RadiusWidth("Thickness", Range(0,1)) = 0.1
    
    _CursorPosition("Cursor Position", Range(-180, 180)) = 180
 }
  
 SubShader {
    Tags {"Queue"="Overlay"}
  
    ZTest Always
  
    Pass {
        CGPROGRAM
        
        #pragma vertex vert  
        #pragma fragment frag
        #include "UnityCG.cginc"
         
        float _Radius, _RadiusWidth;
        float _MaxRadius, _MinRadius;
        
        float3 _DefaultColor;
        float _CursorPosition;
         
        uniform float3 _Points [30];
        uniform float3 _Colors [30];
        uniform float _PointsLength = 0;
         
        const float PI = 3.1415926535;
  
        struct v2f {
            float4 pos : SV_POSITION;
            float3 original : TEXCOORD0;
            float2 uv : TEXCOORD1;
            //fixed4 col : COLOR;
        };
        
        float inverselerp(float a, float b, float x) {
	        x = clamp(a, b, x);
	        return (x - a) / (b - a);
        }
  
        v2f vert (appdata_full v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.original = normalize(v.vertex);
            o.uv = v.texcoord;
            //o.col = lerp(_Color,_Color2, v.texcoord.x );
            // o.col = half4( v.vertex.y, 0, 0, 1);
            return o;
        }
        
  
        float4 frag (v2f i) : COLOR {
            float4 c = float4(_DefaultColor, 1);
            
            // Cut parts of the circle that are beyond the given angles.
            float fragAngle = degrees(atan2(i.original.y, i.original.x));
            if (fragAngle > _MaxRadius || fragAngle < _MinRadius) {
                clip(-1);
            }
            
            // Radius checking (make the quad into a circle).
            // TODO: cleanup.
            float rad = _RadiusWidth;
            if (fragAngle < _CursorPosition + 10 &&
                fragAngle > _CursorPosition - 10) {
                float stepamount = inverselerp(10, 0, abs(fragAngle - _CursorPosition));
                float p = 2.0 * stepamount * stepamount;
                stepamount = stepamount < 0.5 ? p : -p + (4.0 * stepamount) - 1.0;
                rad += stepamount * 0.03;
            }
            
            float d = distance(float4(0,0,0,1), i.original);
            float r = lerp(0, 0.5 - rad, _Radius / 1);
            if (!(d > r && d < r + rad)) {
                clip(-1);
            }
            
            // Loop through each point that has been sent to the shader.
            // Point.x = angle.
            // Point.y = size.
            // Point.z = type = 0, 1, 2, 3 = best, good, average, worst.
            for (int i = 0; i < _PointsLength; i++) {
                if (fragAngle > _Points[i].x - _Points[i].y * .5 &&
                    fragAngle < _Points[i].x + _Points[i].y * .5) {
                    c = float4(_Colors[i], 1);
                    /*
                    if (_Points[i].z == 0) {
                        c = _Color;
                    } else if (_Points[i].z == 1) {
                        c = _Color2;
                    } else if (_Points[i].z == 2) {
                        c = _Color3;
                    } else if (_Points[i].z == 3) {
                        c = _Color4;
                    }
                    */
                }
            }
            
            
            return c;
        }
        
        ENDCG
       }
    }
}