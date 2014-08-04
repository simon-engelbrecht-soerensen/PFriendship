Shader "Custom/No Normals 4 Vertex Color No Light" {

Properties {

    _MainTex ("Texture", 2D) = "white" {}

}

 

Category {

    Tags { "Queue"="Geometry"}

    Lighting Off

    BindChannels {

        Bind "Color", color

        Bind "Vertex", vertex

        Bind "TexCoord", texcoord

    }

    

    // ---- Dual texture cards

    SubShader {

        Pass {

            SetTexture [_MainTex] {

                combine texture * primary

            }

            SetTexture [_MainTex] {

                constantColor (1,1,1,1)

                combine previous lerp (previous) constant

            }

        }

    }

    

    // ---- Single texture cards (does not do vertex colors)

//    SubShader {
//
//        Pass {
//
//            SetTexture [_MainTex] {
//
//                constantColor (1,1,1,1)
//
//                combine texture lerp(texture) constant
//
//            }
//
//        }
//
//    }

}

}