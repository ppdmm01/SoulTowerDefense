Shader "Custom/FlashShader"  
{  
    Properties  
    {  
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}  
        _FlashColor ("Flash Color", Color) = (1,1,1,1)  // 颜色属性  
        _FlashAmount ("Flash Amount", Range(0,1)) = 0  
    }  
    
    SubShader  
    {  
        Tags {   
            "Queue"="Transparent"   
            "RenderType"="Transparent"  
            "PreviewType"="Plane"  
            "CanUseSpriteAtlas"="True"  
        }  
        
        Pass  
        {  
            Blend SrcAlpha OneMinusSrcAlpha  
            Cull Off  
            ZWrite Off  
            
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #pragma multi_compile_instancing  
            
            #include "UnityCG.cginc"  

            struct appdata  
            {  
                float4 vertex : POSITION;  
                float2 uv : TEXCOORD0;  
                fixed4 color : COLOR;  
                UNITY_VERTEX_INPUT_INSTANCE_ID  
            };  

            struct v2f  
            {  
                float2 uv : TEXCOORD0;  
                float4 vertex : SV_POSITION;  
                fixed4 color : COLOR;  
                UNITY_VERTEX_OUTPUT_STEREO   
            };  

            sampler2D _MainTex;  
            float4 _MainTex_ST;  
            fixed4 _FlashColor;  // 声明颜色变量  
            
            UNITY_INSTANCING_BUFFER_START(Props)  
                UNITY_DEFINE_INSTANCED_PROP(fixed, _FlashAmount)  
            UNITY_INSTANCING_BUFFER_END(Props)  
            
            v2f vert (appdata v)  
            {  
                v2f o;  
                UNITY_SETUP_INSTANCE_ID(v);  
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);  
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);  
                o.color = v.color;  // 传递顶点颜色  
                return o;  
            }  
            
            fixed4 frag (v2f i) : SV_Target  
            {  
                // 采样原始颜色并应用顶点颜色  
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;  
                
                // 获取闪白参数  
                fixed flashAmount = UNITY_ACCESS_INSTANCED_PROP(Props, _FlashAmount);  
                
                // 混合颜色（保留原始alpha）  
                col.rgb = lerp(col.rgb, _FlashColor.rgb, flashAmount * _FlashColor.a);  
                return col;  
            }  
            ENDCG  
        }  
    }   
    FallBack "Sprites/Default"  
}  
