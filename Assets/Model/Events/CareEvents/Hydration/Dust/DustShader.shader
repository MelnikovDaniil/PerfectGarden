Shader "Unlit/DustShader"
{
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Paleness ("Paleness", Range(0, 1)) = 0.5
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        half _Paleness;
        float4 _Color;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed3 finalColor = tex.rgb * _Color.rgb;
            o.Albedo = lerp(finalColor, fixed3(1.0, 1.0, 1.0), _Paleness);
            o.Alpha = tex.a * _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}