// Transparent shader that can fade a texture between fully opaque and fully transparent according to a provided alpha ramp texture
// Support for multiple UV layers
Shader "Opcode/AlphaTransparent" {
    Properties {
        _MainTex ("Base (RGBA)", 2D) = "white" {}        
        [Enum(UV0, 0, UV1, 1)] _MainTexUVSet("MainTexture UV Set", Float) = 0.0

        _AlphaTex("AlphaTex (RGBA)", 2D) = "white" {}
        [Enum(UV0, 0, UV1, 1, UV2, 2)] _AlphaTexUVSet("AlphaTexture UV Set", Float) = 0.0
        [Toggle] _Invert("Invert", Float) = 0

        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        //ZWrite On
        //Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:blend fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex, _AlphaTex;
        fixed4 _MainTex_ST, _AlphaTex_ST;
        fixed4 _Color;
        int _MainTexUVSet;
        int _AlphaTexUVSet;
        int _Invert;

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

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed2 mainuv_Tex = _MainTexUVSet == 0 ? IN.MeshUV0 : IN.MeshUV1;
            mainuv_Tex = mainuv_Tex * _MainTex_ST.xy + _MainTex_ST.zw;
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
            half4 mainTexColor = tex2D(_MainTex, mainuv_Tex);
            half4 transColor = (mainTexColor.r, mainTexColor.g, mainTexColor.b, 0);
            float alphaVal = _Invert == 0 ? tex2D(_AlphaTex, alphauv_Tex).a : 1 - tex2D(_AlphaTex, alphauv_Tex).a;

            half4 c = lerp(mainTexColor, transColor, alphaVal) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Standard"
}