Shader "EnviroColorize"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _RampTex ("Ramp Texture", 2D) = "white" { }
        _HueOffset ("Hue Offset", Range(-2, 2)) = 0
        _HueInfluence ("Hue Influence", Range(0, 10)) = 3

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "Universal Forward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            // Properties
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_RampTex); SAMPLER(sampler_RampTex);
            float _HueOffset;
            float _HueInfluence;
            
            float Custom_ColorspaceConversion_Linear_RGB_float(float In)
            {
                float sRGBLo = In * 12.92;
                float sRGBHi = (pow(max(abs(In), 1.192092896e-07), float(1.0 / 2.4)) * 1.055) - 0.055;
                return float(In <= 0.0031308) ? sRGBLo : sRGBHi;
            }
            // Unity's hue implementation is frankly kind of shit
            float3 Modified_Hue_Degrees_float(float3 In, float Offset)
            {
	            Offset = -Offset / 180;
                float3x3 RGBtoYIQ = float3x3(
                    0.299,  0.587,  0.114,
                    0.596, -0.274, -0.322,
                    0.211, -0.523,  0.311);
    
                float3x3 YIQtoRGB = float3x3(
                    1.0,  0.956,  0.621,
                    1.0, -0.272, -0.647,
                    1.0, -1.106,  1.703);
    
                float3 yiq = mul(RGBtoYIQ, In);
                float angle = Offset * 3.14159265;
                float cosA = cos(angle);
                float sinA = sin(angle);
    
                float3x3 hueRotation = float3x3(
                    1,      0,       0,
                    0,  cosA,  -sinA,
                    0,  sinA,   cosA);
    
                yiq = mul(hueRotation, yiq);
                return mul(YIQtoRGB, yiq);
            }
            float GetHue(float3 c)
            {
                float maxC = max(c.r, max(c.g, c.b));
                float minC = min(c.r, min(c.g, c.b));
                float d = maxC - minC;

                float h = (c.g - c.b) / (d + 1e-10);
                h = lerp(h, (c.b - c.r) / (d + 1e-10) + 2.0, step(maxC, c.g));
                h = lerp(h, (c.r - c.g) / (d + 1e-10) + 4.0, step(maxC, c.b));

                return frac(h / 6.0);
            }


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                float4 mainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float luminance = Luminance(mainTexColor.rgb);
                luminance = Custom_ColorspaceConversion_Linear_RGB_float(luminance);
                float4 rampTexColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(luminance, 0.5));
                float hue = GetHue(mainTexColor.rgb);
                rampTexColor.rgb = Modified_Hue_Degrees_float(rampTexColor.rgb, 360 * (hue + _HueOffset) * _HueInfluence);
                return rampTexColor;
            }

            ENDHLSL
        }
    }
}