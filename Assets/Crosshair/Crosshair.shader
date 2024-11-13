Shader "UI/CrosshairShader"
{
    Properties
    {
        _DotColor("DotColor", Color) = (1,1,1,1)
        _DotRadius("DotRadius", Range(0, 1)) = 0.4
        _DotBlurRadius("DotBlurRadius", Range(0, 1)) = 0.05
        _ShadowSpreadRadius("ShadowSpreadRadius", Range(0, 1)) = 0.4
        _ShadowBlurRadius("ShadowBlurRadius", Range(0, 1)) = 0.05
        _HoleRadius("HoleRadius", Range(0, 1)) = 0
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

            fixed4 _DotColor;
            float _DotRadius;
            float _DotBlurRadius;
            float _ShadowSpreadRadius;
            float _ShadowBlurRadius;
            float _HoleRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                _HoleRadius = _HoleRadius - _DotBlurRadius;
                
                float circle = smoothstep(_DotRadius, _DotRadius - _DotBlurRadius, length(i.uv)) - smoothstep( _HoleRadius + _DotBlurRadius, _HoleRadius  , length(i.uv));
                float shadow = smoothstep(_DotRadius + _ShadowSpreadRadius + _ShadowBlurRadius, _DotRadius + _ShadowSpreadRadius , length(i.uv)) - smoothstep( _HoleRadius - _ShadowSpreadRadius, _HoleRadius- _ShadowSpreadRadius- _ShadowBlurRadius, length(i.uv));

                return fixed4(_DotColor.rgb * circle, _DotColor.a * shadow);
            }
            ENDCG
        }
    }
}
