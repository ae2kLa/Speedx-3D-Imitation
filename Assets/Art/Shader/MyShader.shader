Shader "Custom/MyShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        
        _OutColor("Out Color", Color) = (1,1,1,1)
        _InColor("In Color", Color) = (1,1,1,1)
        _InlineWidth("Inline Width", float) = 0.05

        _HorizontalBendStrength("Horizontal Bend Strength", Range(-1, 1)) = 0
        _VerticalBendStrength("Vertical Bend Strength", Range(-1, 0.15)) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Pass
        {
            blend srcalpha oneminussrcalpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 pos : POSITION;
                //float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                //fixed3 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform fixed4 _OutColor;
            uniform fixed4 _InColor;
            float _InlineWidth;

            float _HorizontalBendStrength;
            float _VerticalBendStrength;

            //parabola
            inline void BendModelByVertex(inout float4 vertex, float horizontal_strength, float vertical_strength)
            {
                // 8 is the distance from character to camera
                half originalz = _WorldSpaceCameraPos.z + 1;
                half dis = vertex.z - originalz;
                half disSqr = dis * dis;
                vertex.x = vertex.x - disSqr * 0.05 * horizontal_strength;
                vertex.y = vertex.y - disSqr * 0.05 * vertical_strength;
            }

            v2f vert(a2v v)
            {
                v2f o;
                //纹理采样
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                o.pos = UnityObjectToClipPos(v.pos);//从模型空间变换到裁剪空间

                //弯曲
                o.pos = mul(unity_ObjectToWorld, v.pos);//从模型空间变换到世界空间
                BendModelByVertex(o.pos, _HorizontalBendStrength, _VerticalBendStrength);//根据弯曲参数对世界坐标下的顶点偏移
                o.pos = mul(UNITY_MATRIX_VP, o.pos);//变换到裁剪空间

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 color = _OutColor;
                color +=
                    step(4,(
                        step(_InlineWidth, i.texcoord.x) +
                        step(i.texcoord.x, 1 - _InlineWidth) +
                        step(_InlineWidth, i.texcoord.y) +
                        step(i.texcoord.y, 1 - _InlineWidth))
                    )* _InColor;

                return fixed4(color, 1);
            }

            ENDCG
        }
    }
}
