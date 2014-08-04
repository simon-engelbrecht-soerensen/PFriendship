Shader "Custom/Showthrough2" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _SpecColor ("Spec Color", Color) = (1,1,1,0)
    	_Emission ("Emissive Color", Color) = (0,0,0,0)
    	_Shininess ("Shininess", Range (0.1, 1)) = 0.7
    }
 
    Category 
    {
        SubShader 
        { 
            Tags {"RenderType"="Transparent" "Queue"="Transparent" } 
 			Pass
            {
            	Blend SrcAlpha OneMinusSrcAlpha 
				ZTest Less
				Material {
			Diffuse [_Color]
			Ambient [_Color]
			Shininess [_Shininess]
			Specular [_SpecColor]
			Emission [_Emission]
		} 
		ZTest Less 
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting On
		SeparateSpecular On
		SetTexture [_MainTex] {
			Combine primary DOUBLE, primary
		} 
//			CGPROGRAM 

//			#pragma surface surf Lambert vertex:vert alpha 
			
                
//                SetTexture [_MainTex] {combine texture}
//                Lighting Off
//				foran ting

//                Color [_Color] 
//                ENDCG
            }
            Pass 
            {
//             Blend SrcAlpha OneMinusSrcAlpha
//                ZTest Greater          
//                //bagved
////                Lighting Off
//                Color [_Color] 
//                
//            }
				
				ZTest Greater
		  		ZWrite Off
		        Blend SrcAlpha OneMinusSrcAlpha
		        ColorMask RGB
		        Material {
		            Diffuse [_Color]
		            Ambient [_Color]
		            Shininess [_Shininess]
		            Specular [_SpecColor]
		            Emission [_Emission]
		        }
		        Lighting On
		        SetTexture [_MainTex] {
		            Combine texture * primary DOUBLE, texture * primary
		        }
	        }
            
            
        }
    }
 
    FallBack "Specular", 1
}