//
// Author:
//   Based on the Unity3D built-in shaders
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

Shader "Custom/CutoutFade" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader {
		Tags {
			"Queue" = "AlphaTest"
			"IgnoreProjector" = "True"
			"RenderType" = "Overlay"
		}
		Offset -1, -1
		
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
			float4 color: Color;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = _Color * IN.color;
			o.Albedo = c.rgb;
//			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Decal/Cutout VertexLit"
}
