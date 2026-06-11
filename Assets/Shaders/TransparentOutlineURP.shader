Shader "Custom/Transparent Lit Outline"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(0, 0.1)) = 0.02
        _Metallic("Metallic", Range(0, 1)) = 0
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 300

        // Render the expanded back faces first to form the silhouette.
        Pass
        {
            Name "OUTLINE"

            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata input)
            {
                v2f output;
                float3 expandedPosition =
                    input.vertex.xyz + normalize(input.normal) * _OutlineThickness;
                output.vertex = UnityObjectToClipPos(float4(expandedPosition, 1.0));
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows
        #pragma target 3.0

        sampler2D _BaseMap;

        struct Input
        {
            float2 uv_BaseMap;
        };

        fixed4 _BaseColor;
        half _Metallic;
        half _Smoothness;

        void surf(Input input, inout SurfaceOutputStandard output)
        {
            fixed4 color = tex2D(_BaseMap, input.uv_BaseMap) * _BaseColor;
            output.Albedo = color.rgb;
            output.Metallic = _Metallic;
            output.Smoothness = _Smoothness;
            output.Alpha = color.a;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
