//UVic CSC 305, 2019 Spring
//
//Helping lab for assignment02

Shader "Unlit/simpleShader"
{
	Properties
	{
		//_MainTex("Texture", 2D) = "white" {}
        _SnowTex("snow", 2D) = "white" {}
        _GrassTex("grass", 2D) = "white" {}
        _WaterTex("water", 2D) = "white" {}
        _SandTex("sand", 2D) = "white" {}
        _Thresholds("Height Threshold", vector) = (0,0,0,0)
        _SpecularPow("Specular Power", float) = 8
        _SpecularCoef("Specular Coefficient", float) = 0.06
        _AmbientLight("Ambient Light", vector) = (0.5,0.5,0.5,1)
        
	}

		SubShader
		{
			Tags { "RenderType" = "Opaque"
			"LightMode" = "ForwardBase" }
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

				//sampler2D _MainTex;
                sampler2D _GrassTex;
                sampler2D _WaterTex;
                sampler2D _SandTex;
                sampler2D _SnowTex;
				float4 _GrassTex_ST;
                float4 _WaterTex_ST;
                float4 _SandTex_ST;
                float4 _SnowTex_ST;
                float _SpecularPow;
                float _SpecularCoef;
                float4 _AmbientLight;
                float4 _Thresholds;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.height = v.vertex.y;
                    if (o.height < 0.3) o.uv = TRANSFORM_TEX(v.uv, _WaterTex);
                    else if (o.height < 0.4) o.uv = TRANSFORM_TEX(v.uv, _SandTex);
                    else if (o.height < 0.8) o.uv = TRANSFORM_TEX(v.uv, _GrassTex);
                    else o.uv = TRANSFORM_TEX(v.uv, _SnowTex);
					//o.uv = TRANSFORM_TEX(v.uv, _GrassTex);
					o.normal = UnityObjectToWorldNormal(v.normal);
					o.worldpos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{

					float4 fragment_color = float4(0,0,0,0);
					if (i.height < 0.25) fragment_color = tex2D(_WaterTex, i.uv);
					else if (i.height < 0.3) fragment_color = tex2D(_SandTex, i.uv);
					else if (i.height < 0.75) fragment_color = tex2D(_GrassTex, i.uv);
					else fragment_color = tex2D(_SnowTex, i.uv);
                    
                     //ambient
                     float4 albedo = fragment_color;
                     fragment_color = _AmbientLight * albedo;
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