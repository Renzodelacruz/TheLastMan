Shader "TextMeshPro/MadnessText"
{
    Properties
    {
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        _MainTex ("Font Atlas", 2D) = "white" {}

        _Madness ("Madness", Range(0,1)) = 0
        _GlitchStrength ("Glitch Strength", Float) = 0.5
        _FlickerSpeed ("Flicker Speed", Float) = 5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _FaceColor;

            float _Madness;
            float _GlitchStrength;
            float _FlickerSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float random(float2 st)
            {
                return frac(sin(dot(st, float2(12.9898,78.233))) * 43758.5453123);
            }

            v2f vert(appdata v)
            {
                v2f o;

                float glitch = (random(v.vertex.xy + _Time.y) - 0.5) * _GlitchStrength * _Madness;

                v.vertex.x += glitch;
                v.vertex.y += glitch;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float flicker = sin(_Time.y * _FlickerSpeed) * 0.5 + 0.5;
                float visibility = lerp(1, flicker, _Madness);

                fixed4 col = tex2D(_MainTex, i.uv) * _FaceColor;
                col.a *= visibility;

                return col;
            }
            ENDCG
        }
    }
}