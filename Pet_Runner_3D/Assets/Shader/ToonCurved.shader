Shader "Custom/ToonCurved"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.005
        _Curvature ("Curvature Intensity", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGINCLUDE
        #include "UnityCG.cginc"

        struct appdata_t
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 pos : SV_POSITION;
            float4 worldPos : TEXCOORD1;
        };

        sampler2D _MainTex;
        float4 _Color;
        float _Curvature;

        v2f vert(appdata_t v)
        {
            v2f o;
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
            
            // Apply curved surface effect
            float curveAmount = _Curvature * worldPos.z * worldPos.z;
            worldPos.y += curveAmount;

            o.pos = UnityObjectToClipPos(float4(worldPos.xyz, 1));
            o.uv = v.uv;
            o.worldPos = worldPos;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv) * _Color;
            return col;
        }
        ENDCG

        // Pass 1: Render the base toon shading
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        // Pass 2: Outline rendering
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert_outline
            #pragma fragment frag_outline
            
            struct v2f_outline
            {
                float4 pos : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            v2f_outline vert_outline(appdata_t v)
            {
                v2f_outline o;
                float3 normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                float4 pos = UnityObjectToClipPos(v.vertex);

                // Expand along normal direction for outline
                pos.xy += normal.xy * _OutlineWidth * pos.w;

                o.pos = pos;
                return o;
            }

            fixed4 frag_outline(v2f_outline i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
