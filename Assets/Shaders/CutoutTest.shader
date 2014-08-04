 Shader "Custom/Dissolving" {
    Properties {
      _MainTex ("Texture (RGB)", 2D) = "white" {}
      _Color ("Main Color", Color) = (1,1,1,1)
      _SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
      _SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      Cull Off
      CGPROGRAM
      //if you're not planning on using shadows, remove "addshadow" for better performance
      #pragma surface surf Lambert addshadow finalcolor:mycolor
      struct Input {
          float2 uv_MainTex;
          float2 uv_SliceGuide;
          float _SliceAmount;
          float4 color: Color;
      };
      sampler2D _MainTex;
      sampler2D _SliceGuide;
      float _SliceAmount;
      fixed4 _Color;
      void mycolor (Input IN, SurfaceOutput o, inout fixed4 color)
      {
          color *= _Color;
      }
      
      void surf (Input IN, inout SurfaceOutput o) {
          clip(tex2D (_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount);
          o.Albedo = _Color * IN.color;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }