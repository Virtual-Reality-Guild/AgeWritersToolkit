// Opaque shader that blends two textures together according to a third alpha ramp texture
// Has support for 2 UV layers for each texture, and 3 UV layers for the alpha texture
// Supports an additional overlay texture that lays over the main texture
Shader "Opcode/AlphaBlend" {
    Properties {
        _MainTex ("Base (RGBA)", 2D) = "white" {}        
        //Shows drop down in material inspector
        [Enum(UV0, 0, UV1, 1)] _MainTexUVSet("MainTexture UV Set", Float) = 0.0
        
        _SecondTex("SecondaryTex (RGBA)", 2D) = "white" {}        
        [Enum(UV0, 0, UV1, 1)] _SecondTexUVSet("SecondaryTexture UV Set", Float) = 0.0

        _OverlayTex("OverlayTex (RGBA)", 2D) = "white" {}
        [Enum(UV0, 0, UV1, 1)] _OverlayTexUVSet("OverlayTexture UV Set", Float) = 0.0

        _AlphaTex("AlphaTex (RGBA)", 2D) = "white" {}
        [Enum(UV0, 0, UV1, 1, UV2, 2)] _AlphaTexUVSet("AlphaTexture UV Set", Float) = 0.0

        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        ZWrite On
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows keepalpha vertex:vert
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex, _SecondTex, _AlphaTex, _OverlayTex;
        fixed4 _MainTex_ST, _SecondTex_ST, _AlphaTex_ST, _OverlayTex_ST;
        fixed4 _Color;
        int _MainTexUVSet;
        int _SecondTexUVSet;
        int _AlphaTexUVSet;
        int _OverlayTexUVSet;

        struct Input {
            float2 MeshUV0;
            float2 MeshUV1;    
            float2 MeshUV2;
        };

        void vert(inout appdata_full v, out Input IN) {
            // choose between UV sets
            // v.texcoord is first UV set, v.texcoord1 is second UV set
            IN.MeshUV0 = v.texcoord;
            IN.MeshUV1 = v.texcoord1;
            IN.MeshUV2 = v.texcoord2;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            fixed2 mainuv_Tex = _MainTexUVSet == 0 ? IN.MeshUV0 : IN.MeshUV1;
            mainuv_Tex = mainuv_Tex * _MainTex_ST.xy + _MainTex_ST.zw;
            fixed2 seconduv_Tex = _SecondTexUVSet == 0 ? IN.MeshUV0 : IN.MeshUV1;
            seconduv_Tex = seconduv_Tex * _SecondTex_ST.xy + _SecondTex_ST.zw;
            fixed2 overlayuv_Tex = _OverlayTexUVSet == 0 ? IN.MeshUV0 : IN.MeshUV1;
            overlayuv_Tex = overlayuv_Tex * _OverlayTex_ST.xy + _OverlayTex_ST.zw;
            fixed2 alphauv_Tex;
            switch (_AlphaTexUVSet) {
            case 0:
                alphauv_Tex = IN.MeshUV0;
                break;
            case 1:
                alphauv_Tex = IN.MeshUV1;
                break;
            case 2: 
                alphauv_Tex = IN.MeshUV2;
                break;
            default:
                alphauv_Tex = IN.MeshUV0;
                break;
            }
            alphauv_Tex = alphauv_Tex * _AlphaTex_ST.xy + _AlphaTex_ST.zw;
            half4 overlayTexColor = tex2D(_OverlayTex, overlayuv_Tex);
            half4 mainTexColor = tex2D(_MainTex, mainuv_Tex) * overlayTexColor.a;
            half4 secondTexColor = tex2D(_SecondTex, seconduv_Tex);
            half4 alphaTexColor = tex2D(_AlphaTex, alphauv_Tex);

            half4 c = lerp(mainTexColor, secondTexColor, alphaTexColor.a) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Standard"
}