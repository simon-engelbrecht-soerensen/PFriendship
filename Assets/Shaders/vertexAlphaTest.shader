Shader "Custom/Lightmapped, Vertex Colored, A Blend" {

 

Properties

{

    _MainTex ("Lightmap", 2D) = ""

}

 

SubShader 

{

    Blend SrcAlpha OneMinusSrcAlpha

    BindChannels

    {

        Bind "Vertex", vertex

        Bind "Texcoord", texcoord

        Bind "Color", color

    }

 

    Pass

    {      

        SetTexture[_MainTex] {Combine primary * texture Double}

    }

}

 

 

}