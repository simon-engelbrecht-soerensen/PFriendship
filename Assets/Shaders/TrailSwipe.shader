Shader "Custom/TrailSwipe" {

Properties {

    _Color ("Main Color", Color) = (1,1,1,1)

    _MainTex ("Base (RGB)", 2D) = "white" {}
	_CutOff("CutOff", float) = 0.5
	_Width("Width", float) = 0.1
	_TrailRatio("Ratio", float) = 2

}

 

SubShader {

  Tags {"Queue"="Transparent" "RenderType"="Transparent"}

    LOD 150

  Blend SrcAlpha OneMinusSrcAlpha

CGPROGRAM

#pragma surface surf Lambert vertex:vert alpha

 
float _CutOff;
float _Width;
float _TrailRatio;

sampler2D _MainTex;

fixed4 _Color;

 

struct Input {
	
    float2 uv_MainTex;

    float3 vertColor;

};

 

void vert (inout appdata_full v, out Input o) {

    UNITY_INITIALIZE_OUTPUT(Input, o);

    o.vertColor = v.color;

}

 

void surf (Input IN, inout SurfaceOutput o) {

    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

    o.Albedo = c.rgb * IN.vertColor;
	
	float width = 0.1;
	
	float alpha;
	
	if(IN.uv_MainTex.x > _CutOff)
	{
		alpha = (1 - clamp(abs(IN.uv_MainTex.x - _CutOff)/_Width,0,1));
    }
	else
	{
		alpha = (1 - clamp(abs(IN.uv_MainTex.x - _CutOff)/(_TrailRatio * _Width),0,1));
	}
	
	alpha *= c.a;
	
	o.Alpha = alpha; 
}

ENDCG

}

 

Fallback "Diffuse"

}