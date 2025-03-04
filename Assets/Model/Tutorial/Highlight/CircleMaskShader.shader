Shader "Custom/CircleMaskShader"
{
   Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _MaskCenter("Mask Center", Vector) = (0.5, 0.5, 0, 0)
        _MaskRadius("Mask Radius", float) = 0.25
        _AspectRatio("Aspect Ratio", float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
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
            float4 _MainTex_ST;
            float4 _MaskCenter;
            float _MaskRadius;
            float _AspectRatio;
            float4 _BaseColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 aspectCorrectedTexcoord = float2(i.texcoord.x * _AspectRatio, i.texcoord.y);
                half2 maskPos = half2(_MaskCenter.x * _AspectRatio, _MaskCenter.y);
                half dist = distance(aspectCorrectedTexcoord, maskPos);
                half4 col = tex2D(_MainTex, i.texcoord);

                col.rgb *= _BaseColor.rgb;
                
                col.a *= _BaseColor.a * step(_MaskRadius, dist);

                return col;
            }
            ENDCG
        }
    }
}