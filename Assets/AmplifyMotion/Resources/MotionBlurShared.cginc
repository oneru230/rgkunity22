// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFY_MOTION_BLUR_SHARED_INCLUDED
#define AMPLIFY_MOTION_BLUR_SHARED_INCLUDED

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _MainTex_TexelSize;

sampler2D _CameraDepthTexture;
float4 _CameraDepthTexture_TexelSize;

sampler2D _DepthTex;
float4 _DepthTex_TexelSize;
sampler2D _MotionTex;

half4 _AM_BLUR_STEP;
half2 _AM_DEPTH_THRESHOLD;

struct v2f
{
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o = ( v2f ) 0;
	o.pos = UnityObjectToClipPos( v.vertex );
	o.uv.xy = v.texcoord.xy;
	o.uv.zw = v.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
	if ( _MainTex_TexelSize.y < 0 )
		o.uv.w = 1 - o.uv.w;
#endif
	return o;
}

half4 frag_mobile( v2f i ) : SV_Target
{
	// 3-TAP
	half3 motion = tex2D( _MotionTex, i.uv.zw ).xyz;
	half4 color = tex2D( _MainTex, i.uv.xy );
	half4 accum = half4( color.xyz, 1 );

	half ref_depth = saturate( DecodeFloatRGBA( tex2D( _DepthTex, i.uv.xy ) ) );
	half ref_id = color.a;

	half id = floor( color.a * 255 + 0.5 );
	half ref_isobj = ( id > 1 ) * ( id < 254 );

	half2 dir_step = _AM_BLUR_STEP.xy * ( motion.xy * 2.0 - 1.0 ) * motion.z;

	half sample_depth0 = saturate( DecodeFloatRGBA( tex2D( _DepthTex, i.uv.xy - dir_step ) ) );
	half sample_depth1 = saturate( DecodeFloatRGBA( tex2D( _DepthTex, i.uv.xy + dir_step ) ) );

	half4 sample_color0 = tex2D( _MainTex, i.uv.xy - dir_step );
	half4 sample_color1 = tex2D( _MainTex, i.uv.xy + dir_step );

	half2 sample_depth = half2( sample_depth0, sample_depth1 );
	half2 sample_id = half2( sample_color0.a, sample_color1.a );

	half2 depth_test = sample_depth > ( ref_depth.xx - _AM_DEPTH_THRESHOLD.xx );
	half2 obj_test = ref_isobj.xx * ( sample_id == ref_id.xx );

	half2 sample_test = saturate( depth_test + obj_test );

	accum += sample_test.x * half4( sample_color0.xyz, 1 );
	accum += sample_test.y * half4( sample_color1.xyz, 1 );

	return half4( accum.xyz / accum.w, ref_id );
}

#endif
