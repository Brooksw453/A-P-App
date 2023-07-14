// Upgrade NOTE: upgraded instancing buffer 'NOT_LonelyNL_AlphaTestTileable' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NOT_Lonely/NL_AlphaTestTileable"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Scale", Float) = 1
		_MetallicGlossMap("Metallic (R) Occlusion (G) Gloss (A)", 2D) = "white" {}
		[HideInInspector]_OffsetX("OffsetX", Float) = 0
		[HideInInspector]_TilingXY("Tiling XY", Vector) = (1,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _BumpMap;
		uniform float _BumpScale;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform sampler2D _MetallicGlossMap;
		uniform float _Cutoff = 0.5;

		UNITY_INSTANCING_BUFFER_START(NOT_LonelyNL_AlphaTestTileable)
			UNITY_DEFINE_INSTANCED_PROP(float2, _TilingXY)
#define _TilingXY_arr NOT_LonelyNL_AlphaTestTileable
			UNITY_DEFINE_INSTANCED_PROP(float, _OffsetX)
#define _OffsetX_arr NOT_LonelyNL_AlphaTestTileable
		UNITY_INSTANCING_BUFFER_END(NOT_LonelyNL_AlphaTestTileable)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 _TilingXY_Instance = UNITY_ACCESS_INSTANCED_PROP(_TilingXY_arr, _TilingXY);
			float _OffsetX_Instance = UNITY_ACCESS_INSTANCED_PROP(_OffsetX_arr, _OffsetX);
			float2 appendResult10 = (float2(_OffsetX_Instance , 0.0));
			float2 uv_TexCoord7 = i.uv_texcoord * _TilingXY_Instance + appendResult10;
			o.Normal = UnpackScaleNormal( tex2D( _BumpMap, uv_TexCoord7 ), _BumpScale );
			float4 tex2DNode1 = tex2D( _MainTex, uv_TexCoord7 );
			o.Albedo = ( _Color * tex2DNode1 ).rgb;
			float4 tex2DNode6 = tex2D( _MetallicGlossMap, uv_TexCoord7 );
			o.Metallic = tex2DNode6.r;
			o.Smoothness = tex2DNode6.a;
			o.Occlusion = tex2DNode6.g;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
0;154;1920;940;1947.853;510.5687;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;11;-1398.612,-162.3315;Inherit;False;InstancedProperty;_OffsetX;OffsetX;6;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1202.612,-166.3315;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;13;-1333.853,-334.5687;Inherit;False;InstancedProperty;_TilingXY;Tiling XY;7;1;[HideInInspector];Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-999.406,-279.5541;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-764.4929,37.51246;Inherit;False;Property;_BumpScale;Normal Scale;4;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-462.493,-521.4876;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-546.4929,-313.4875;Inherit;True;Property;_MainTex;Albedo;1;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-192.493,-337.4876;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-560.6328,170.8212;Inherit;True;Property;_MetallicGlossMap;Metallic (R) Occlusion (G) Gloss (A);5;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-543.4929,-64.48744;Inherit;True;Property;_BumpMap;Normal Map;3;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;175,-52;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;NOT_Lonely/NL_AlphaTestTileable;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;11;0
WireConnection;7;0;13;0
WireConnection;7;1;10;0
WireConnection;1;1;7;0
WireConnection;3;0;2;0
WireConnection;3;1;1;0
WireConnection;6;1;7;0
WireConnection;4;1;7;0
WireConnection;4;5;5;0
WireConnection;0;0;3;0
WireConnection;0;1;4;0
WireConnection;0;3;6;1
WireConnection;0;4;6;4
WireConnection;0;5;6;2
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=B462C70394608A6ED21CFC574A8D18C97CD04328