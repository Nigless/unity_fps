Shader "Unlit/Billboard"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(Off,0,On,1)]_ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 0
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Cull [_Culling]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
            struct v2f {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;

                float4 viewPos = mul(UNITY_MATRIX_MV, float4(0,0,0,1));

                float3 objScale;
                objScale.x = length(unity_ObjectToWorld._m00_m10_m20);
                objScale.y = length(unity_ObjectToWorld._m01_m11_m21);

                viewPos.xyz += float3(
                    v.vertex.x * objScale.x,
                    v.vertex.y * objScale.y,
                    0
                );

                o.vertex = mul(UNITY_MATRIX_P, viewPos);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
