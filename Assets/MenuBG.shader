// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/MenuBG"
{
    Properties
    {
    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0) {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv - float2(0.5,0.5);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                i.uv *= 16 / 9.0;
                i.uv *= 2;


                float d = length(i.uv);
                d = smoothstep(0.1,0.8,d);

                d = 0.01 / abs(sin(10*d + _Time.y + 0.1 * i.uv.x / i.uv.y));

                float3 colour = float3(d / 1.35, 2 * d/5, 2 * d) * (1.5+cos(_Time.y*0.25));
                return half4(colour,1);
            }
            ENDCG
        }
    }
}
