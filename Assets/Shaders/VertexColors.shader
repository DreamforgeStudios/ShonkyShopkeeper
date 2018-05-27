Shader "Vertex Colors" {
    Subshader {
		Tags {"Queue" = "Overlay" "RenderType" = "Overlay"}
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha

            struct appdata {
                float4 vertex : POSITION;
				float4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
				float4 color : COLOR;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target{
				return i.color;
            }

            ENDCG
        }
	}
}