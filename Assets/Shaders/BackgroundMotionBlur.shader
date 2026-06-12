Shader "RunningGame/BackgroundMotionBlur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 0.02)) = 0.003
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float _BlurSize;

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 offset = float2(_BlurSize, 0);

                half4 color = 0;

                color += SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv - offset * 3
                ) * 0.08;

                color += SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv - offset * 2
                ) * 0.12;

                color += SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv - offset
                ) * 0.20;

                color += SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv
                ) * 0.20;

                color += SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv + offset
                ) * 0.20;

                color += SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv + offset * 2
                ) * 0.12;

                color += SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv + offset * 3
                ) * 0.08;

                return color * _Color * input.color;
            }

            ENDHLSL
        }
    }
}