Shader "Custom/CarveRoom"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _RoomTex ("Room Texture", 2D) = "black" {}
        _RoomScale ("Room Scale", Vector) = (1,1,0,0)
        _RoomOffset ("Room Offset", Vector) = (0,0,0,0)
        _Progress ("Progress", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        Cull Off
        ZTest Always
        Blend Off

        Pass
        {
            Name "CarveRoomPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_RoomTex);
            SAMPLER(sampler_RoomTex);

            float2 _RoomScale;
            float2 _RoomOffset;
            float _Progress;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings Vert (Attributes v)
            {
                Varyings o;

                // Fullscreen triangle
                o.positionCS = GetFullScreenTriangleVertexPosition(v.vertexID);
                o.uv = GetFullScreenTriangleTexCoord(v.vertexID);

                return o;
            }

            half4 Frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                // Sample main texture normally
                half4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // Apply offset + scale for room texture
                float2 roomUV = (uv + _RoomOffset) * _RoomScale;
                half4 roomCol = SAMPLE_TEXTURE2D(_RoomTex, sampler_RoomTex, roomUV);

                // Apply progress to room color
                roomCol = min(roomCol, _Progress);

                // Take max per-channel
                return max(mainCol, roomCol);
            }

            ENDHLSL
        }
    }
}