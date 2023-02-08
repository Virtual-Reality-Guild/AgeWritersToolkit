// Transparent shader that can scroll a texture on top of another texture
// Has support for emission
Shader "Opcode/ScrollingUV" {
    Properties {
        _MainTex ("Base (RGBA)", 2D) = "white" {}        
        //Shows drop down in material inspector
        [Enum(UV0, 0, UV1, 1)] _MainTexUVSet("MainTexture UV Set", Float) = 0.0
        
        _ScrollTex("ScrollingTex (RGBA)", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ScrollXSpeed ("X Scroll Speed", Range(-50, 50)) = 0
        _ScrollYSpeed ("Y Scroll Speed", Range(-50, 50)) = 0
        [Toggle] _Emissive("Emissive", Float) = 0
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector" = "True" "RenderType"="Soft Additive" "IsEmissive" = "true"}
        LOD 100

        //Zwrite Off
        Cull Off
        Blend OneMinusDstColor One // Soft additive

        CGPROGRAM
        #pragma surface surf Lambert alpha vertex:vert
        #pragma target 3.0

        sampler2D _MainTex, _ScrollTex;
        fixed4 _MainTex_ST, _ScrollTex_ST;
        fixed4 _Color;
        fixed _ScrollXSpeed;
        fixed _ScrollYSpeed;
        int _MainTexUVSet;
        int _Emissive;

        struct Input {
            float2 MainTexUV1;
            float2 MainTexUV2;
            float2 ScrollTexUV1;
        };

        void vert(inout appdata_full v, out Input IN) {
            // choose between UV sets
            // v.texcoord is first UV set, v.texcoord1 is second UV set
            IN.MainTexUV1 = v.texcoord;
            IN.MainTexUV2 = v.texcoord1;
            IN.ScrollTexUV1 = v.texcoord;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            fixed varX = _ScrollXSpeed * _Time;
            fixed varY = _ScrollYSpeed * _Time;
            fixed2 scrolluv_Tex = IN.ScrollTexUV1 * _ScrollTex_ST.xy + _ScrollTex_ST.zw + fixed2(varX, varY);
            fixed2 mainuv_Tex = _MainTexUVSet == 0 ? IN.MainTexUV1 : IN.MainTexUV2;
            mainuv_Tex = mainuv_Tex * _MainTex_ST.xy + _MainTex_ST.zw;
            half4 c = tex2D(_ScrollTex, scrolluv_Tex) * tex2D(_MainTex, mainuv_Tex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            if(_Emissive == 1)
                o.Emission = c.rgb;
        }
        ENDCG
    }
    FallBack "Standard"
}