Shader "EnviroColorize"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _SampleTex ("Sample Texture", 2D) = "white" { }
        _MaskTex ("Mask Texture", 2D) = "white" { }
        _RampTex ("Ramp Texture", 2D) = "white" { }
        _HueOffset ("Hue Offset", Range(-2, 2)) = 0
        _HueInfluence ("Hue Influence", Range(0, 10)) = 3
        _MinColor ("Min Color", Color) = (0, 0, 0, 1)
        _MidColor ("Mid Color", Color) = (0.5, 0.5, 0.5, 1)
        _MaxColor ("Max Color", Color) = (1, 1, 1, 1)

        _AllowBeyondRange ("Allow Beyond Range", Float) = 1
        _BelowRangeMultiplier ("Below Range Multiplier", Float) = 1
        _AboveRangeMultiplier ("Above Range Multiplier", Float) = 1
        _BottomHalfMultiplier ("Adjust Luminance From Bottom", Range(0, 0.5)) = 0
        _TopHalfMultiplier ("Adjust Luminance From Top", Range(0, 0.5)) = 0

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
            TEXTURE2D(_SampleTex); SAMPLER(sampler_SampleTex);
            TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);
            TEXTURE2D(_RampTex); SAMPLER(sampler_RampTex);
            float _HueInfluence;
            float4 _MinColor;
            float4 _MidColor;
            float4 _MaxColor;

            float _AllowBeyondRange;
            float _BelowRangeMultiplier;
            float _AboveRangeMultiplier;
            float _BottomHalfMultiplier;
            float _TopHalfMultiplier;
            
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
                float4 sampleTexColor = SAMPLE_TEXTURE2D(_SampleTex, sampler_SampleTex, i.uv);
                
                float sampledLuminance = Custom_ColorspaceConversion_Linear_RGB_float(Luminance(sampleTexColor.rgb));
                float minLuminance = Custom_ColorspaceConversion_Linear_RGB_float(Luminance(_MinColor.rgb));
                float midLuminance = Custom_ColorspaceConversion_Linear_RGB_float(Luminance(_MidColor.rgb));
                float maxLuminance = Custom_ColorspaceConversion_Linear_RGB_float(Luminance(_MaxColor.rgb));

                float4 rampTexColor;
                float4 colorOnRange;

                // While we have the real 0-1 luminance, figure out what the expected baseline hue is in that range
                if (sampledLuminance <= midLuminance)
                {
                    if (_AllowBeyondRange > .5 && sampledLuminance < minLuminance)
                    {
                        colorOnRange = _MinColor;
                    }
                    else
                    {
                        float percent = saturate((sampledLuminance - minLuminance) / (midLuminance - minLuminance));
                        colorOnRange = lerp(_MinColor, _MidColor, percent);
                    }
                }
                else
                {
                    if (_AllowBeyondRange > .5 && sampledLuminance > maxLuminance)
                    {
                        colorOnRange = _MaxColor;
                    }
                    else
                    {
                        float percent = saturate((sampledLuminance - midLuminance) / (maxLuminance - midLuminance));
                        colorOnRange = lerp(_MidColor, _MaxColor, percent);
                    }
                }

                // Get ramp point by sampling luminance and seeing where it falls between the defined points
                // - A percent of 0 represents the min or max color
                // - A percent of 1 represents the mid color
                // - A percent of -1 represents black or white
                if (sampledLuminance <= midLuminance)
                {
                    float percent = (sampledLuminance - minLuminance) / (midLuminance - minLuminance);
                    percent = 1 - (1- percent) * _BottomHalfMultiplier;
                    if (_AllowBeyondRange > .5 && percent < 0)
                    {
                        percent = percent * _BelowRangeMultiplier;
                        rampTexColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(0, 0.5));
                        rampTexColor = lerp(rampTexColor, float4(0, 0, 0, _MinColor.a), abs(percent));
                    }
                    else
                    {
                        float normalizedLuminance = 0.5 * percent;
                        rampTexColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(normalizedLuminance, 0.5));
                    }
                }
                else
                {
                    float percent = (maxLuminance - sampledLuminance) / (maxLuminance - midLuminance);
                    percent = 1 - (1- percent) * _TopHalfMultiplier;
                    if (_AllowBeyondRange > .5 && percent < 0)
                    {
                        percent = percent * _AboveRangeMultiplier;
                        rampTexColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(1, 0.5));
                        rampTexColor = lerp(rampTexColor, float4(1, 1, 1, _MaxColor.a), abs(percent));
                    }
                    else
                    {
                        float normalizedLuminance = 0.5 + 0.5 * (1 - percent);
                        rampTexColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(normalizedLuminance, 0.5));
                    }
                }
                
                // Adjust the hue based on the original hue shift
                float sampledHue = GetHue(sampleTexColor.rgb);
                float baselineHue = GetHue(colorOnRange.rgb);
                rampTexColor.rgb = Modified_Hue_Degrees_float(rampTexColor.rgb, 360 * (sampledHue - baselineHue) * _HueInfluence);
                
                // Apply the mask alpha
                float4 maskTexColor = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                rampTexColor.a = sampleTexColor.a * maskTexColor.a;

                // Mix with input color
                float4 inputColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 outColor;
                outColor.rgb = lerp(inputColor.rgb, rampTexColor.rgb, rampTexColor.a);
                outColor.a = max(inputColor.a, rampTexColor.a);
                return outColor;
            }

            ENDHLSL
        }
    }
}