Shader "Custom/CusTom_UnlitAlhaTest"
{
   Properties
    {
        _BaseColor("Color",color) = (1,1,1,1)
        _BaseMap ("Texture", 2D) = "white" {}

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2

    }
    SubShader
    {
                Tags {"RenderPipeline" = "UniversalPipeline"  "RenderType" = "TransparentCutout" "Queue" = "AlphaTest"}
                Cull [_Cull]
                Pass
        {
                 	 Name "Universal Forward"
            Tags {"LightMode" = "UniversalForward"}
                     AlphaToMask On

             HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
           #pragma vertex vert
            #pragma fragment frag

            //unity defiend keywords
            #pragma multi_compile_instancing

            //cg shader는 .cginc를 hlsl shader는 .hlsl을 include하게 됩니다.
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"//언릿인푼참조
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"        	
  
            //vertex buffer에서 읽어올 정보를 선언합니다. 	
            struct VertexInput
            {
              float4 vertex : POSITION;
              float2 uv : TEXCOORD0;
              UNITY_VERTEX_INPUT_INSTANCE_ID

            };

            //보간기를 통해 버텍스 셰이더에서 픽셀 셰이더로 전달할 정보를 선언합니다.
            struct VertexOutput
            {
                float2 uv : TEXCOORD0;
                float fogCoord : TEXCOORD1;
                float4 vertex  	: SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            //버텍스 셰이더
            VertexOutput vert(VertexInput input)
            {

                VertexOutput output = (VertexOutput)0;
            
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                //VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz );
                //output.vertex = vertexInput.positionCs;
                output.vertex = TransformObjectToHClip(input.vertex.xyz);
                output.uv = input.uv;
                output.fogCoord = ComputeFogFactor(output.vertex.z);

                return output;
            }

            //픽셀 셰이더
            half4 frag(VertexOutput input) : SV_Target
            { 
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
       	
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,input.uv);
                texColor.rgb *=_BaseColor.rgb;

                return texColor; 
            }

            ENDHLSL     
         }
    }
}