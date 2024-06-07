Shader "Racing Game Kit/Racing Line" {
Properties {
	_MainTex ("Racing Line Texture", 2D) = "white" {}
	_EmisColor ("Color", Color) = (.2,.2,.2,0)
	
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off ZWrite Off 
	
	Lighting On
	Material { Emission [_EmisColor] }
	ColorMaterial AmbientAndDiffuse

	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}