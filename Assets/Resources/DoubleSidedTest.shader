Shader "Custom/DoubleSidedTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _Center ("Center", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 20
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
        Cull Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
            float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        float3 _Center;
        float _Radius;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
            half time = _Time.y;
            float rad = 20 + 5 * sin(time);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

            float d = distance(_Center, IN.worldPos);
            float dN = 1 - saturate(d / rad);
            if (dN > 0.25 && dN < 0.3)
                o.Albedo = half3(0.5,0.5,0.5);
//                o.Alpha = 0.25;
//            o.Albedo = half3(0.5,0.25,0.25);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
