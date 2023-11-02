Shader "Custom/Curved"
{
    Properties
    {
        _BendAmount("Bend Amount", Range(-1, 1)) = 0.5
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
            };

            float _BendAmount;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Apply bending based on _BendAmount here
                o.pos.x += _BendAmount * v.vertex.z; // Adjust X position based on bend amount
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(1, 1, 1, 1); // Basic white color
            }
            ENDCG
        }
    }
}
