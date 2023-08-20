Shader "TrailNoSelfOverLap"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        [HDR]_MainColor("MainColor", Color) = (2, 0, 0, 1)
        [HDR]_OutterFlameColor("OutterFlameColor", Color) = (1, 1, 0, 1)
        [HDR]_DotColor("DotColor", Color) = (0, 0, 0, 1)
        _DotSpeed("DotSpeed", Range(0.1, 10)) = 2
        _DotSpeedSecond("DotSpeedSecond", Range(0, 10)) = 5
        _SideCutOut("SideCutOut", Range(0, 10)) = 1
        _TopCutOut("TopCutOut", Range(0, 10)) = 1
        _GrowSpeed("GrowSpeed", Range(1, 50)) = 5
        _Size("Size", Range(1, 10)) = 10
        _Density("Density", Range(0.1, 5)) = 4
        _FlickAmount("FlickAmount", Range(0, 1)) = 0.5
        _BigDotSize("BigDotSize", Range(1, 24)) = 20
        _SmallDotSize("SmallDotSize", Range(1, 24)) = 1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"=""
        }
        Pass
        {
            Name "Sprite Lit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest Less
        ZWrite On
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define VARYINGS_NEED_SCREENPOSITION
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITELIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
             float4 screenPosition;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            output.interp3.xyzw =  input.screenPosition;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            output.screenPosition = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainColor;
        float _SideCutOut;
        float _TopCutOut;
        float _GrowSpeed;
        float _Size;
        float4 _MainTex_TexelSize;
        float4 _OutterFlameColor;
        float _Density;
        float4 _DotColor;
        float _DotSpeed;
        float _DotSpeedSecond;
        float _FlickAmount;
        float _BigDotSize;
        float _SmallDotSize;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        
        inline float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }
        
        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
        
            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
        
                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_DegreesToRadians_float(float In, out float Out)
        {
            Out = radians(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        
        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        {
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float4 SpriteMask;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_884635e6c9294e3aaee2f5d0a6539855_Out_0 = IsGammaSpace() ? LinearToSRGB(_OutterFlameColor) : _OutterFlameColor;
            float4 _Property_aa3eee1c710e48ff833377cae2890b24_Out_0 = IsGammaSpace() ? LinearToSRGB(_MainColor) : _MainColor;
            float _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1);
            float _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2;
            Unity_Multiply_float_float(_OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1, 2, _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2);
            float2 _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0 = float2(_Multiply_3be1ea122189430eac6435db8df19cdf_Out_2, IN.TimeParameters.y);
            float2 _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0, _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3);
            float _Property_f513090750e84d40a8d84766cc36e62e_Out_0 = _Density;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3, 2, _Property_f513090750e84d40a8d84766cc36e62e_Out_0, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4);
            float _Power_876802c01b1e435091f1643e714a33a4_Out_2;
            Unity_Power_float(_Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, 2, _Power_876802c01b1e435091f1643e714a33a4_Out_2);
            float _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2;
            Unity_Multiply_float_float(_Power_876802c01b1e435091f1643e714a33a4_Out_2, 5, _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2);
            float4 _UV_d330e0a791714618987503aefda68ccc_Out_0 = IN.uv0;
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_R_1 = _UV_d330e0a791714618987503aefda68ccc_Out_0[0];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_G_2 = _UV_d330e0a791714618987503aefda68ccc_Out_0[1];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_B_3 = _UV_d330e0a791714618987503aefda68ccc_Out_0[2];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_A_4 = _UV_d330e0a791714618987503aefda68ccc_Out_0[3];
            float _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1;
            Unity_OneMinus_float(_Split_cbe6e4ce039641ea8d3914edacad64a4_G_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1);
            float _Add_900201d365624a198e9ba914f8935c89_Out_2;
            Unity_Add_float(_Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1, _Add_900201d365624a198e9ba914f8935c89_Out_2);
            float _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1;
            Unity_DegreesToRadians_float(180, _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1);
            float4 _UV_0ae088b93f05405c89adec82b7cec810_Out_0 = IN.uv0;
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[0];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[1];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_B_3 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[2];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_A_4 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[3];
            float _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2;
            Unity_Multiply_float_float(_DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1, _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2, _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2);
            float _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1;
            Unity_Sine_float(_Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2, _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1);
            float _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0 = _SideCutOut;
            float _Power_e73d214d36974172897d0ac7ad68776c_Out_2;
            Unity_Power_float(_Sine_ad36353488c74a378d89f6e3ae290a09_Out_1, _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0, _Power_e73d214d36974172897d0ac7ad68776c_Out_2);
            float _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_e73d214d36974172897d0ac7ad68776c_Out_2, _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3);
            float _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1;
            Unity_OneMinus_float(_Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1, _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1);
            float _Property_94007eaee9d94c8aa4e15139e7945492_Out_0 = _TopCutOut;
            float _Power_435d91e9236b45828f54670136bf12aa_Out_2;
            Unity_Power_float(_OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1, _Property_94007eaee9d94c8aa4e15139e7945492_Out_0, _Power_435d91e9236b45828f54670136bf12aa_Out_2);
            float _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_435d91e9236b45828f54670136bf12aa_Out_2, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3);
            float _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2;
            Unity_Multiply_float_float(_Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2);
            float _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2;
            Unity_Multiply_float_float(_Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, 1.5, _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2);
            float _Property_b79c3f484a9a4a76ad32b053aee56812_Out_0 = _Size;
            float _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1;
            Unity_OneMinus_float(_Property_b79c3f484a9a4a76ad32b053aee56812_Out_0, _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1);
            float _Add_44067c9172b6430299f6498ef84b1fdc_Out_2;
            Unity_Add_float(_OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1, 10, _Add_44067c9172b6430299f6498ef84b1fdc_Out_2);
            float _Property_9d8702d4ad38405dbe31af92628115db_Out_0 = _GrowSpeed;
            float _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_9d8702d4ad38405dbe31af92628115db_Out_0, _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2);
            float _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1;
            Unity_Sine_float(_Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2, _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1);
            float _Add_fde914a920eb4e508b0b79404d5627ae_Out_2;
            Unity_Add_float(_Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1, 2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2);
            float _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2;
            Unity_Multiply_float_float(_Add_44067c9172b6430299f6498ef84b1fdc_Out_2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2);
            float _Property_85a315d160c0400dba8557ebb894c35a_Out_0 = _FlickAmount;
            float _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3;
            Unity_Lerp_float(1, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2, _Property_85a315d160c0400dba8557ebb894c35a_Out_0, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3);
            float _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2;
            Unity_Power_float(_Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2);
            float _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2;
            Unity_Multiply_float_float(_Add_900201d365624a198e9ba914f8935c89_Out_2, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2, _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2);
            float _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3;
            Unity_Clamp_float(_Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2, 0, 1, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3);
            float _Power_eb5a01505c9546ffb884a2626df23f13_Out_2;
            Unity_Power_float(_Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, 5, _Power_eb5a01505c9546ffb884a2626df23f13_Out_2);
            float4 _Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3;
            Unity_Lerp_float4(_Property_884635e6c9294e3aaee2f5d0a6539855_Out_0, _Property_aa3eee1c710e48ff833377cae2890b24_Out_0, (_Power_eb5a01505c9546ffb884a2626df23f13_Out_2.xxxx), _Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3);
            float4 _Property_bf625041a2254b3c800d4003b885fbaf_Out_0 = IsGammaSpace() ? LinearToSRGB(_DotColor) : _DotColor;
            float _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1);
            float2 _Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.y);
            float _Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0 = _DotSpeed;
            float2 _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2;
            Unity_Multiply_float2_float2(_Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0, (_Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0.xx), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2);
            float2 _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2, _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3);
            float _Property_3308b7072f014457832f27b76be59cdd_Out_0 = _BigDotSize;
            float _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2;
            Unity_Multiply_float_float(_Property_3308b7072f014457832f27b76be59cdd_Out_0, -1, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2);
            float _Add_d9113240b1b54159902e1e83c00ad97f_Out_2;
            Unity_Add_float(25, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2);
            float _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2);
            float _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2, _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3);
            float _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2;
            Unity_Multiply_float_float(_Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3, 5, _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2);
            float2 _Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.z);
            float _Property_3efe4630801e4184acf1494a309782c6_Out_0 = _DotSpeedSecond;
            float2 _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2;
            Unity_Multiply_float2_float2(_Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0, (_Property_3efe4630801e4184acf1494a309782c6_Out_0.xx), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2);
            float2 _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2, _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3);
            float _Property_4dfc2acea8434c739af90ae838caf34f_Out_0 = _SmallDotSize;
            float _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2;
            Unity_Multiply_float_float(_Property_4dfc2acea8434c739af90ae838caf34f_Out_0, -1, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2);
            float _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2;
            Unity_Add_float(25, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2);
            float _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2);
            float _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2, _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3);
            float _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2;
            Unity_Multiply_float_float(_Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3, 5, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2);
            float _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2;
            Unity_Add_float(_Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2, _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2);
            float _Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3;
            Unity_Clamp_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, 0, 1, _Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3);
            float4 _Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3;
            Unity_Lerp_float4(_Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3, _Property_bf625041a2254b3c800d4003b885fbaf_Out_0, (_Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3.xxxx), _Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3);
            float _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2;
            Unity_Multiply_float_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2);
            float _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3;
            Unity_Clamp_float(_Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2, 0, 1, _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3);
            float _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2;
            Unity_Add_float(_Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2);
            float _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            Unity_Clamp_float(_Add_68ea4332311a4ea8b2c7966894897e3e_Out_2, 0, 1, _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3);
            surface.BaseColor = (_Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3.xyz);
            surface.Alpha = _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            surface.SpriteMask = IsGammaSpace() ? float4(1, 1, 1, 1) : float4 (SRGBToLinear(float3(1, 1, 1)), 1);
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteLitPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Normal"
            Tags
            {
                "LightMode" = "NormalsRendering"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest Less
        ZWrite On
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITENORMAL
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            output.interp2.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            output.texCoord0 = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainColor;
        float _SideCutOut;
        float _TopCutOut;
        float _GrowSpeed;
        float _Size;
        float4 _MainTex_TexelSize;
        float4 _OutterFlameColor;
        float _Density;
        float4 _DotColor;
        float _DotSpeed;
        float _DotSpeedSecond;
        float _FlickAmount;
        float _BigDotSize;
        float _SmallDotSize;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        
        inline float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }
        
        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
        
            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
        
                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_DegreesToRadians_float(float In, out float Out)
        {
            Out = radians(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        
        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        {
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 NormalTS;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_884635e6c9294e3aaee2f5d0a6539855_Out_0 = IsGammaSpace() ? LinearToSRGB(_OutterFlameColor) : _OutterFlameColor;
            float4 _Property_aa3eee1c710e48ff833377cae2890b24_Out_0 = IsGammaSpace() ? LinearToSRGB(_MainColor) : _MainColor;
            float _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1);
            float _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2;
            Unity_Multiply_float_float(_OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1, 2, _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2);
            float2 _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0 = float2(_Multiply_3be1ea122189430eac6435db8df19cdf_Out_2, IN.TimeParameters.y);
            float2 _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0, _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3);
            float _Property_f513090750e84d40a8d84766cc36e62e_Out_0 = _Density;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3, 2, _Property_f513090750e84d40a8d84766cc36e62e_Out_0, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4);
            float _Power_876802c01b1e435091f1643e714a33a4_Out_2;
            Unity_Power_float(_Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, 2, _Power_876802c01b1e435091f1643e714a33a4_Out_2);
            float _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2;
            Unity_Multiply_float_float(_Power_876802c01b1e435091f1643e714a33a4_Out_2, 5, _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2);
            float4 _UV_d330e0a791714618987503aefda68ccc_Out_0 = IN.uv0;
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_R_1 = _UV_d330e0a791714618987503aefda68ccc_Out_0[0];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_G_2 = _UV_d330e0a791714618987503aefda68ccc_Out_0[1];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_B_3 = _UV_d330e0a791714618987503aefda68ccc_Out_0[2];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_A_4 = _UV_d330e0a791714618987503aefda68ccc_Out_0[3];
            float _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1;
            Unity_OneMinus_float(_Split_cbe6e4ce039641ea8d3914edacad64a4_G_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1);
            float _Add_900201d365624a198e9ba914f8935c89_Out_2;
            Unity_Add_float(_Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1, _Add_900201d365624a198e9ba914f8935c89_Out_2);
            float _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1;
            Unity_DegreesToRadians_float(180, _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1);
            float4 _UV_0ae088b93f05405c89adec82b7cec810_Out_0 = IN.uv0;
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[0];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[1];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_B_3 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[2];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_A_4 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[3];
            float _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2;
            Unity_Multiply_float_float(_DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1, _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2, _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2);
            float _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1;
            Unity_Sine_float(_Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2, _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1);
            float _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0 = _SideCutOut;
            float _Power_e73d214d36974172897d0ac7ad68776c_Out_2;
            Unity_Power_float(_Sine_ad36353488c74a378d89f6e3ae290a09_Out_1, _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0, _Power_e73d214d36974172897d0ac7ad68776c_Out_2);
            float _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_e73d214d36974172897d0ac7ad68776c_Out_2, _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3);
            float _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1;
            Unity_OneMinus_float(_Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1, _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1);
            float _Property_94007eaee9d94c8aa4e15139e7945492_Out_0 = _TopCutOut;
            float _Power_435d91e9236b45828f54670136bf12aa_Out_2;
            Unity_Power_float(_OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1, _Property_94007eaee9d94c8aa4e15139e7945492_Out_0, _Power_435d91e9236b45828f54670136bf12aa_Out_2);
            float _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_435d91e9236b45828f54670136bf12aa_Out_2, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3);
            float _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2;
            Unity_Multiply_float_float(_Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2);
            float _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2;
            Unity_Multiply_float_float(_Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, 1.5, _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2);
            float _Property_b79c3f484a9a4a76ad32b053aee56812_Out_0 = _Size;
            float _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1;
            Unity_OneMinus_float(_Property_b79c3f484a9a4a76ad32b053aee56812_Out_0, _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1);
            float _Add_44067c9172b6430299f6498ef84b1fdc_Out_2;
            Unity_Add_float(_OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1, 10, _Add_44067c9172b6430299f6498ef84b1fdc_Out_2);
            float _Property_9d8702d4ad38405dbe31af92628115db_Out_0 = _GrowSpeed;
            float _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_9d8702d4ad38405dbe31af92628115db_Out_0, _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2);
            float _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1;
            Unity_Sine_float(_Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2, _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1);
            float _Add_fde914a920eb4e508b0b79404d5627ae_Out_2;
            Unity_Add_float(_Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1, 2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2);
            float _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2;
            Unity_Multiply_float_float(_Add_44067c9172b6430299f6498ef84b1fdc_Out_2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2);
            float _Property_85a315d160c0400dba8557ebb894c35a_Out_0 = _FlickAmount;
            float _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3;
            Unity_Lerp_float(1, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2, _Property_85a315d160c0400dba8557ebb894c35a_Out_0, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3);
            float _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2;
            Unity_Power_float(_Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2);
            float _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2;
            Unity_Multiply_float_float(_Add_900201d365624a198e9ba914f8935c89_Out_2, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2, _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2);
            float _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3;
            Unity_Clamp_float(_Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2, 0, 1, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3);
            float _Power_eb5a01505c9546ffb884a2626df23f13_Out_2;
            Unity_Power_float(_Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, 5, _Power_eb5a01505c9546ffb884a2626df23f13_Out_2);
            float4 _Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3;
            Unity_Lerp_float4(_Property_884635e6c9294e3aaee2f5d0a6539855_Out_0, _Property_aa3eee1c710e48ff833377cae2890b24_Out_0, (_Power_eb5a01505c9546ffb884a2626df23f13_Out_2.xxxx), _Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3);
            float4 _Property_bf625041a2254b3c800d4003b885fbaf_Out_0 = IsGammaSpace() ? LinearToSRGB(_DotColor) : _DotColor;
            float _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1);
            float2 _Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.y);
            float _Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0 = _DotSpeed;
            float2 _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2;
            Unity_Multiply_float2_float2(_Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0, (_Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0.xx), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2);
            float2 _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2, _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3);
            float _Property_3308b7072f014457832f27b76be59cdd_Out_0 = _BigDotSize;
            float _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2;
            Unity_Multiply_float_float(_Property_3308b7072f014457832f27b76be59cdd_Out_0, -1, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2);
            float _Add_d9113240b1b54159902e1e83c00ad97f_Out_2;
            Unity_Add_float(25, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2);
            float _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2);
            float _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2, _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3);
            float _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2;
            Unity_Multiply_float_float(_Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3, 5, _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2);
            float2 _Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.z);
            float _Property_3efe4630801e4184acf1494a309782c6_Out_0 = _DotSpeedSecond;
            float2 _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2;
            Unity_Multiply_float2_float2(_Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0, (_Property_3efe4630801e4184acf1494a309782c6_Out_0.xx), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2);
            float2 _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2, _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3);
            float _Property_4dfc2acea8434c739af90ae838caf34f_Out_0 = _SmallDotSize;
            float _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2;
            Unity_Multiply_float_float(_Property_4dfc2acea8434c739af90ae838caf34f_Out_0, -1, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2);
            float _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2;
            Unity_Add_float(25, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2);
            float _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2);
            float _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2, _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3);
            float _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2;
            Unity_Multiply_float_float(_Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3, 5, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2);
            float _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2;
            Unity_Add_float(_Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2, _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2);
            float _Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3;
            Unity_Clamp_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, 0, 1, _Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3);
            float4 _Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3;
            Unity_Lerp_float4(_Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3, _Property_bf625041a2254b3c800d4003b885fbaf_Out_0, (_Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3.xxxx), _Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3);
            float _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2;
            Unity_Multiply_float_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2);
            float _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3;
            Unity_Clamp_float(_Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2, 0, 1, _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3);
            float _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2;
            Unity_Add_float(_Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2);
            float _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            Unity_Clamp_float(_Add_68ea4332311a4ea8b2c7966894897e3e_Out_2, 0, 1, _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3);
            surface.BaseColor = (_Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3.xyz);
            surface.Alpha = _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            surface.NormalTS = IN.TangentSpaceNormal;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
            output.TangentSpaceNormal =                         float3(0.0f, 0.0f, 1.0f);
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteNormalPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
            // Render State
            Cull Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainColor;
        float _SideCutOut;
        float _TopCutOut;
        float _GrowSpeed;
        float _Size;
        float4 _MainTex_TexelSize;
        float4 _OutterFlameColor;
        float _Density;
        float4 _DotColor;
        float _DotSpeed;
        float _DotSpeedSecond;
        float _FlickAmount;
        float _BigDotSize;
        float _SmallDotSize;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        
        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        {
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DegreesToRadians_float(float In, out float Out)
        {
            Out = radians(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        
        inline float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }
        
        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
        
            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
        
                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1);
            float2 _Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.y);
            float _Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0 = _DotSpeed;
            float2 _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2;
            Unity_Multiply_float2_float2(_Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0, (_Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0.xx), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2);
            float2 _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2, _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3);
            float _Property_3308b7072f014457832f27b76be59cdd_Out_0 = _BigDotSize;
            float _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2;
            Unity_Multiply_float_float(_Property_3308b7072f014457832f27b76be59cdd_Out_0, -1, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2);
            float _Add_d9113240b1b54159902e1e83c00ad97f_Out_2;
            Unity_Add_float(25, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2);
            float _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2);
            float _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2, _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3);
            float _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2;
            Unity_Multiply_float_float(_Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3, 5, _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2);
            float2 _Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.z);
            float _Property_3efe4630801e4184acf1494a309782c6_Out_0 = _DotSpeedSecond;
            float2 _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2;
            Unity_Multiply_float2_float2(_Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0, (_Property_3efe4630801e4184acf1494a309782c6_Out_0.xx), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2);
            float2 _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2, _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3);
            float _Property_4dfc2acea8434c739af90ae838caf34f_Out_0 = _SmallDotSize;
            float _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2;
            Unity_Multiply_float_float(_Property_4dfc2acea8434c739af90ae838caf34f_Out_0, -1, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2);
            float _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2;
            Unity_Add_float(25, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2);
            float _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2);
            float _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2, _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3);
            float _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2;
            Unity_Multiply_float_float(_Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3, 5, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2);
            float _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2;
            Unity_Add_float(_Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2, _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2);
            float _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1;
            Unity_DegreesToRadians_float(180, _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1);
            float4 _UV_0ae088b93f05405c89adec82b7cec810_Out_0 = IN.uv0;
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[0];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[1];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_B_3 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[2];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_A_4 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[3];
            float _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2;
            Unity_Multiply_float_float(_DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1, _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2, _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2);
            float _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1;
            Unity_Sine_float(_Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2, _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1);
            float _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0 = _SideCutOut;
            float _Power_e73d214d36974172897d0ac7ad68776c_Out_2;
            Unity_Power_float(_Sine_ad36353488c74a378d89f6e3ae290a09_Out_1, _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0, _Power_e73d214d36974172897d0ac7ad68776c_Out_2);
            float _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_e73d214d36974172897d0ac7ad68776c_Out_2, _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3);
            float _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1;
            Unity_OneMinus_float(_Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1, _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1);
            float _Property_94007eaee9d94c8aa4e15139e7945492_Out_0 = _TopCutOut;
            float _Power_435d91e9236b45828f54670136bf12aa_Out_2;
            Unity_Power_float(_OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1, _Property_94007eaee9d94c8aa4e15139e7945492_Out_0, _Power_435d91e9236b45828f54670136bf12aa_Out_2);
            float _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_435d91e9236b45828f54670136bf12aa_Out_2, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3);
            float _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2;
            Unity_Multiply_float_float(_Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2);
            float _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2;
            Unity_Multiply_float_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2);
            float _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3;
            Unity_Clamp_float(_Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2, 0, 1, _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3);
            float _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1);
            float _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2;
            Unity_Multiply_float_float(_OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1, 2, _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2);
            float2 _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0 = float2(_Multiply_3be1ea122189430eac6435db8df19cdf_Out_2, IN.TimeParameters.y);
            float2 _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0, _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3);
            float _Property_f513090750e84d40a8d84766cc36e62e_Out_0 = _Density;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3, 2, _Property_f513090750e84d40a8d84766cc36e62e_Out_0, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4);
            float _Power_876802c01b1e435091f1643e714a33a4_Out_2;
            Unity_Power_float(_Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, 2, _Power_876802c01b1e435091f1643e714a33a4_Out_2);
            float _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2;
            Unity_Multiply_float_float(_Power_876802c01b1e435091f1643e714a33a4_Out_2, 5, _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2);
            float4 _UV_d330e0a791714618987503aefda68ccc_Out_0 = IN.uv0;
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_R_1 = _UV_d330e0a791714618987503aefda68ccc_Out_0[0];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_G_2 = _UV_d330e0a791714618987503aefda68ccc_Out_0[1];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_B_3 = _UV_d330e0a791714618987503aefda68ccc_Out_0[2];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_A_4 = _UV_d330e0a791714618987503aefda68ccc_Out_0[3];
            float _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1;
            Unity_OneMinus_float(_Split_cbe6e4ce039641ea8d3914edacad64a4_G_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1);
            float _Add_900201d365624a198e9ba914f8935c89_Out_2;
            Unity_Add_float(_Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1, _Add_900201d365624a198e9ba914f8935c89_Out_2);
            float _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2;
            Unity_Multiply_float_float(_Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, 1.5, _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2);
            float _Property_b79c3f484a9a4a76ad32b053aee56812_Out_0 = _Size;
            float _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1;
            Unity_OneMinus_float(_Property_b79c3f484a9a4a76ad32b053aee56812_Out_0, _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1);
            float _Add_44067c9172b6430299f6498ef84b1fdc_Out_2;
            Unity_Add_float(_OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1, 10, _Add_44067c9172b6430299f6498ef84b1fdc_Out_2);
            float _Property_9d8702d4ad38405dbe31af92628115db_Out_0 = _GrowSpeed;
            float _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_9d8702d4ad38405dbe31af92628115db_Out_0, _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2);
            float _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1;
            Unity_Sine_float(_Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2, _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1);
            float _Add_fde914a920eb4e508b0b79404d5627ae_Out_2;
            Unity_Add_float(_Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1, 2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2);
            float _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2;
            Unity_Multiply_float_float(_Add_44067c9172b6430299f6498ef84b1fdc_Out_2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2);
            float _Property_85a315d160c0400dba8557ebb894c35a_Out_0 = _FlickAmount;
            float _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3;
            Unity_Lerp_float(1, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2, _Property_85a315d160c0400dba8557ebb894c35a_Out_0, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3);
            float _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2;
            Unity_Power_float(_Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2);
            float _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2;
            Unity_Multiply_float_float(_Add_900201d365624a198e9ba914f8935c89_Out_2, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2, _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2);
            float _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3;
            Unity_Clamp_float(_Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2, 0, 1, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3);
            float _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2;
            Unity_Add_float(_Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2);
            float _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            Unity_Clamp_float(_Add_68ea4332311a4ea8b2c7966894897e3e_Out_2, 0, 1, _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3);
            surface.Alpha = _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
            // Render State
            Cull Back
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainColor;
        float _SideCutOut;
        float _TopCutOut;
        float _GrowSpeed;
        float _Size;
        float4 _MainTex_TexelSize;
        float4 _OutterFlameColor;
        float _Density;
        float4 _DotColor;
        float _DotSpeed;
        float _DotSpeedSecond;
        float _FlickAmount;
        float _BigDotSize;
        float _SmallDotSize;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        
        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        {
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DegreesToRadians_float(float In, out float Out)
        {
            Out = radians(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        
        inline float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }
        
        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
        
            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
        
                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1);
            float2 _Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.y);
            float _Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0 = _DotSpeed;
            float2 _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2;
            Unity_Multiply_float2_float2(_Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0, (_Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0.xx), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2);
            float2 _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2, _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3);
            float _Property_3308b7072f014457832f27b76be59cdd_Out_0 = _BigDotSize;
            float _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2;
            Unity_Multiply_float_float(_Property_3308b7072f014457832f27b76be59cdd_Out_0, -1, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2);
            float _Add_d9113240b1b54159902e1e83c00ad97f_Out_2;
            Unity_Add_float(25, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2);
            float _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2);
            float _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2, _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3);
            float _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2;
            Unity_Multiply_float_float(_Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3, 5, _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2);
            float2 _Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.z);
            float _Property_3efe4630801e4184acf1494a309782c6_Out_0 = _DotSpeedSecond;
            float2 _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2;
            Unity_Multiply_float2_float2(_Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0, (_Property_3efe4630801e4184acf1494a309782c6_Out_0.xx), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2);
            float2 _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2, _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3);
            float _Property_4dfc2acea8434c739af90ae838caf34f_Out_0 = _SmallDotSize;
            float _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2;
            Unity_Multiply_float_float(_Property_4dfc2acea8434c739af90ae838caf34f_Out_0, -1, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2);
            float _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2;
            Unity_Add_float(25, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2);
            float _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2);
            float _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2, _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3);
            float _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2;
            Unity_Multiply_float_float(_Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3, 5, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2);
            float _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2;
            Unity_Add_float(_Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2, _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2);
            float _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1;
            Unity_DegreesToRadians_float(180, _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1);
            float4 _UV_0ae088b93f05405c89adec82b7cec810_Out_0 = IN.uv0;
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[0];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[1];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_B_3 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[2];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_A_4 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[3];
            float _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2;
            Unity_Multiply_float_float(_DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1, _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2, _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2);
            float _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1;
            Unity_Sine_float(_Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2, _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1);
            float _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0 = _SideCutOut;
            float _Power_e73d214d36974172897d0ac7ad68776c_Out_2;
            Unity_Power_float(_Sine_ad36353488c74a378d89f6e3ae290a09_Out_1, _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0, _Power_e73d214d36974172897d0ac7ad68776c_Out_2);
            float _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_e73d214d36974172897d0ac7ad68776c_Out_2, _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3);
            float _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1;
            Unity_OneMinus_float(_Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1, _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1);
            float _Property_94007eaee9d94c8aa4e15139e7945492_Out_0 = _TopCutOut;
            float _Power_435d91e9236b45828f54670136bf12aa_Out_2;
            Unity_Power_float(_OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1, _Property_94007eaee9d94c8aa4e15139e7945492_Out_0, _Power_435d91e9236b45828f54670136bf12aa_Out_2);
            float _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_435d91e9236b45828f54670136bf12aa_Out_2, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3);
            float _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2;
            Unity_Multiply_float_float(_Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2);
            float _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2;
            Unity_Multiply_float_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2);
            float _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3;
            Unity_Clamp_float(_Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2, 0, 1, _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3);
            float _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1);
            float _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2;
            Unity_Multiply_float_float(_OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1, 2, _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2);
            float2 _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0 = float2(_Multiply_3be1ea122189430eac6435db8df19cdf_Out_2, IN.TimeParameters.y);
            float2 _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0, _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3);
            float _Property_f513090750e84d40a8d84766cc36e62e_Out_0 = _Density;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3, 2, _Property_f513090750e84d40a8d84766cc36e62e_Out_0, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4);
            float _Power_876802c01b1e435091f1643e714a33a4_Out_2;
            Unity_Power_float(_Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, 2, _Power_876802c01b1e435091f1643e714a33a4_Out_2);
            float _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2;
            Unity_Multiply_float_float(_Power_876802c01b1e435091f1643e714a33a4_Out_2, 5, _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2);
            float4 _UV_d330e0a791714618987503aefda68ccc_Out_0 = IN.uv0;
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_R_1 = _UV_d330e0a791714618987503aefda68ccc_Out_0[0];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_G_2 = _UV_d330e0a791714618987503aefda68ccc_Out_0[1];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_B_3 = _UV_d330e0a791714618987503aefda68ccc_Out_0[2];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_A_4 = _UV_d330e0a791714618987503aefda68ccc_Out_0[3];
            float _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1;
            Unity_OneMinus_float(_Split_cbe6e4ce039641ea8d3914edacad64a4_G_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1);
            float _Add_900201d365624a198e9ba914f8935c89_Out_2;
            Unity_Add_float(_Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1, _Add_900201d365624a198e9ba914f8935c89_Out_2);
            float _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2;
            Unity_Multiply_float_float(_Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, 1.5, _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2);
            float _Property_b79c3f484a9a4a76ad32b053aee56812_Out_0 = _Size;
            float _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1;
            Unity_OneMinus_float(_Property_b79c3f484a9a4a76ad32b053aee56812_Out_0, _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1);
            float _Add_44067c9172b6430299f6498ef84b1fdc_Out_2;
            Unity_Add_float(_OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1, 10, _Add_44067c9172b6430299f6498ef84b1fdc_Out_2);
            float _Property_9d8702d4ad38405dbe31af92628115db_Out_0 = _GrowSpeed;
            float _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_9d8702d4ad38405dbe31af92628115db_Out_0, _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2);
            float _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1;
            Unity_Sine_float(_Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2, _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1);
            float _Add_fde914a920eb4e508b0b79404d5627ae_Out_2;
            Unity_Add_float(_Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1, 2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2);
            float _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2;
            Unity_Multiply_float_float(_Add_44067c9172b6430299f6498ef84b1fdc_Out_2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2);
            float _Property_85a315d160c0400dba8557ebb894c35a_Out_0 = _FlickAmount;
            float _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3;
            Unity_Lerp_float(1, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2, _Property_85a315d160c0400dba8557ebb894c35a_Out_0, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3);
            float _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2;
            Unity_Power_float(_Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2);
            float _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2;
            Unity_Multiply_float_float(_Add_900201d365624a198e9ba914f8935c89_Out_2, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2, _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2);
            float _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3;
            Unity_Clamp_float(_Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2, 0, 1, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3);
            float _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2;
            Unity_Add_float(_Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2);
            float _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            Unity_Clamp_float(_Add_68ea4332311a4ea8b2c7966894897e3e_Out_2, 0, 1, _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3);
            surface.Alpha = _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest Less
        ZWrite On
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainColor;
        float _SideCutOut;
        float _TopCutOut;
        float _GrowSpeed;
        float _Size;
        float4 _MainTex_TexelSize;
        float4 _OutterFlameColor;
        float _Density;
        float4 _DotColor;
        float _DotSpeed;
        float _DotSpeedSecond;
        float _FlickAmount;
        float _BigDotSize;
        float _SmallDotSize;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        
        inline float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }
        
        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
        
            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
        
                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_DegreesToRadians_float(float In, out float Out)
        {
            Out = radians(In);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        
        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        {
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 NormalTS;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_884635e6c9294e3aaee2f5d0a6539855_Out_0 = IsGammaSpace() ? LinearToSRGB(_OutterFlameColor) : _OutterFlameColor;
            float4 _Property_aa3eee1c710e48ff833377cae2890b24_Out_0 = IsGammaSpace() ? LinearToSRGB(_MainColor) : _MainColor;
            float _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1);
            float _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2;
            Unity_Multiply_float_float(_OneMinus_364e55b658a7416e8efd29b7d5f7fcb5_Out_1, 2, _Multiply_3be1ea122189430eac6435db8df19cdf_Out_2);
            float2 _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0 = float2(_Multiply_3be1ea122189430eac6435db8df19cdf_Out_2, IN.TimeParameters.y);
            float2 _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_f09789213ab74817a39b87f7bf70840d_Out_0, _TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3);
            float _Property_f513090750e84d40a8d84766cc36e62e_Out_0 = _Density;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3;
            float _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_6ef966117a2d4bf6ae8daebfeb2eeb70_Out_3, 2, _Property_f513090750e84d40a8d84766cc36e62e_Out_0, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, _Voronoi_c077930ea17a428bb6f3dd2f880082e9_Cells_4);
            float _Power_876802c01b1e435091f1643e714a33a4_Out_2;
            Unity_Power_float(_Voronoi_c077930ea17a428bb6f3dd2f880082e9_Out_3, 2, _Power_876802c01b1e435091f1643e714a33a4_Out_2);
            float _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2;
            Unity_Multiply_float_float(_Power_876802c01b1e435091f1643e714a33a4_Out_2, 5, _Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2);
            float4 _UV_d330e0a791714618987503aefda68ccc_Out_0 = IN.uv0;
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_R_1 = _UV_d330e0a791714618987503aefda68ccc_Out_0[0];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_G_2 = _UV_d330e0a791714618987503aefda68ccc_Out_0[1];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_B_3 = _UV_d330e0a791714618987503aefda68ccc_Out_0[2];
            float _Split_cbe6e4ce039641ea8d3914edacad64a4_A_4 = _UV_d330e0a791714618987503aefda68ccc_Out_0[3];
            float _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1;
            Unity_OneMinus_float(_Split_cbe6e4ce039641ea8d3914edacad64a4_G_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1);
            float _Add_900201d365624a198e9ba914f8935c89_Out_2;
            Unity_Add_float(_Multiply_e271cd78a6fc426ea95300dee0bd5c26_Out_2, _OneMinus_c9a0166a541845dab524f4b7063671ea_Out_1, _Add_900201d365624a198e9ba914f8935c89_Out_2);
            float _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1;
            Unity_DegreesToRadians_float(180, _DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1);
            float4 _UV_0ae088b93f05405c89adec82b7cec810_Out_0 = IN.uv0;
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[0];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[1];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_B_3 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[2];
            float _Split_9fdbc565b5d04641be6d5d7fb7bd996b_A_4 = _UV_0ae088b93f05405c89adec82b7cec810_Out_0[3];
            float _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2;
            Unity_Multiply_float_float(_DegreesToRadians_98c7c6ec632545ed89c7c2e8a6f43075_Out_1, _Split_9fdbc565b5d04641be6d5d7fb7bd996b_G_2, _Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2);
            float _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1;
            Unity_Sine_float(_Multiply_44cf0954ae644075b8b7b65ed1c8fd35_Out_2, _Sine_ad36353488c74a378d89f6e3ae290a09_Out_1);
            float _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0 = _SideCutOut;
            float _Power_e73d214d36974172897d0ac7ad68776c_Out_2;
            Unity_Power_float(_Sine_ad36353488c74a378d89f6e3ae290a09_Out_1, _Property_de660f22fa5b4f849ca3fdacd1b16503_Out_0, _Power_e73d214d36974172897d0ac7ad68776c_Out_2);
            float _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_e73d214d36974172897d0ac7ad68776c_Out_2, _Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3);
            float _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1;
            Unity_OneMinus_float(_Split_9fdbc565b5d04641be6d5d7fb7bd996b_R_1, _OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1);
            float _Property_94007eaee9d94c8aa4e15139e7945492_Out_0 = _TopCutOut;
            float _Power_435d91e9236b45828f54670136bf12aa_Out_2;
            Unity_Power_float(_OneMinus_7db479d458934e90a666dc2a1082afc8_Out_1, _Property_94007eaee9d94c8aa4e15139e7945492_Out_0, _Power_435d91e9236b45828f54670136bf12aa_Out_2);
            float _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3;
            Unity_Smoothstep_float(0.02, 1, _Power_435d91e9236b45828f54670136bf12aa_Out_2, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3);
            float _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2;
            Unity_Multiply_float_float(_Smoothstep_95b9be4479384cfdba35236bd6d67515_Out_3, _Smoothstep_417547a7b1cc4cc695019188c5cdd8a5_Out_3, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2);
            float _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2;
            Unity_Multiply_float_float(_Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, 1.5, _Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2);
            float _Property_b79c3f484a9a4a76ad32b053aee56812_Out_0 = _Size;
            float _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1;
            Unity_OneMinus_float(_Property_b79c3f484a9a4a76ad32b053aee56812_Out_0, _OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1);
            float _Add_44067c9172b6430299f6498ef84b1fdc_Out_2;
            Unity_Add_float(_OneMinus_baa63ba46fb4499b9869d95b7a1fbe0b_Out_1, 10, _Add_44067c9172b6430299f6498ef84b1fdc_Out_2);
            float _Property_9d8702d4ad38405dbe31af92628115db_Out_0 = _GrowSpeed;
            float _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_9d8702d4ad38405dbe31af92628115db_Out_0, _Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2);
            float _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1;
            Unity_Sine_float(_Multiply_f7ebd9a05dbd439f9a00ce4a9f3ee3b3_Out_2, _Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1);
            float _Add_fde914a920eb4e508b0b79404d5627ae_Out_2;
            Unity_Add_float(_Sine_6aef342fe2aa43b2a38f49d7a80c8a60_Out_1, 2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2);
            float _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2;
            Unity_Multiply_float_float(_Add_44067c9172b6430299f6498ef84b1fdc_Out_2, _Add_fde914a920eb4e508b0b79404d5627ae_Out_2, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2);
            float _Property_85a315d160c0400dba8557ebb894c35a_Out_0 = _FlickAmount;
            float _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3;
            Unity_Lerp_float(1, _Multiply_5d3faed0022142ecb0cf533a3ffb2658_Out_2, _Property_85a315d160c0400dba8557ebb894c35a_Out_0, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3);
            float _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2;
            Unity_Power_float(_Multiply_ce016dbd869a433aacbd621219f5dbf1_Out_2, _Lerp_a2c76aac3ba144b5ab1dcf6a9d6c28bb_Out_3, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2);
            float _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2;
            Unity_Multiply_float_float(_Add_900201d365624a198e9ba914f8935c89_Out_2, _Power_4065f3cdf7f04c51aa692b4095fd6bcb_Out_2, _Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2);
            float _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3;
            Unity_Clamp_float(_Multiply_5dc74124783f455f8e40e2efd9f59b9e_Out_2, 0, 1, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3);
            float _Power_eb5a01505c9546ffb884a2626df23f13_Out_2;
            Unity_Power_float(_Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, 5, _Power_eb5a01505c9546ffb884a2626df23f13_Out_2);
            float4 _Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3;
            Unity_Lerp_float4(_Property_884635e6c9294e3aaee2f5d0a6539855_Out_0, _Property_aa3eee1c710e48ff833377cae2890b24_Out_0, (_Power_eb5a01505c9546ffb884a2626df23f13_Out_2.xxxx), _Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3);
            float4 _Property_bf625041a2254b3c800d4003b885fbaf_Out_0 = IsGammaSpace() ? LinearToSRGB(_DotColor) : _DotColor;
            float _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1;
            Unity_OneMinus_float(IN.TimeParameters.x, _OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1);
            float2 _Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.y);
            float _Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0 = _DotSpeed;
            float2 _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2;
            Unity_Multiply_float2_float2(_Vector2_8b75ec4b255b49b791a8be0875c3530b_Out_0, (_Property_82874edb1b7a46a4a16c0f7fc0fe41c7_Out_0.xx), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2);
            float2 _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d4fff5819da348e8831a8d8d4babcb0c_Out_2, _TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3);
            float _Property_3308b7072f014457832f27b76be59cdd_Out_0 = _BigDotSize;
            float _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2;
            Unity_Multiply_float_float(_Property_3308b7072f014457832f27b76be59cdd_Out_0, -1, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2);
            float _Add_d9113240b1b54159902e1e83c00ad97f_Out_2;
            Unity_Add_float(25, _Multiply_cab74655394d42f7bf2d4e6334e3b641_Out_2, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2);
            float _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_e9abab62cc624c64932b50e4046b402d_Out_3, _Add_d9113240b1b54159902e1e83c00ad97f_Out_2, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2);
            float _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_6591ca5282874eef87a921faab58ae09_Out_2, _Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3);
            float _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2;
            Unity_Multiply_float_float(_Smoothstep_00be09e71417457ea20a8c327fa61ba8_Out_3, 5, _Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2);
            float2 _Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0 = float2(_OneMinus_1e643371641e4b70ab392809749a2d1e_Out_1, IN.TimeParameters.z);
            float _Property_3efe4630801e4184acf1494a309782c6_Out_0 = _DotSpeedSecond;
            float2 _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2;
            Unity_Multiply_float2_float2(_Vector2_534abebc8dcd4d55b696a6389b3f2993_Out_0, (_Property_3efe4630801e4184acf1494a309782c6_Out_0.xx), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2);
            float2 _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1.5), _Multiply_d460f3f17e61407d9f1eba4296084009_Out_2, _TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3);
            float _Property_4dfc2acea8434c739af90ae838caf34f_Out_0 = _SmallDotSize;
            float _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2;
            Unity_Multiply_float_float(_Property_4dfc2acea8434c739af90ae838caf34f_Out_0, -1, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2);
            float _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2;
            Unity_Add_float(25, _Multiply_38aea8f7db5a481caf8cd26c4f8bc2f5_Out_2, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2);
            float _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_31151fa3520e405785f9937cf7f7ae3a_Out_3, _Add_2fe82f3957d1476da1de9fadc30c3e9e_Out_2, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2);
            float _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3;
            Unity_Smoothstep_float(0.9, 1, _GradientNoise_50a0983dc3034276a2a46130514068d0_Out_2, _Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3);
            float _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2;
            Unity_Multiply_float_float(_Smoothstep_fbfe2aad60c0478ebe48ac8bc55071e8_Out_3, 5, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2);
            float _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2;
            Unity_Add_float(_Multiply_a4ed0332a93e4576a4306a3d6cfa14dd_Out_2, _Multiply_4bd7a43cbd59475aba5d4643c832796a_Out_2, _Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2);
            float _Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3;
            Unity_Clamp_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, 0, 1, _Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3);
            float4 _Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3;
            Unity_Lerp_float4(_Lerp_06fc85c8627143ae92baf6a504cf3d88_Out_3, _Property_bf625041a2254b3c800d4003b885fbaf_Out_0, (_Clamp_7f7641dccd1543ecababd11ede3236ba_Out_3.xxxx), _Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3);
            float _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2;
            Unity_Multiply_float_float(_Add_246a221c4dcc458cb33a37f2a98cdf72_Out_2, _Multiply_e0508f8d1b354f34ab14cccae1dbac16_Out_2, _Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2);
            float _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3;
            Unity_Clamp_float(_Multiply_1aa606f6765a48498ffc9e2ad2c2e2be_Out_2, 0, 1, _Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3);
            float _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2;
            Unity_Add_float(_Clamp_5eefd8d3aecd4a92b1bb568f22324a00_Out_3, _Clamp_32352b8fac7644aa8d6fd1a19403ecac_Out_3, _Add_68ea4332311a4ea8b2c7966894897e3e_Out_2);
            float _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            Unity_Clamp_float(_Add_68ea4332311a4ea8b2c7966894897e3e_Out_2, 0, 1, _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3);
            surface.BaseColor = (_Lerp_24fd77d4146f49b585e226d6c7a99436_Out_3.xyz);
            surface.Alpha = _Clamp_07525ed22fa4495f9a5b9696335c34eb_Out_3;
            surface.NormalTS = IN.TangentSpaceNormal;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
            output.TangentSpaceNormal =                         float3(0.0f, 0.0f, 1.0f);
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteForwardPass.hlsl"
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}