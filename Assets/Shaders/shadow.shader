Shader "Transparent/Shadow" {
	Properties {
		_Color ("Main Color", Color) = (0.0, 0.0, 0.0, 0.3)
	}

	SubShader {
		Tags {"Queue"="Transparent+10" "RenderType"="Transparent"}
		LOD 200

		ColorMask RGBA
		Cull Off
		ZWrite On
		ZTest Less
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

		Color[_Color]

		Pass {

		}

		//UsePass "Transparent/Diffuse/FORWARD"
	}
}