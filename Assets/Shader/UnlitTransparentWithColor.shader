Shader "Custom/LineRendererWithOffset"  
{  
    Properties  
    {  
        _MainTex ("Texture", 2D) = "white" {}              // ����  
        _MainTex_ST ("Tiling/Offset", Vector) = (1,1,0,0)  // ����ƽ�̺�ƫ��  
        _TintColor ("Tint Color", Color) = (1,1,1,1)       // ȫ����ɫ��ɫ  
    }  
    SubShader  
    {  
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }  
        LOD 100  

        Pass  
        {  
            Blend SrcAlpha OneMinusSrcAlpha   // ����͸���Ȼ��  
            ZWrite Off                        // ��ֹ���д��  
            Cull Off                          // ��Ⱦ˫�棬���޳��κ���  

            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  

            #include "UnityCG.cginc"  

            struct appdata_t  
            {  
                float4 vertex : POSITION;    // ����λ��  
                float4 color : COLOR;        // ������ɫ������ LineRenderer �Ķ�̬��ɫ��  
                float2 texcoord : TEXCOORD0; // ��������  
            };  

            struct v2f  
            {  
                float4 vertex : POSITION;    // ��������  
                float4 color : COLOR;        // ������ɫ  
                float2 texcoord : TEXCOORD0; // ��������  
            };  

            sampler2D _MainTex;  
            float4 _MainTex_ST;              // ƽ�̺�ƫ�Ʋ���  
            fixed4 _TintColor;               // ȫ����ɫ����  

            // ������ɫ��  
            v2f vert (appdata_t v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);                     // ת�������ÿռ�  
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);              // Ӧ��ƽ�̺�ƫ��  
                o.color = v.color;                                            // ������ɫ������ LineRenderer ���ã�  
                return o;  
            }  

            // Ƭ����ɫ��  
            fixed4 frag (v2f i) : SV_Target  
            {  
                fixed4 texColor = tex2D(_MainTex, i.texcoord);                 // ��ȡ������ɫ  
                fixed4 outputColor = texColor * i.color * _TintColor;         // ����������ɫ��������ɫ��ȫ����ɫ  
                return outputColor;                                           // ���������ɫ  
            }  
            ENDCG  
        }  
    }  
}  