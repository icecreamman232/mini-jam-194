Shader "SGGames/Spin Quadrant"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0,0,0,1)
        _Cutoff ("Cutoff", Range(0,1)) = 1.0
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            #define PI 3.1415926538
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;
            fixed4 _TintColor;

            float get_angle_in_quadrant_with_offset(float2 p)
            {
                float2 center = float2(0.5, 0.5);
                float2 dir = p - center;
                
                // Get the angle from -PI to PI
                float angle = atan2(dir.y, dir.x);
                
                // Convert to 0-2PI range
                if (angle < 0.0) angle += 2.0 * PI;
                
                // Map each quadrant's angle to 0-PI/2 range with different starting points
                float quadrantAngle;
                
                if (angle >= 0.0 && angle < PI / 2.0) {
                    // First quadrant: Start from bottom (Y-axis), spin clockwise
                    quadrantAngle = PI / 2.0 - angle;
                }
                else if (angle >= PI / 2.0 && angle < PI)
                {
                    // Second quadrant: Start from left (negative X-axis), spin clockwise  
                    quadrantAngle = PI  - angle;
                }
                else if (angle >= PI && angle < 3.0 * PI/ 2.0)
                {
                    // Third quadrant: Start from top (negative Y-axis), spin clockwise
                    quadrantAngle = 1.5 * PI - angle;
                }
                else
                {
                    // Fourth quadrant: Start from right (X-axis), spin clockwise
                    quadrantAngle = 2 * PI - angle;
                }
                
                return quadrantAngle;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Get the angle within the quadrant with different starting points
                float quadrantAngle = get_angle_in_quadrant_with_offset(uv);


                // Convert cutoff to angle (0 to PI/2)
                float cutoffAngle = _Cutoff * PI / 2.0;
                
                if (quadrantAngle < cutoffAngle)
                {
                    return _TintColor * i.color;
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