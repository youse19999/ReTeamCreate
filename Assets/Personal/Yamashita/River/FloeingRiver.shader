Shader "Custom/FlowingRiver"
{
    Properties
    {
        _DeepColor ("Deep Color", Color) = (0.02, 0.25, 0.35, 1)
        _LightColor ("Light Color", Color) = (0.15, 0.65, 0.75, 1)

        _FlowSpeed ("Flow Speed", Range(-3, 20)) = 0.5
        _WaveScale ("Wave Scale", Range(1, 30)) = 8
        _WaveStrength ("Wave Strength", Range(0, 1)) = 0.5

        _Transparency ("Transparency", Range(0, 1)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _DeepColor;
            fixed4 _LightColor;

            float _FlowSpeed;
            float _WaveScale;
            float _WaveStrength;
            float _Transparency;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // éûä‘Ç…ÇÊÇ¡ÇƒêÏâ∫ï˚å¸Ç÷ó¨Ç∑
                float time = _Time.y * _FlowSpeed;

                // 1Ç¬ñ⁄ÇÃêÖó¨
                float wave1 =
                    sin(
                        uv.y * _WaveScale * 6.28
                        - time
                        + sin(uv.x * 10.0)
                    );

                wave1 = wave1 * 0.5 + 0.5;

                // 2Ç¬ñ⁄ÇÃêÖó¨
                float wave2 =
                    sin(
                        uv.y * (_WaveScale * 0.55) * 6.28
                        - time * 1.3
                        + uv.x * 15.0
                    );

                wave2 = wave2 * 0.5 + 0.5;

                // 2éÌóﬁÇÃîgÇç¨Ç∫ÇÈ
                float waterPattern =
                    wave1 * 0.65 +
                    wave2 * 0.35;

                waterPattern =
                    lerp(
                        0.5,
                        waterPattern,
                        _WaveStrength
                    );

                fixed3 color =
                    lerp(
                        _DeepColor.rgb,
                        _LightColor.rgb,
                        waterPattern
                    );

                // ñæÇÈÇ¢êÖãÿ
                float highlight =
                    smoothstep(
                        0.75,
                        1.0,
                        waterPattern
                    );

                color += highlight * 0.15;

                return fixed4(
                    color,
                    _Transparency
                );
            }

            ENDCG
        }
    }
}