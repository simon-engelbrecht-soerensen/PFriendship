Shader "Custom/VertexBlend"
{
	Properties 
	{
_Diffuse1("Diffuse A", 2D) = "black" {}
_Diffuse2("Diffuse B", 2D) = "black" {}
_BlendMask("Diffuse B Blend", 2D) = "black" {}
_BlendPower("_BlendPower", Float) = 2
_Normal1("Normal A", 2D) = "black" {}
_Normal2("Normal B", 2D) = "black" {}

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Off
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _Diffuse1;
sampler2D _Diffuse2;
sampler2D _BlendMask;
float _BlendPower;
sampler2D _Normal1;
sampler2D _Normal2;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_Diffuse1;
float2 uv_Diffuse2;
float2 uv_BlendMask;
float4 color : COLOR;
float2 uv_Normal1;
float2 uv_Normal2;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Sampled2D0=tex2D(_Diffuse1,IN.uv_Diffuse1.xy);
float4 Sampled2D1=tex2D(_Diffuse2,IN.uv_Diffuse2.xy);
float4 Sampled2D2=tex2D(_BlendMask,IN.uv_BlendMask.xy);
float4 Multiply3=Sampled2D1 * Sampled2D2;
float4 Splat0=Multiply3.x;
float4 Multiply0=Splat0 * IN.color;
float4 Multiply1=Multiply0 * float4( 4,4,4,4 );
float4 Multiply2=IN.color * float4( 2,2,2,2 );
float4 Add2=Multiply1 + Multiply2;
float4 Pow0=pow(Add2,_BlendPower.xxxx);
float4 Saturate0=saturate(Pow0);
float4 Lerp0=lerp(Sampled2D0,Sampled2D1,Saturate0);
float4 Sampled2D4=tex2D(_Normal1,IN.uv_Normal1.xy);
float4 UnpackNormal0=float4(UnpackNormal(Sampled2D4).xyz, 1.0);
float4 Sampled2D3=tex2D(_Normal2,IN.uv_Normal2.xy);
float4 UnpackNormal1=float4(UnpackNormal(Sampled2D3).xyz, 1.0);
float4 Lerp2=lerp(UnpackNormal0,UnpackNormal1,Saturate0);
float4 Lerp1=lerp(Sampled2D0.aaaa,Sampled2D1.aaaa,Saturate0);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp0;
o.Normal = Lerp2;
o.Gloss = Lerp1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}