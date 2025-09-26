Shader "SGGames/Wipe Transition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        [Toggle] _IsHorizontal ("Is Horizontal", float) = 1.0
        [Toggle] _IsFadeIn ("Is Fade In", float) = 1.0
        _Cutoff ("Cutoff", Range(0,1)) = 0
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
            float4 _MainTex_ST;
            float _Cutoff;
            fixed4 _TintColor;
            fixed2 _Direction;
            float _IsHorizontal;
            float _IsFadeIn;


            fixed4 calculate_horizontal_wipe(v2f i, float isFadeIn)
            {
                if (isFadeIn)
                {
                    if (i.uv.x < _Cutoff)
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
                else
                {
                    if (i.uv.x > _Cutoff)
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
            }

            fixed4 calculate_vertical_wipe(v2f i, float isFadeIn)
            {
                if (isFadeIn)
                {
                    if (i.uv.y < _Cutoff)
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
                else
                {
                    if (i.uv.y > _Cutoff)
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
            }

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_IsHorizontal == 1)
                {
                    return calculate_horizontal_wipe(i, _IsFadeIn);
                }
                else
                {
                    return calculate_vertical_wipe(i, _IsFadeIn);
                }
            }
            ENDCG
        }
    }
}