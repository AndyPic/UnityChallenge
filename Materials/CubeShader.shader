Shader "Custom/CubeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
        }
        LOD 200

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Standard fullforwardshadows alpha
        #pragma multi_compile_instancing
        #pragma multi_compile_fwdbase

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;

        UNITY_INSTANCING_BUFFER_START(Prop)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            #define _Color_arr Prop
        UNITY_INSTANCING_BUFFER_END(Prop)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);

            o.Albedo = c.rgb;
            o.Alpha = c.a;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
