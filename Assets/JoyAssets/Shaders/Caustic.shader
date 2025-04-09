// This Unity shader reconstructs the world space positions for pixels using a depth
// texture and screen space UV coordinates. The shader draws a checkerboard pattern
// on a mesh to visualize the positions.
Shader "Example/URPReconstructWorldPos"
{
    Properties
    {
        //half4x4 _MainLightDirection;

        _CausticsTexture("Caustics Texture", 2D) = "" {}
        _CausticsSpeed("Caustics Speed", Float) = 0.5
        _ShallowCausticsScale("Shallow Caustics Scale", Float) = 0.5
        _DeepCausticsScale("Deep Caustics Scale", Float) = 0.5
        _CausticsDepth("Caustics Depth", Float) = 0.5
        _CausticsStrength("Caustics Strength", Float) = 0.5
        _CausticsMaxStrength("Caustics Max Strength", Float) = 1.0
        _CausticsSplit("Caustics Split", Float) = 0.5
        _CausticsLuminanceMaskStrength("Caustics Luminance Mask Strength", Float) = 0.5
        _CausticsFadeFactor("Caustics Fade Factor", Float) = 0.5
        _ShallowColor("ShallowColor", Color) = (.25, .5, .5, 1)
        _ShallowColorDepth("Shallow Color Depth", Float) = 0.5
        _DeepColor("DeepColor", Color) = (.25, .5, .5, 1)
        _DeepColorDepth("Deep Color Depth", Float) = 0.5
    }

        // The SubShader block containing the Shader code.
            SubShader
        {
            // SubShader Tags define when and under which conditions a SubShader block or
            // a pass is executed.
            Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

            Pass
            {
                 Blend One One
                 ZWrite Off
                 Cull Front
                 ZTest Always
                HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The DeclareDepthTexture.hlsl file contains utilities for sampling the
            // Camera depth texture.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Debug.hlsl"

            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
            // The positionOS variable contains the vertex positions in object
            // space.
            float4 positionOS   : POSITION;
        };

        struct Varyings
        {
            // The positions in this struct must have the SV_POSITION semantic.
            float4 positionHCS  : SV_POSITION;
        };

        half4x4 _MainLightDirection;

        TEXTURE2D(_CausticsTexture);
        SAMPLER(sampler_CausticsTexture);

        float _CausticsSpeed;
        float _ShallowCausticsScale;
        float _DeepCausticsScale;
        float _CausticsDepth;
        float _CausticsStrength;
        float _CausticsMaxStrength;
        float _CausticsSplit;
        float _CausticsLuminanceMaskStrength;
        float _CausticsFadeFactor;
        float4 _ShallowColor;
        float _ShallowColorDepth;
        float4 _DeepColor;
        float _DeepColorDepth;

        float WaveHeight(float3 posWS)
        {
            float waveScale = .2f;
            float waveAmp1 = 2;
            float waveAmp2 = 1;
            float waveAmp3 = .5f;
            float waveFreq1 = .1f;
            float waveFreq2 = .15f;
            float waveFreq3 = .4f;
            float waveSpeed1 = 1;
            float waveSpeed2 = -1;
            float waveSpeed3 = 2;

            float heightX = waveAmp1 * sin(posWS.x * waveFreq1 + _Time.y * waveSpeed1)
                            + waveAmp2 * sin(posWS.x * waveFreq2 + _Time.y * waveSpeed2)
                            + waveAmp3 * sin(posWS.x * waveFreq3 + _Time.y * waveSpeed3);

            float heightZ = waveAmp1 * sin(posWS.z * waveFreq1 + _Time.y * waveSpeed1)
                            + waveAmp2 * sin(posWS.z * waveFreq2 + _Time.y * waveSpeed2)
                            + waveAmp3 * sin(posWS.z * waveFreq3 + _Time.y * waveSpeed3);
    
            return waveScale * (heightX + heightZ);
        }

        half2 Panner(half2 uv, half speed, half tiling)
        {
            return (half2(1, 0) * _Time.y * speed) + (uv * tiling);
        }

        half3 SampleCaustics(half2 uv, half split)
        {
            half2 uv1 = uv + half2(split, split);
            half2 uv2 = uv + half2(split, -split);
            half2 uv3 = uv + half2(-split, -split);

            half r = SAMPLE_TEXTURE2D(_CausticsTexture, sampler_CausticsTexture, uv1).r;
            half g = SAMPLE_TEXTURE2D(_CausticsTexture, sampler_CausticsTexture, uv2).r;
            half b = SAMPLE_TEXTURE2D(_CausticsTexture, sampler_CausticsTexture, uv3).r;

            return half3(r, g, b);
        }

        // The vertex shader definition with properties defined in the Varyings
        // structure. The type of the vert function must match the type (struct)
        // that it returns.
        Varyings vert(Attributes IN)
        {
            // Declaring the output object (OUT) with the Varyings struct.
            Varyings OUT;
            // The TransformObjectToHClip function transforms vertex positions
            // from object space to homogenous clip space.
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            // Returning the output.
            return OUT;
        }

        // The fragment shader definition.
        // The Varyings input structure contains interpolated values from the
        // vertex shader. The fragment shader uses the `positionHCS` property
        // from the `Varyings` struct to get locations of pixels.
        half4 frag(Varyings IN) : SV_Target
        {
            // To calculate the UV coordinates for sampling the depth buffer,
            // divide the pixel location by the render target resolution
            // _ScaledScreenParams.
            float2 positionNDC = IN.positionHCS.xy / _ScaledScreenParams.xy;

            // Sample the depth from the Camera depth texture.
            #if UNITY_REVERSED_Z
                real depth = SampleSceneDepth(positionNDC);
            #else
                // Adjust Z to match NDC for OpenGL ([-1, 1])
                real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(positionNDC));
            #endif

                // Set the color to black in the proximity to the far clipping
                // plane.
    #if UNITY_REVERSED_Z
        // Case for platforms with REVERSED_Z, such as D3D.
                if (depth < 0.0001)
                    return half4(0, 0, 0, 1);
    #else
        // Case for platforms without REVERSED_Z, such as OpenGL.
                if (depth > 0.9999)
                    return half4(0, 0, 0, 1);
    #endif

                // Reconstruct the world space positions.
                float3 positionWS = ComputeWorldSpacePosition(positionNDC, depth, UNITY_MATRIX_I_VP);

                // calculate position in object-space coordinates
                float3 positionOS = TransformWorldToObject(positionWS);

                // create bounding box mask
                float boundingBoxMask = all(step(positionOS, 0.5) * (1 - step(positionOS, -0.5)));

                // get the wave height and add that to Mask
                float waveHeight = WaveHeight(positionWS);
                float waterHeightMask = step(positionWS.y, waveHeight);
                waterHeightMask *= WaveHeight(positionWS) - positionWS.y;
                waterHeightMask = clamp(waterHeightMask, 0, 1);

                // calculate the water color gradient based on the depth of the water
                float4 waterColor = lerp(float4(.0f, .0f, .0f, .0f), _ShallowColor, (waveHeight - positionWS.y)/_ShallowColorDepth) * step(waveHeight - _ShallowColorDepth, positionWS.y);
                waterColor += lerp(_ShallowColor, _DeepColor, (waveHeight - positionWS.y - _ShallowColorDepth)/_DeepColorDepth) * step(positionWS.y, waveHeight - _ShallowColorDepth);
                waterColor = clamp(waterColor, .0f, 1.0f);
                waterColor *= boundingBoxMask;
                waterColor *= waterHeightMask;

                // calculate the vertical fade based on depth from the water surface
                float verticalFade = lerp(1.0f, 0.0f, (waveHeight - positionWS.y)/_DeepColorDepth) * step(waveHeight - _DeepColorDepth, positionWS.y);
                boundingBoxMask *= verticalFade;
                //waterColor *= verticalFade;
                
                // calculate the depth fade factor based on the object space and apply to the boundbox mask and the water color
                float fogDepth = _CausticsFadeFactor - length(positionOS);
                boundingBoxMask *= fogDepth;
                waterColor *= fogDepth;

                // calculate caustics texture UV coordinates (influenced by light direction)
                half2 uv_caustic = mul(positionWS, _MainLightDirection).xy;

                float causticsScale = lerp(_ShallowCausticsScale, _DeepCausticsScale, abs(0 - positionWS.y)/_CausticsDepth);
                causticsScale = _ShallowCausticsScale;

                half2 uv1 = Panner(uv_caustic, 0.75 * _CausticsSpeed, 1 / causticsScale);
                half2 uv2 = Panner(uv_caustic, 1 * _CausticsSpeed, -1 / causticsScale);

                // sample the caustics
                half3 tex1 = SampleCaustics(uv1, _CausticsSplit);
                half3 tex2 = SampleCaustics(uv2, _CausticsSplit);

                half3 caustics = clamp(0, _CausticsMaxStrength, min(tex1, tex2) * _CausticsStrength);

                half3 sceneColor = SampleSceneColor(positionNDC);
                half sceneLuminance = Luminance(sceneColor);
                half luminanceMask = lerp(1, sceneLuminance, _CausticsLuminanceMaskStrength);
                //half luminanceMask = smoothstep(_CausticsLuminanceMaskStrength, _CausticsLuminanceMaskStrength + 0.1, sceneLuminance);

                

                half4 color = half4((caustics.xyz) * boundingBoxMask * luminanceMask * waterHeightMask , length(caustics.xyz) * boundingBoxMask * luminanceMask * waterHeightMask);

                
                //float3 test = float3(_CausticsFadeFactor, _CausticsFadeFactor, _CausticsFadeFactor) - abs(positionOS);
                
                //color = float4(positionOS.x, positionOS.y, positionOS.z, 1);
                //sceneColor *= 20;
                //color = half4(sceneColor.x, sceneColor.y, sceneColor.z, .01);

                //color = caustics;
                return color;
            }
            ENDHLSL
        }
        }
}