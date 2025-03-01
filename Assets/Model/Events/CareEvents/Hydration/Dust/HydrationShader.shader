Shader "Unlit/HydrationShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Desaturate("Desaturate", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Desaturate;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.texcoord);

                // Calculate luminance
                float luminance = dot(texColor.rgb, float3(0.299, 0.587, 0.114));

                // Lerp between original color and grey
                texColor.rgb = lerp(texColor.rgb, float3(luminance, luminance, luminance), _Desaturate);

                return texColor;
            }
            ENDCG
        }
    }
}