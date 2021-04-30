Shader "Custom/Unlit/OutlineByPixelAlpha"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
		_OutlineWidth("outline Width", Range(0, 0.04)) =0.003
		_Brightness("Outline Brightness", Range(0,8)) = 3.2
	}
		SubShader
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha
			Tags { 
			
			 "Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"		
				}

			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 pos : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_TexelSize;
				float _OutlineWidth;
				float _Brightness;

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 _OutlineColor;
				fixed4 _Color;

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 c = tex2D(_MainTex, i.uv) * _Color;

				//fixed4 t = tex2D(_OutlineTex, float2(IN.texcoord.x + (_Time.x * _SpeedX), IN.texcoord.y + (_Time.x * _SpeedY))) * _Color;

					c.rgb *= c.a;

					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);

					half4 outlineC = _OutlineColor;
					//outlineC.a *= ceil(c.a);
					outlineC.rgb *= outlineC.a;

					fixed leftAlpha = tex2D(_MainTex, i.uv - float2(_OutlineWidth, 0)).a;
					fixed rightAlpha = tex2D(_MainTex, i.uv + float2(_OutlineWidth, 0)).a;								
					fixed downAlpha = tex2D(_MainTex, i.uv - float2(0, _OutlineWidth)).a;
					fixed upAlpha = tex2D(_MainTex, i.uv + float2(0, _OutlineWidth)).a;

					// then combine
					float result = max(max(leftAlpha, upAlpha), max(rightAlpha, downAlpha)) - c.a;
					// delete original alpha to only leave outline
					result *= (1 - c.a);
					// add color and brightness
					float4 outlines = result * _OutlineColor* _Brightness;

					result *= (1 - c.a);

					result += c.rgb;

					// show outlines +sprite
					c.rgb = c.rgb + outlines;
					//c = outlines;

					return c;

					/*
					fixed upRightAlpha = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)).a;
					fixed upLeftAlpha = tex2D(_MainTex, i.uv + fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)).a;
					fixed downLeftAlpha = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)).a;
					fixed downRightAlpha = tex2D(_MainTex, i.uv - fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)).a;
					*/

					//return lerp(c, outlineC, ceil(clamp(downAlpha + upAlpha + leftAlpha + rightAlpha, 0, 1)) - ceil(myAlpha));
				}
				ENDCG
			}
		}
}
