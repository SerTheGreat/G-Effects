// Compiled shader for all platforms, uncompressed size: 17.1KB

Shader "Custom/GrayScale" {
Properties {
 _MainTex ("", 2D) = "white" {}
 _Magnitude ("Magnitude", Float) = 0
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //    d3d11_9x : 6 math
 //        d3d9 : 8 math
 //        gles : 7 math, 1 texture
 //       gles3 : 7 math, 1 texture
 //   glesdesktop : 7 math, 1 texture
 //       metal : 3 math
 //      opengl : 8 math
 // Stats for Fragment shader:
 //       d3d11 : 3 math, 1 texture
 //    d3d11_9x : 3 math, 1 texture
 //        d3d9 : 11 math, 1 texture
 //       metal : 7 math, 1 texture
 //      opengl : 11 math, 1 texture
 Pass {
Program "vp" {
SubProgram "opengl " {
// Stats: 8 math
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"!!ARBvp1.0
PARAM c[9] = { { 0 },
		state.matrix.mvp,
		state.matrix.texture[0] };
TEMP R0;
MOV R0.zw, c[0].x;
MOV R0.xy, vertex.texcoord[0];
DP4 result.texcoord[0].y, R0, c[6];
DP4 result.texcoord[0].x, R0, c[5];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 8 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
// Stats: 8 math
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
"vs_2_0
def c8, 0.00000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.zw, c8.x
mov r0.xy, v1
dp4 oT0.y, r0, c5
dp4 oT0.x, r0, c4
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "d3d11 " {
// Stats: 6 math
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
ConstBuffer "UnityPerDrawTexMatrices" 768
Matrix 512 [glstate_matrix_texture0]
BindCB  "UnityPerDraw" 0
BindCB  "UnityPerDrawTexMatrices" 1
"vs_4_0
eefiecedjlfomejbofdklfcgafioaaodagpgfnjcabaaaaaaciacaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklklfdeieefceiabaaaa
eaaaabaafcaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafjaaaaaeegiocaaa
abaaaaaaccaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagiaaaaac
abaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaaaaaaaaa
abaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaaaaaaaaaagbabaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaa
acaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaa
egiocaaaaaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaai
dcaabaaaaaaaaaaafgbfbaaaabaaaaaaegiacaaaabaaaaaacbaaaaaadcaaaaak
dccabaaaabaaaaaaegiacaaaabaaaaaacaaaaaaaagbabaaaabaaaaaaegaabaaa
aaaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
// Stats: 6 math
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
ConstBuffer "UnityPerDrawTexMatrices" 768
Matrix 512 [glstate_matrix_texture0]
BindCB  "UnityPerDraw" 0
BindCB  "UnityPerDrawTexMatrices" 1
"vs_4_0_level_9_1
eefiecedkjfdmdegebiaplcnddhiannceeeaamhgabaaaaaaceadaaaaaeaaaaaa
daaaaaaaciabaaaahiacaaaammacaaaaebgpgodjpaaaaaaapaaaaaaaaaacpopp
laaaaaaaeaaaaaaaacaaceaaaaaadmaaaaaadmaaaaaaceaaabaadmaaaaaaaaaa
aeaaabaaaaaaaaaaabaacaaaacaaafaaaaaaaaaaaaaaaaaaaaacpoppbpaaaaac
afaaaaiaaaaaapjabpaaaaacafaaabiaabaaapjaafaaaaadaaaaadiaabaaffja
agaaoekaaeaaaaaeaaaaadoaafaaoekaabaaaajaaaaaoeiaafaaaaadaaaaapia
aaaaffjaacaaoekaaeaaaaaeaaaaapiaabaaoekaaaaaaajaaaaaoeiaaeaaaaae
aaaaapiaadaaoekaaaaakkjaaaaaoeiaaeaaaaaeaaaaapiaaeaaoekaaaaappja
aaaaoeiaaeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaamma
aaaaoeiappppaaaafdeieefceiabaaaaeaaaabaafcaaaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafjaaaaaeegiocaaaabaaaaaaccaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadiaaaaaidcaabaaaaaaaaaaafgbfbaaaabaaaaaa
egiacaaaabaaaaaacbaaaaaadcaaaaakdccabaaaabaaaaaaegiacaaaabaaaaaa
caaaaaaaagbabaaaabaaaaaaegaabaaaaaaaaaaadoaaaaabejfdeheoemaaaaaa
acaaaaaaaiaaaaaadiaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaa
ebaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaafaepfdejfeejepeo
aafeeffiedepepfceeaaklklepfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadamaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfcee
aaklklkl"
}
SubProgram "gles " {
// Stats: 7 math, 1 textures
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_texture0;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  highp vec2 tmpvar_3;
  highp vec2 inUV_4;
  inUV_4 = tmpvar_1;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = inUV_4;
  tmpvar_3 = (glstate_matrix_texture0 * tmpvar_5).xy;
  tmpvar_2 = tmpvar_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform lowp float _Magnitude;
uniform sampler2D _MainTex;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 outColor_1;
  highp float average_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp float tmpvar_4;
  tmpvar_4 = (((
    (0.299 * tmpvar_3.x)
   + 
    (0.587 * tmpvar_3.y)
  ) + (0.114 * tmpvar_3.z)) / 3.0);
  average_2 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.xyz = mix (tmpvar_3.xyz, vec3(average_2), vec3(_Magnitude));
  tmpvar_5.w = tmpvar_3.w;
  outColor_1 = tmpvar_5;
  gl_FragData[0] = outColor_1;
}



#endif"
}
SubProgram "flash " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
"agal_vs
c8 0.0 0.0 0.0 0.0
[bc]
aaaaaaaaaaaaamacaiaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r0.zw, c8.x
aaaaaaaaaaaaadacadaaaaoeaaaaaaaaaaaaaaaaaaaaaaaa mov r0.xy, a3
bdaaaaaaaaaaacaeaaaaaaoeacaaaaaaafaaaaoeabaaaaaa dp4 v0.y, r0, c5
bdaaaaaaaaaaabaeaaaaaaoeacaaaaaaaeaaaaoeabaaaaaa dp4 v0.x, r0, c4
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
"
}
SubProgram "glesdesktop " {
// Stats: 7 math, 1 textures
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_texture0;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  highp vec2 tmpvar_3;
  highp vec2 inUV_4;
  inUV_4 = tmpvar_1;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = inUV_4;
  tmpvar_3 = (glstate_matrix_texture0 * tmpvar_5).xy;
  tmpvar_2 = tmpvar_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform lowp float _Magnitude;
uniform sampler2D _MainTex;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 outColor_1;
  highp float average_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp float tmpvar_4;
  tmpvar_4 = (((
    (0.299 * tmpvar_3.x)
   + 
    (0.587 * tmpvar_3.y)
  ) + (0.114 * tmpvar_3.z)) / 3.0);
  average_2 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.xyz = mix (tmpvar_3.xyz, vec3(average_2), vec3(_Magnitude));
  tmpvar_5.w = tmpvar_3.w;
  outColor_1 = tmpvar_5;
  gl_FragData[0] = outColor_1;
}



#endif"
}
SubProgram "gles3 " {
// Stats: 7 math, 1 textures
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_texture0;
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  highp vec2 tmpvar_3;
  highp vec2 inUV_4;
  inUV_4 = tmpvar_1;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = inUV_4;
  tmpvar_3 = (glstate_matrix_texture0 * tmpvar_5).xy;
  tmpvar_2 = tmpvar_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform lowp float _Magnitude;
uniform sampler2D _MainTex;
in mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 outColor_1;
  highp float average_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture (_MainTex, xlv_TEXCOORD0);
  lowp float tmpvar_4;
  tmpvar_4 = (((
    (0.299 * tmpvar_3.x)
   + 
    (0.587 * tmpvar_3.y)
  ) + (0.114 * tmpvar_3.z)) / 3.0);
  average_2 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.xyz = mix (tmpvar_3.xyz, vec3(average_2), vec3(_Magnitude));
  tmpvar_5.w = tmpvar_3.w;
  outColor_1 = tmpvar_5;
  _glesFragData[0] = outColor_1;
}



#endif"
}
SubProgram "metal " {
// Stats: 3 math
Bind "vertex" ATTR0
Bind "texcoord" ATTR1
ConstBuffer "$Globals" 128
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [glstate_matrix_texture0]
"metal_vs
#include <metal_stdlib>
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float4 _glesMultiTexCoord0 [[attribute(1)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half2 xlv_TEXCOORD0;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  float4x4 glstate_matrix_texture0;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half2 tmpvar_1;
  tmpvar_1 = half2(_mtl_i._glesMultiTexCoord0.xy);
  half2 tmpvar_2;
  float2 tmpvar_3;
  float2 inUV_4;
  inUV_4 = float2(tmpvar_1);
  float4 tmpvar_5;
  tmpvar_5.zw = float2(0.0, 0.0);
  tmpvar_5.xy = inUV_4;
  tmpvar_3 = (_mtl_u.glstate_matrix_texture0 * tmpvar_5).xy;
  tmpvar_2 = half2(tmpvar_3);
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_TEXCOORD0 = tmpvar_2;
  return _mtl_o;
}

"
}
}
Program "fp" {
SubProgram "opengl " {
// Stats: 11 math, 1 textures
Float 0 [_Magnitude]
SetTexture 0 [_MainTex] 2D 0
"!!ARBfp1.0
PARAM c[2] = { program.local[0],
		{ 0.11401367, 0.29907227, 0.58691406, 0.33333334 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MUL R1.x, R0.y, c[1].z;
MAD R1.x, R0, c[1].y, R1;
MAD R1.x, R0.z, c[1], R1;
MAD R1.y, R1.x, c[1].w, -R0.x;
MAD result.color.x, R1.y, c[0], R0;
MAD R0.x, R1, c[1].w, -R0.y;
MAD R1.x, R1, c[1].w, -R0.z;
MAD result.color.y, R0.x, c[0].x, R0;
MAD result.color.z, R1.x, c[0].x, R0;
MOV result.color.w, R0;
END
# 11 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
// Stats: 11 math, 1 textures
Float 0 [_Magnitude]
SetTexture 0 [_MainTex] 2D 0
"ps_2_0
dcl_2d s0
def c1, 0.58691406, 0.29907227, 0.11401367, 0.33333334
dcl t0.xy
texld r3, t0, s0
mul_pp r0.x, r3.y, c1
mad_pp r0.x, r3, c1.y, r0
mad_pp r0.x, r3.z, c1.z, r0
mad_pp r2.x, r0, c1.w, -r3
mad_pp r1.x, r0, c1.w, -r3.y
mad_pp r0.x, r0, c1.w, -r3.z
mad_pp r2.x, r2, c0, r3
mad_pp r2.y, r1.x, c0.x, r3
mov_pp r2.w, r3
mad_pp r2.z, r0.x, c0.x, r3
mov_pp oC0, r2
"
}
SubProgram "d3d11 " {
// Stats: 3 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer "$Globals" 32
Float 16 [_Magnitude]
BindCB  "$Globals" 0
"ps_4_0
eefiecedncgiodibdhgjdojdfjooplgbmjclmjofabaaaaaaneabaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbeabaaaa
eaaaaaaaefaaaaaafjaaaaaeegiocaaaaaaaaaaaacaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaabaaaaaakbcaabaaa
abaaaaaaegacbaaaaaaaaaaaaceaaaaaihbgjjdokcefbgdpnfhiojdnaaaaaaaa
dcaaaaanhcaabaaaabaaaaaaagaabaaaabaaaaaaaceaaaaaklkkkkdoklkkkkdo
klkkkkdoaaaaaaaaegacbaiaebaaaaaaaaaaaaaadcaaaaakhccabaaaaaaaaaaa
agiacaaaaaaaaaaaabaaaaaaegacbaaaabaaaaaaegacbaaaaaaaaaaadgaaaaaf
iccabaaaaaaaaaaadkaabaaaaaaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
// Stats: 3 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer "$Globals" 32
Float 16 [_Magnitude]
BindCB  "$Globals" 0
"ps_4_0_level_9_1
eefiecedicmjhpgphioiffnjccpichelbbagpeniabaaaaaabiadaaaaaeaaaaaa
daaaaaaahaabaaaaimacaaaaoeacaaaaebgpgodjdiabaaaadiabaaaaaaacpppp
aeabaaaadeaaaaaaabaaciaaaaaadeaaaaaadeaaabaaceaaaaaadeaaaaaaaaaa
aaaaabaaabaaaaaaaaaaaaaaaaacppppfbaaaaafabaaapkakcefbgdpihbgjjdo
nfhiojdnklkkkkdobpaaaaacaaaaaaiaaaaacdlabpaaaaacaaaaaajaaaaiapka
ecaaaaadaaaacpiaaaaaoelaaaaioekaafaaaaadabaaciiaaaaaffiaabaaaaka
aeaaaaaeabaacbiaaaaaaaiaabaaffkaabaappiaaeaaaaaeabaacbiaaaaakkia
abaakkkaabaaaaiaaeaaaaaeabaaaciaabaaaaiaabaappkaaaaaaaibaeaaaaae
aaaacbiaaaaaaakaabaaffiaaaaaaaiaaeaaaaaeabaaaciaabaaaaiaabaappka
aaaaffibaeaaaaaeabaaabiaabaaaaiaabaappkaaaaakkibaeaaaaaeaaaaceia
aaaaaakaabaaaaiaaaaakkiaaeaaaaaeaaaacciaaaaaaakaabaaffiaaaaaffia
abaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcbeabaaaaeaaaaaaaefaaaaaa
fjaaaaaeegiocaaaaaaaaaaaacaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaabaaaaaakbcaabaaaabaaaaaaegacbaaa
aaaaaaaaaceaaaaaihbgjjdokcefbgdpnfhiojdnaaaaaaaadcaaaaanhcaabaaa
abaaaaaaagaabaaaabaaaaaaaceaaaaaklkkkkdoklkkkkdoklkkkkdoaaaaaaaa
egacbaiaebaaaaaaaaaaaaaadcaaaaakhccabaaaaaaaaaaaagiacaaaaaaaaaaa
abaaaaaaegacbaaaabaaaaaaegacbaaaaaaaaaaadgaaaaaficcabaaaaaaaaaaa
dkaabaaaaaaaaaaadoaaaaabejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
SubProgram "gles " {
"!!GLES"
}
SubProgram "flash " {
Float 0 [_Magnitude]
SetTexture 0 [_MainTex] 2D 0
"agal_ps
c1 0.586914 0.299072 0.114014 0.333333
[bc]
ciaaaaaaadaaapacaaaaaaoeaeaaaaaaaaaaaaaaafaababb tex r3, v0, s0 <2d wrap linear point>
adaaaaaaaaaaabacadaaaaffacaaaaaaabaaaaoeabaaaaaa mul r0.x, r3.y, c1
adaaaaaaaaaaacacadaaaaaaacaaaaaaabaaaaffabaaaaaa mul r0.y, r3.x, c1.y
abaaaaaaaaaaabacaaaaaaffacaaaaaaaaaaaaaaacaaaaaa add r0.x, r0.y, r0.x
adaaaaaaabaaabacadaaaakkacaaaaaaabaaaakkabaaaaaa mul r1.x, r3.z, c1.z
abaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaacaaaaaa add r0.x, r1.x, r0.x
adaaaaaaabaaacacaaaaaaaaacaaaaaaabaaaappabaaaaaa mul r1.y, r0.x, c1.w
acaaaaaaacaaabacabaaaaffacaaaaaaadaaaaaaacaaaaaa sub r2.x, r1.y, r3.x
adaaaaaaaeaaabacaaaaaaaaacaaaaaaabaaaappabaaaaaa mul r4.x, r0.x, c1.w
acaaaaaaabaaabacaeaaaaaaacaaaaaaadaaaaffacaaaaaa sub r1.x, r4.x, r3.y
adaaaaaaaeaaabacaaaaaaaaacaaaaaaabaaaappabaaaaaa mul r4.x, r0.x, c1.w
acaaaaaaaaaaabacaeaaaaaaacaaaaaaadaaaakkacaaaaaa sub r0.x, r4.x, r3.z
adaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaoeabaaaaaa mul r2.x, r2.x, c0
abaaaaaaacaaabacacaaaaaaacaaaaaaadaaaaaaacaaaaaa add r2.x, r2.x, r3.x
adaaaaaaacaaacacabaaaaaaacaaaaaaaaaaaaaaabaaaaaa mul r2.y, r1.x, c0.x
abaaaaaaacaaacacacaaaaffacaaaaaaadaaaaffacaaaaaa add r2.y, r2.y, r3.y
aaaaaaaaacaaaiacadaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r2.w, r3.w
adaaaaaaacaaaeacaaaaaaaaacaaaaaaaaaaaaaaabaaaaaa mul r2.z, r0.x, c0.x
abaaaaaaacaaaeacacaaaakkacaaaaaaadaaaakkacaaaaaa add r2.z, r2.z, r3.z
aaaaaaaaaaaaapadacaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r2
"
}
SubProgram "glesdesktop " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
SubProgram "metal " {
// Stats: 7 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer "$Globals" 2
ScalarHalf 0 [_Magnitude]
"metal_fs
#include <metal_stdlib>
using namespace metal;
struct xlatMtlShaderInput {
  half2 xlv_TEXCOORD0;
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
  half _Magnitude;
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]]
  ,   texture2d<half> _MainTex [[texture(0)]], sampler _mtlsmp__MainTex [[sampler(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 outColor_1;
  float average_2;
  half4 tmpvar_3;
  tmpvar_3 = _MainTex.sample(_mtlsmp__MainTex, (float2)(_mtl_i.xlv_TEXCOORD0));
  half tmpvar_4;
  tmpvar_4 = (((
    ((half)0.299 * tmpvar_3.x)
   + 
    ((half)0.587 * tmpvar_3.y)
  ) + ((half)0.114 * tmpvar_3.z)) / (half)3.0);
  average_2 = float(tmpvar_4);
  float4 tmpvar_5;
  tmpvar_5.xyz = mix ((float3)tmpvar_3.xyz, float3(average_2), (float3)half3(_mtl_u._Magnitude));
  tmpvar_5.w = float(tmpvar_3.w);
  outColor_1 = half4(tmpvar_5);
  _mtl_o._glesFragData_0 = outColor_1;
  return _mtl_o;
}

"
}
}
 }
}
Fallback "Diffuse"
}