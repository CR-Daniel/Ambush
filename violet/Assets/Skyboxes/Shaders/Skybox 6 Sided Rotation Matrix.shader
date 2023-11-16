﻿// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Skybox/6 Sided Rotation Matrix" {
	Properties{
		_Tint("Tint Color", Color) = (.5, .5, .5, .5)
		[Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
		_RotationX("RotationX", Range(0, 360)) = 0
		_RotationY("RotationY", Range(0, 360)) = 0
		_RotationZ("RotationZ", Range(0, 360)) = 0
		[NoScaleOffset] _FrontTex("Front [+Z]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _BackTex("Back [-Z]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _LeftTex("Left [+X]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _RightTex("Right [-X]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _UpTex("Up [+Y]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _DownTex("Down [-Y]   (HDR)", 2D) = "grey" {}
	}

		SubShader{
		Tags{ "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Cull Off ZWrite Off

		CGINCLUDE
#include "UnityCG.cginc"

		half4 _Tint;
	half _Exposure;
	float _RotationX;
	float _RotationY;
	float _RotationZ;

	float4x4 rotate(float3 r, float4 d) // r=rotations axes
	{
		float cx, cy, cz, sx, sy, sz;
		sincos(r.x, sx, cx);
		sincos(r.y, sy, cy);
		sincos(r.z, sz, cz);
		return float4x4(cy*cz, -sz, sy, d.x,
			sz, cx*cz, -sx, d.y,
			-sy, sx, cx*cy, d.z,
			0, 0, 0, d.w);
	}

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};
	struct v2f {
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};
	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		float3 rotation = float3(_RotationX, _RotationY, _RotationZ) * UNITY_PI / 180.0;
		float4x4 rotationMatrix = rotate(rotation, float4(0, 0, 0, 1));
		float3 rotated = mul(rotationMatrix, v.vertex);
		o.vertex = UnityObjectToClipPos(rotated);
		o.texcoord = v.texcoord;
		return o;
	}
	half4 skybox_frag(v2f i, sampler2D smp, half4 smpDecode)
	{
		half4 tex = tex2D(smp, i.texcoord);
		half3 c = DecodeHDR(tex, smpDecode);
		c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
		c *= _Exposure;
		return half4(c, 1);
	}
	ENDCG

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
		sampler2D _FrontTex;
	half4 _FrontTex_HDR;
	half4 frag(v2f i) : SV_Target{ return skybox_frag(i,_FrontTex, _FrontTex_HDR); }
		ENDCG
	}
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
		sampler2D _BackTex;
	half4 _BackTex_HDR;
	half4 frag(v2f i) : SV_Target{ return skybox_frag(i,_BackTex, _BackTex_HDR); }
		ENDCG
	}
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
		sampler2D _LeftTex;
	half4 _LeftTex_HDR;
	half4 frag(v2f i) : SV_Target{ return skybox_frag(i,_LeftTex, _LeftTex_HDR); }
		ENDCG
	}
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
		sampler2D _RightTex;
	half4 _RightTex_HDR;
	half4 frag(v2f i) : SV_Target{ return skybox_frag(i,_RightTex, _RightTex_HDR); }
		ENDCG
	}
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
		sampler2D _UpTex;
	half4 _UpTex_HDR;
	half4 frag(v2f i) : SV_Target{ return skybox_frag(i,_UpTex, _UpTex_HDR); }
		ENDCG
	}
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
		sampler2D _DownTex;
	half4 _DownTex_HDR;
	half4 frag(v2f i) : SV_Target{ return skybox_frag(i,_DownTex, _DownTex_HDR); }
		ENDCG
	}
	}
}