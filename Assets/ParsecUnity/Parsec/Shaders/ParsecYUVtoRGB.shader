Shader "Parsec/YUV to RGB"
{
    Properties
    {
        [HideInInspector]
        _Y("Texture", 2D) = "grey" {}
        [HideInInspector]
        _U("Texture", 2D) = "grey" {}
        [HideInInspector]
        _V("Texture", 2D) = "grey" {}
        [HideInInspector]
        _Padding("Padding", Vector) = (1,1,0,0)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Conversion function from: https://stackoverflow.com/a/54334776
            float3 YUVtoRGB(float3 yuv)
            {
                float3 rgb;

                rgb.r = yuv.x + yuv.z * 1.13983;
                rgb.g = yuv.x + dot(fixed2(-0.39465, -0.58060), yuv.yz);
                rgb.b = yuv.x + yuv.y * 2.03211;

                return rgb;
            }

            sampler2D _Y;
            sampler2D _U;
            sampler2D _V;
            float2 _Padding;

            fixed4 frag(v2f i) : SV_Target
            {
            #if SHADER_API_GLCORE
                float2 uv = float2(i.uv.x, 1 - i.uv.y);
            #else
                float2 uv = i.uv;
            #endif

                uv *= _Padding;

                float y = tex2D(_Y, uv).x;
                float u = tex2D(_U, uv).x - 0.5;
                float v = tex2D(_V, uv).x - 0.5;

                return float4(YUVtoRGB(float3(y, u, v)), 1);
            }
            ENDCG
        }
    }
}