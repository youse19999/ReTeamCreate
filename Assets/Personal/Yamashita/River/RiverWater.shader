Shader "Custom/RiverWater"
{
    Properties
    {
        _Color ("Water Color", Color) = (0.1, 0.5, 0.7, 0.6)

        _MainTex ("Water Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}

        _FlowX ("Flow X", Float) = 0
        _FlowY ("Flow Y", Float) = 0.2

        _Smoothness ("Smoothness", Range(0,1)) = 0.9
        _Transparency ("Transparency", Range(0,1)) = 0.65
        _NormalStrength ("Normal Strength", Range(0,2)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM

        #pragma surface surf Standard alpha:fade

        sampler2D _MainTex;
        sampler2D _BumpMap;

        fixed4 _Color;

        float _FlowX;
        float _FlowY;

        half _Smoothness;
        half _Transparency;
        half _NormalStrength;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 flow = float2(_FlowX, _FlowY) * _Time.y;

            fixed4 water =
                tex2D(_MainTex, IN.uv_MainTex + flow)
                * _Color;

            fixed3 normal =
                UnpackNormal(
                    tex2D(
                        _BumpMap,
                        IN.uv_BumpMap + flow
                    )
                );

            normal.xy *= _NormalStrength;

            o.Albedo = water.rgb;
            o.Normal = normal;

            o.Metallic = 0;
            o.Smoothness = _Smoothness;

            o.Alpha = water.a * _Transparency;
        }

        ENDCG
    }

    FallBack "Transparent/Diffuse"
}