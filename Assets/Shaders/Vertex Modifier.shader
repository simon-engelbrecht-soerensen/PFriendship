 Shader "Custom/Vertex Modifier" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
//      _Amount ("Amount", Float) = 1.0
      _Speed ("Speed", Float) = 5.0
	  _XYZ ("Amount", Vector) = (1,1,1)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert
      struct Input {
          float2 uv_MainTex;
          float4 color: Color; // Vertex color
      };
 
      // Access the shaderlab properties
      float _Amount;
      float _Speed;
      float4 _XYZ;
      sampler2D _MainTex;
 
	 float rand(float3 co)
	{
	    return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
	}
      // Vertex modifier function
      void vert (inout appdata_full v) {
          // Do whatever you want with the "vertex" property of v here
//          float amt = _Amount * _SinTime;
			float sinV = sin(_Time * _Speed);
         	v.vertex.z += _XYZ.z * sinV * v.color.r;
         	v.vertex.x += _XYZ.x * sinV * v.color.r;
         	v.vertex.y += _XYZ.y * sinV * v.color.r;
//          	v.vertex.x += _XYZ.x * v.color.r;
//          	v.vertex.y += _XYZ.y * v.color.r;

      }
 
      // Surface shader function
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
// 			half4 c = tex2D (_MainTex, IN.uv_MainTex);
//            o.Albedo = c.rgb * IN.color.rgb; // vertex RGB
      }
      ENDCG
    }
  }