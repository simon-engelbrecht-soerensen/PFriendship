Shader "Custom/No Normals_CutoffGlowInSteps" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_LightCutoff("Maximum distance", Float) = 2.0
		_Steps ("Gradient steps", Float) = 10.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf WrapLambert fullforwardshadows
		#pragma target 3.0
		uniform float _LightCutoff;
		uniform float _Steps;
		
		half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot (s.Normal, lightDir);
			//atten = step(_LightCutoff, atten) * atten;
			_Steps = int(_Steps);
			atten = int(atten * _Steps) / float(_Steps);
			atten = atten < 1 - _LightCutoff ? 0 : atten;
			half vMax = (max(max(s.Albedo.r, s.Albedo.g), s.Albedo.b));
			half3 colorAdjust = vMax > 0 ? s.Albedo / vMax : 1;
			half4 c;
			c.rgb = _LightColor0.rgb * atten;
			c.a = s.Alpha;
			return c;
		}
	
		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR; //vertex color
		};
		
		sampler2D _MainTex;
		//half4 _Color;
		
		void surf (Input IN, inout SurfaceOutput o) {
			//o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * IN.color.rgb;
		}
		ENDCG
	}
	Fallback "Diffuse"
}