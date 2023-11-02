Shader "Custom/CubeOutline"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _Width("Width", Range(0, 0.5)) = 0.1
    }

        SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass
        {

            //如果要显示背面的线框，取消下面两个注释即可
            //cull off
            //ZWrite off
            blend srcalpha oneminussrcalpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float4 _OutlineColor;
            float _Width;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = half4(0, 0, 0, 0);
                half2 border = half2(_Width, 1.0 - _Width);

                if (i.uv.x < border.x || i.uv.x > border.y || i.uv.y < border.x || i.uv.y > border.y)
                {
                    col = _OutlineColor; // 边缘部分使用描边颜色
                }
                else
                {
                    col = _Color; // 内部部分使用填充颜色
                }

                return col;
            }
            ENDCG
        }
    }
}
//这个Shader能不能改成距离相机越远则物体颜色越暗，离玩家越近则物体颜色越亮。