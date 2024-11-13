Shader "UI/CrosshairShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Thickness("Thickness", Range(0, 1)) = 0.05
        _Radius("Radius", Range(0, 1)) = 0.4
        _ShadowRadius("ShadowRadius", Range(0, 1)) = 0.4
        _ShadowThickness("ShadowThickness", Range(0, 1)) = 0.05
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off
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

            fixed4 _Color;
            float _Radius;
            float _Thickness;
            float _ShadowRadius;
            float _ShadowThickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float circle = smoothstep(_Radius, _Radius - _Thickness, length(i.uv));
                float shadow = smoothstep(_ShadowRadius, _ShadowRadius - _ShadowThickness, length(i.uv));

                return fixed4(_Color.rgb * circle, _Color.a * shadow);
            }
            ENDCG
        }
    }
}
