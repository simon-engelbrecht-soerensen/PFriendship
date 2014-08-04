  Shader "Custom/No Normals 4 Vertex Color" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
	_LightCutoff("Maximum distance", Float) = 2.0
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM		
      	#pragma surface surf WrapLambert

		uniform float _LightCutoff;
		
      		half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
          half NdotL = dot (s.Normal, lightDir);

		  	atten = step(_LightCutoff, atten) * atten;
          	half4 c;
          	c.rgb = _LightColor0.rgb * (atten);
          	c.a = s.Alpha;
          	return c;
      }

      struct Input {
          float2 uv_MainTex;
          float4 color : COLOR; //vertex color

      };
      sampler2D _MainTex;
       
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * IN.color.rgb;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }