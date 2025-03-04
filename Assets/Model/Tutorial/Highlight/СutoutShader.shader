Shader "Custom/CutoutShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1) // Цвет фона
        _CutoutPos ("Cutout Position", Vector) = (0.5, 0.5, 0, 0)
        _CutoutSize ("Cutout Size", Vector) = (0.1, 0.1, 0, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _CutoutPos;
            float4 _CutoutSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Рассчитываем UV для вырезаемой области
                float2 cutoutUV = (i.uv - _CutoutPos.xy) / _CutoutSize.xy;
                float cutout = step(1.0, length(cutoutUV)); // Инверсия: 1 внутри, 0 снаружи

                // Получаем цвет текстуры или используем цвет фона
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
                texColor.a *= cutout; // Применяем вырезание
                return texColor;
            }
            ENDCG
        }
    }
}