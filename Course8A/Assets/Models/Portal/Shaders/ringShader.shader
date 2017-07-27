Shader "Custom/ringShader"
{
	Properties
	{
		_NoiseTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				//float3 worldPos;
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _NoiseTex;
			float4 _Color;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex) + float4(v.normal.xy, 0, 0) * (0.05) *  sin(tex2Dlod(_NoiseTex, v.vertex).r * _Time[1]);
				o.normal = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_NoiseTex, i.uv) * _Color;

				return col;
			}
			ENDCG
		}
	}
}
