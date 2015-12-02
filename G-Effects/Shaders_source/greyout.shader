Shader "G-Effects/Greyout" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Magnitude ("Magnitude", Range (0, 1)) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
 
			#include "UnityCG.cginc"
 
			uniform sampler2D _MainTex;
			uniform float _Magnitude;
 
			float4 frag(v2f_img input) : COLOR {
				float4 color = tex2D(_MainTex, input.uv);
				
				float average = 0.299*color.r + 0.587*color.g + 0.114*color.b;
				float3 greycolor = float3(average, average, average); 
				
				float4 result = color;
				result.rgb = lerp(color.rgb, greycolor, _Magnitude);
				return result;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}