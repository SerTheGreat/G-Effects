Shader "Custom/GrayScale" {
Properties {
 _MainTex ("", 2D) = "white" {}
 _Magnitude("Magnitude", Float) = 0.0
}
 
SubShader {
 
 Pass{
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc" 
  
  uniform fixed _Magnitude;
    
  struct v2f {
   float4 pos : POSITION;
   half2 uv : TEXCOORD0;
  };
   
  v2f vert (appdata_img v){
   v2f o;
   o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
   o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
   return o; 
  }
    
  sampler2D _MainTex; //Reference in Pass is necessary to let us use this variable in shaders
    
  fixed4 frag (v2f i) : COLOR{
   fixed4 inColor = tex2D(_MainTex, i.uv); //Get the orginal rendered color 
     
   float average = (0.299*inColor.r + 0.587*inColor.g + 0.114*inColor.b)/3.0;
   fixed4 outColor = {lerp(inColor.r, average, _Magnitude), 
   				 lerp(inColor.g, average, _Magnitude),
   				 lerp(inColor.b, average, _Magnitude),
   				 inColor.a}; 
   fixed4(average, average, average, 1);
     
   return outColor;
  }
  ENDCG
 }
} 
 FallBack "Diffuse"
}