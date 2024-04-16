Shader "PXR/Hand SingPath"
{
    Properties
    {
        [Header(BaseColor)]
        _InnerColor("innerColor",Color) = (1,1,1,1)
        _OutColor("outColor",Color) = (1,1,1,1)
        _FresnelPower("fresenlPower",float) = 1

        [Header(Light)][Space(5)]
        _PressLight("pressLight",Color) = (1,1,1,1)
        _ClickLight("clickLight",Color) = (1,1,1,1)
        _PressRange("pressRange",Range(0,1)) = 0.015
        _ClickRange("clickRange",Range(0,1)) = 0.015
        _ClickPosition("ClickPosition",Vector) = (1,1,1,1)
        _PressIntensity("pressItensity",Range(0,1)) = 1

        [Header(Wrist)][Space(10)]
        _WristFadeRange("wristFadeRange",Range(0,1)) = 1

        _FadeIn("fadeIm",Range(0,1)) = 0
    }

        CGINCLUDE
#include "Lighting.cginc"
#pragma target 3.0

            float4 _InnerColor;
        float4 _OutColor;
        float _FresnelPower;

        float4 _PressLight;
        float4 _ClickLight;
        half _PressIntensity;
        float4 _ClickPosition;

        float _PressRange;
        float _ClickRange;

        float _WristFadeRange;
        float _FadeIn;

        //-----------------描边参数----------------
        struct OutlineVertexInput
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;

            UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
        };

        struct OutlineVertexOutput
        {
            float4 vertex : SV_POSITION;
            float2 uv:TEXCOORD3;

            UNITY_VERTEX_OUTPUT_STEREO //Insert
        };

        //-------------------------------------------

        //---------------------正常绘制参数-------------------
        struct VertexInput
        {
            float4 vertex : POSITION;
            half3 normal : NORMAL;
            half4 vertexColor : COLOR;
            float2 texcoord : TEXCOORD0;

            UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
        };

        struct VertexOutput
        {
            float4 vertex : SV_POSITION;
            float3 worldPos : TEXCOORD1;
            float3 worldNormal : TEXCOORD2;
            float2 uv:TEXCOORD3;

            UNITY_VERTEX_OUTPUT_STEREO //Insert

        };

        void CustomRemap(in float4 inValue, float2 inMinMax, float2 outMinMax, out float4 outValue)
        {
            outValue = outMinMax.x + (inValue - inMinMax.x) * (outMinMax.y - outMinMax.x) / (inMinMax.y - inMinMax.x);
        }

        float GetAlpha(float2 uv)
        {
            float dis = distance(float2(0.5, 0), uv * float2(0.9, 1) + float2(0.05, 0));
            float4 s1;
            CustomRemap(_WristFadeRange, float2(0, 1), float2(0.12, 1), s1);
            const float s2 = 0.12;
            float alpha = smoothstep(s2, s1, dis);

            float s3 = 1 - _FadeIn;
            float4 s4;
            CustomRemap(s3, float2(0, 0.5), float2(0, 1), s4);
            s4 = 1.1 * saturate(s4);

            return alpha * smoothstep(s3, s4, dis);
        }
        float GetFresnel(float3 viewDir, float3 normal, float power)
        {
            return pow(1 - dot(viewDir, normal), power);
        }

        //-----------------正常绘制------------------------
        VertexOutput baseVertex(VertexInput v)
        {
            VertexOutput o;

            UNITY_SETUP_INSTANCE_ID(v); //Insert
            UNITY_INITIALIZE_OUTPUT(VertexOutput, o); //Insert
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
            return o;
        }

        fixed4 baseFragment(VertexOutput v) : SV_Target
        {
            float3 normalWS = normalize(v.worldNormal);
            float3 viewWS = normalize(UnityWorldSpaceViewDir(v.worldPos));
            float fresnel = saturate(GetFresnel(viewWS, normalWS, _FresnelPower));

            float4 baseColor = lerp(_InnerColor, _OutColor, fresnel);
            float4 clickColor = lerp(_PressLight, _ClickLight, step(0.99, _PressIntensity));


            float3 localClickPos = mul((float3x3)unity_WorldToObject, _ClickPosition);
            float3 vertexPos = mul((float3x3)unity_WorldToObject, v.worldPos);
            float dis = distance(localClickPos, vertexPos);

            float2 inMinMax = float2(0, lerp(_PressRange, _ClickRange, _PressIntensity));
            float2 outMinMax = float2(1, 0);
            float4 s;
            CustomRemap(dis, inMinMax, outMinMax, s);
            float4 r = smoothstep(0, 1, clamp(s, 0, 1));
            r.a *= _PressIntensity;

            fixed4 finalCol = lerp(baseColor, clickColor, r.a);
            finalCol.a *= saturate(GetAlpha(v.uv));
            return finalCol;
        }

            ENDCG

            SubShader
        {
            Tags
            {
                "RenderPipeline" = "UniversalPipeline"
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
            }
                Pass
            {
                Name "Depth"
                Tags
                {
                    "LightMode" = "SRPDefaultUnlit"
                }
                ZWrite On
                ColorMask 0
            }
                Pass
            {
                Name "BaseColor"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off
                CGPROGRAM
                #pragma vertex baseVertex
                #pragma fragment baseFragment
                ENDCG
            }
        }
        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
            }
            LOD 200
            Pass
            {
                Name "Depth"
                ZWrite On
                ColorMask 0
            }
            Pass
            {
                Name "Interior"
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite On
                CGPROGRAM
                #pragma vertex baseVertex
                #pragma fragment baseFragment
                ENDCG
            }
        }
}