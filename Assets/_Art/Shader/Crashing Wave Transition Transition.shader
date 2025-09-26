Shader "SGGames/Wave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientTex ("Gradient", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0,0,0,1)
        _Cutoff ("Cutoff Value", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
            
        }
        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _GradientTex;
            float4 _MainTex_ST;
            float _Cutoff;
            fixed4 _TintColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 p = tex2D(_GradientTex, i.uv);
                if (p.r < _Cutoff)
                {
                    return _TintColor;
                }
                else
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col.w = 0;
                    return col;
                }
            }
            ENDCG
        }
    }
}
