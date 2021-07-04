Shader "Materialize/Dissolve2"
{
 
    Properties
    {
        _DissolveTex("Dissolve", 2D) = "white" {}
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _SpecularTex("Specular", 2D) = "white" {}
        _SpecularColor("Specular Amount", Range(0,1)) = 0
        _Normal("Normal",2D) = "white" {}
        _DissolveAmount("DissolveAmount", Range( 0 , 1)) = 0
       
       
        _DissolveStart("Dissolve Start Point", Vector) = (1, 1, 1, 1)
        _DissolveEnd("Dissolve End Point", Vector) = (0, 0, 0, 1)
        _DissolveBand("Dissolve Band Size", Float) = 0.25
 
        _GlowIntensity("Glow Intensity", Range(0.0, 5.0)) = 3
        _GlowScale("Glow Size", Range(0.0, 5.0)) = 0.5
        _Glow("Glow Color", Color) = (1, 0, 0, 1)
        _GlowEnd("Glow End Color", Color) = (1, 1, 0, 1)
        _GlowColFac("Glow Colorshift", Range(0.01, 2.0)) = 1
 
    }
 
    SubShader
    {
        Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+1" "IsEmissive" = "true" }
        LOD 200
        Cull Off
        CGPROGRAM
        #include "UnityShaderVariables.cginc"
        #pragma target 3.0
        #pragma surface surf StandardSpecular addshadow fullforwardshadows keepalpha
        struct Input
        {
           
            float2 uv_DissolveTex;
            float2 uv_MainTex;
           
            float3 worldPos;
        };
 
        uniform float _DissolveAmount;
        uniform sampler2D _DissolveTex;
        uniform sampler2D _SpecularTex;
        uniform float _SpecularColor;
        uniform sampler2D _MainTex;
        uniform sampler2D _Normal;
        uniform float4 _DissolveStart;
        uniform float4 _DissolveEnd;
        uniform float _DissolveBand;
        static float3 dDir = normalize(_DissolveStart - _DissolveEnd);
        static float3 dissolveStartConverted = _DissolveStart - _DissolveBand * dDir;
        static float dBandFactor = 1.0f / _DissolveBand;
 
        fixed4 _Color;
        fixed4 _Glow;
        fixed4 _GlowEnd;
          half _GlowIntensity;
        half _GlowScale;
        uniform float _isZPass;
        half _GlowColFac;
 
 
        void surf( Input IN , inout SurfaceOutputStandardSpecular o )
        {
            //Convert dissolve progression to -1 to 1 scale.
            half dBase = -2.0f * _DissolveAmount + 1.0f;
 
            //Read from noise texture.
            fixed4 dTex = tex2D(_DissolveTex, IN.uv_DissolveTex);
            //Convert dissolve texture sample based on dissolve progression.
            half dTexRead = dTex.r + dBase;
 
            float3 dPoint = lerp(dissolveStartConverted, _DissolveEnd, _DissolveAmount);
            float dGeometry = dot(IN.worldPos- dPoint, dDir) * dBandFactor;
 
            //Set output alpha value.
            //half alpha = clamp(dTexRead, 0.0f, 1.0f);
            half dFinal = dTexRead + dGeometry;
 
            half dPredict = (_GlowScale * (1-_DissolveAmount) - dFinal) * _GlowIntensity;
            half dPredictCol = (_GlowScale * (1-_DissolveAmount) * _GlowColFac - dFinal) * _GlowIntensity;    
            fixed4 glowCol = dPredict * lerp(_Glow, _GlowEnd, clamp(dPredictCol, 0.0f, 1.0f)) * dTex.a;
            glowCol = clamp(glowCol, 0.0f, 1.0f);
 
           
 
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
 
           
 
            o.Albedo = c.rgb;
            //o.Alpha = clamp(dFinal, 0.0f, 1.0f) -0.5f;
 
            o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));
            o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).g * _SpecularColor;
            //o.Occlusion =  (1-tex2D(_SpecularTex, IN.uv_MainTex).g * _SpecularColor);
 
            o.Smoothness = 0;
            o.Emission = glowCol;
            clip( clamp(dFinal, 0.0f, 1.0f) -0.5f );
 
           
        }
        ENDCG
 
    }
    Fallback "Diffuse"
}
