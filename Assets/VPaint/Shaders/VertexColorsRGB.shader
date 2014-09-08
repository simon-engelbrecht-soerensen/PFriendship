Shader "VPaint/Unlit/VertexColorsRGB"
{
	 Properties {
	      _MainTex ("Texture (RGB)", 2D) = "white" {}
	      _Color ("Main Color", Color) = (1,1,1,1)
	      _SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
	      _SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
    }

	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			struct appdata {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f o) : COLOR
			{
				
				return o.color;
			}
			ENDCG
		}
	} 
}
