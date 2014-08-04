Shader "Custom/Showthrough" 
{
 Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "white" {}
        _Reflections ("Base (RGB) Gloss (A)", Cube) = "skybox" { TexGen CubeReflect }
    }
    SubShader {
        Tags { "Queue" = "Transparent" }
        Pass {
        ZTest Greater
            Blend SrcAlpha OneMinusSrcAlpha
            Material {
                Diffuse [_Color]
            }
            Lighting On
            SetTexture [_MainTex] {
                combine texture * primary double, texture * primary
            }
        }
        Pass {
        ZTest Less
            Blend One One
            Material {
                Diffuse [_Color]
            }
            Lighting On
            SetTexture [_Reflections] {
                combine texture
                Matrix [_Reflection]
            }
        }
    }
} 