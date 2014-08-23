//Shader "Custom/SmokeCutout" {
//    Properties {
//      _MainTex ("Texture (RGB)", 2D) = "white" {}
//      _Color ("Main Color", Color) = (1,1,1,1)
//      _SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
//      _SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
//    }
//    SubShader {
//      Tags { "RenderType" = "Opaque" }
//      Cull Off
//      CGPROGRAM
//      //if you're not planning on using shadows, remove "addshadow" for better performance
//      #pragma surface surf Lambert 
//      struct Input {
//          float2 uv_MainTex;
//          float2 uv_SliceGuide;
//          float _SliceAmount;
//          float4 color: Color;
//      };
//      sampler2D _MainTex;
//      sampler2D _SliceGuide;
//      float _SliceAmount;
//      fixed4 _Color;
//
//      
//      void surf (Input IN, inout SurfaceOutput o) {
//          clip(tex2D (_SliceGuide, IN.uv_SliceGuide).rgb - _Color.a);
//          o.Albedo = _Color * IN.color;
//          o.Alpha = _Color.a;
//      }
//      ENDCG
//    } 
//    Fallback "Diffuse"
//  }

Shader "Custom/SmokeCutout" 
{
Properties
{
//	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
}

Category
{
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
//	Blend SrcAlpha OneMinusSrcAlpha
//	AlphaTest Greater .01
//	ColorMask RGB
	Cull Off
// Lighting Off ZWrite Off
	BindChannels
	{
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	// ---- Fragment program cards
	SubShader
	{
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			sampler2D _SliceGuide;
			
			struct appdata_t
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : POSITION;
				float3 normal : TEXCOORD3;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.normal = v.normal;
				
				return o;
			}
			
			sampler2D _CameraDepthTexture;
			
			fixed4 frag (v2f i) : COLOR
			{
				clip(tex2D (_SliceGuide, i.texcoord).rgb - i.color.a);
				return 2* i.color *  tex2D(_MainTex, i.texcoord);
			}
			ENDCG 
		}
	} 	
	
	// ---- Dual texture cards
	SubShader
	{
		Pass
		{
			SetTexture [_MainTex]
			{
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex]
			{
				combine texture * previous DOUBLE
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader
	{
		Pass
		{
			SetTexture [_MainTex]
			{
				combine texture * primary
			}
		}
	}
}
}