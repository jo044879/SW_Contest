Shader "Custom/SkyboxBlend"
{
    Properties
    {
        _DayTex ("Day (Lat-Long)", 2D) = "grey" {}
        _EveningTex ("Evening (Lat-Long)", 2D) = "grey" {}
        _NightTex ("Night (Lat-Long)", 2D) = "grey" {}
        _Blend ("Blend (0=Day, 1=Night)", Range(0,1)) = 0
        _EveningPos ("Evening Position", Range(0.1,0.9)) = 0.6
        _Exposure ("Exposure", Range(0,8)) = 1
        _Rotation ("Rotation", Range(0,360)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _DayTex;     half4 _DayTex_HDR;
            sampler2D _EveningTex; half4 _EveningTex_HDR;
            sampler2D _NightTex;   half4 _NightTex_HDR;
            half _Blend, _EveningPos, _Exposure;
            float _Rotation;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float a = _Rotation * UNITY_PI / 180.0;
                float s, c; sincos(a, s, c);
                float3 p = v.vertex.xyz;
                o.dir = float3(c * p.x - s * p.z, p.y, s * p.x + c * p.z);
                return o;
            }

            float2 ToLatLong (float3 d)
            {
                float3 n = normalize(d);
                float lat = acos(n.y);
                float lon = atan2(n.z, n.x);
                return float2(0.5, 1.0) - float2(lon, lat) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 uv = ToLatLong(i.dir);
                half3 day     = DecodeHDR(tex2Dlod(_DayTex,     float4(uv, 0, 0)), _DayTex_HDR);
                half3 evening = DecodeHDR(tex2Dlod(_EveningTex, float4(uv, 0, 0)), _EveningTex_HDR);
                half3 night   = DecodeHDR(tex2Dlod(_NightTex,   float4(uv, 0, 0)), _NightTex_HDR);

                half3 col;
                if (_Blend < _EveningPos)
                {
                    float k = smoothstep(0.0, 1.0, _Blend / _EveningPos);
                    col = lerp(day, evening, k);
                }
                else
                {
                    float k = smoothstep(0.0, 1.0, (_Blend - _EveningPos) / (1.0 - _EveningPos));
                    col = lerp(evening, night, k);
                }
                return half4(col * _Exposure, 1);
            }
            ENDCG
        }
    }
}