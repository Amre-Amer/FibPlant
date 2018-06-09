Shader "Custom/Wavy" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Param1 ("Wave Params 1", Vector) = (6.0, 2.0, 0.12, 0.0)
        _Param2 ("Wave Params 2", Vector) = (12.0, 10.0, 0.03, 0.0)
        _Radius ("Radius", Float) = 20
        _Center ("Center", Vector) = (0,0,0,0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        half4 _Param1;
        half4 _Param2;
        float _Radius;
        float3 _Center;

        void vert (inout appdata_full v) {
            half time = _Time.y;
            half u = v.texcoord.x;
            half w1 = sin(time * _Param1.x - u * _Param1.y) * _Param1.z;
            half w2 = sin(time * _Param2.x - u * _Param2.y) * _Param2.z;
//            v.vertex.xyz += v.normal * (w1 + w2) * u;
            v.vertex.xyz += v.normal * _Radius;
        }

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;

            half time = _Time.y;
            float rad = 50 + 5 * sin(time);
            float d = distance(_Center, IN.worldPos);
            float dN = 1 - saturate(d / rad);
            for (int c = 0; c < 4; c++) {
                float s1 = c * 0.25;
                float s2 = s1 + 0.05;
                if (dN > s1 && dN < s2)
                   o.Albedo = half3(0.2,0.5,0.5);
            }
        }
        ENDCG
    } 
    FallBack "Diffuse"
    CustomEditor "WavyAnimatedMaterialInspector"
}