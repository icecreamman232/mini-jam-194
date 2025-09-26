Shader "SGGames/Reflected Wipe Transition Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Cutoff ("Cutoff", Range(0,1)) = 0
        [Toggle] _IsHorizontal(" Is Horizontal", float) = 1
        [Toggle] _IsFadeIn(" Is Fade In", float) = 1
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
            // make fog work
            #pragma multi_compile_fog
            float _Cutoff;
            fixed4 _TintColor;
            float _IsHorizontal;
            float _IsFadeIn;

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float get_calculate_coordinate(float isHorizontal, v2f i)
            {
                if (isHorizontal == 1)
                {
                    return i.uv.x - 0.5;
                }
                else
                {
                    return i.uv.y - 0.5;
                }
            }

            float get_calculate_cutoff(float isFadeIn)
            {
                if (isFadeIn)
                {
                    return (1.0 - _Cutoff);
                }
                else
                {
                    return _Cutoff;
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (abs(get_calculate_coordinate(_IsHorizontal, i)) * 2.0 > get_calculate_cutoff(_IsFadeIn))
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
