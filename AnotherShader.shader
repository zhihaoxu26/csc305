Shader "Custom/AnotherShader"
{
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_Tex1("Texture1", 2D) = "white" {}
		_Tex2("Texture2", 2D) = "white" {}
		_Tex3("Texture3", 2D) = "white" {}
		_Thresholds("Height Threshold", vector) = (0,0,0,0)
		_SpecularPow("Specular Power", float) = 1
		_SpecularCoef("Specular Coefficient", float) = 1
		_AmbientLight("Ambient Light", vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;

			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float height : TEXCOORD1;
				float3 normal : TEXCOORD2;
				float4 worldpos : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			float4 _MainTex_ST;
			float _SpecularPow;
			float _SpecularCoef;
			float4 _AmbientLight;
			float4 _Thresholds;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.height = v.vertex.y;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldpos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			//sampler2d _mainTex;
			fixed4 frag(v2f i) : SV_Target
			{
				float4 col0 = tex2D(_MainTex, i.uv);
				float4 col1 = tex2D(_Tex1, i.uv);
				float4 col2 = tex2D(_Tex2, i.uv);
				float4 col3 = tex2D(_Tex3, i.uv);

				float4 albedo = 0;

				if (i.height < _Thresholds.x) albedo = col0;
				else if (i.height < _Thresholds.y) albedo = col1;
				else if (i.height < _Thresholds.z) albedo = col3;
				else albedo = col3;
				//ambient
				float4 fragment_color = _AmbientLight * albedo;
				fragment_color.w = 1;
				//diffuse
				float3 L = normalize(_WorldSpaceLightPos0.xyz);
				float3 N = normalize(i.normal);
				float d = dot(L, N);

				if (d > 0)
				{
					fragment_color.xyz += _LightColor0.xyz * d * albedo.xyz;
				}

				// Bilnn-Phong Specular
				float3 V = _WorldSpaceCameraPos.xyz - i.worldpos.xyz;
				V = normalize(V);
				float3 H = (V + L) / 2;
				d = dot(H, N);
				if (d > 0) {
					float specular = pow(d, _SpecularPow) * _SpecularCoef;
					fragment_color.xyz += _LightColor0.xyz * specular;
				}
				return fragment_color; 
			}
			ENDCG
		}
		}
}
