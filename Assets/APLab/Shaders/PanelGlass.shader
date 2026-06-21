// A&P Lab — simple URP transparent unlit panel ("glass").
// Used for the large back panel behind the skull: a light-grey transparent middle backdrop
// and the two darker transparent side info panels. URP's built-in Unlit IGNORES alpha (a
// low-alpha colour renders opaque — that's the documented gotcha behind the bone-hover bug),
// so this tiny shader returns _Color verbatim, alpha included. Modeled on RayOverlay.shader
// but depth-tested (ZTest LEqual) so the opaque skull in front correctly occludes it.
Shader "APLab/PanelGlass"
{
    Properties
    {
        _Color ("Color", Color) = (0.8, 0.8, 0.85, 0.2)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }

        Pass
        {
            ZTest LEqual
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
    Fallback Off
}
