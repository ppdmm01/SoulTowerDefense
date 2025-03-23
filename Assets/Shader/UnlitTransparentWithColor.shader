Shader "Custom/LineRendererWithOffset"  
{  
    Properties  
    {  
        _MainTex ("Texture", 2D) = "white" {}              // 纹理  
        _MainTex_ST ("Tiling/Offset", Vector) = (1,1,0,0)  // 纹理平铺和偏移  
        _TintColor ("Tint Color", Color) = (1,1,1,1)       // 全局着色颜色  
    }  
    SubShader  
    {  
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }  
        LOD 100  

        Pass  
        {  
            Blend SrcAlpha OneMinusSrcAlpha   // 开启透明度混合  
            ZWrite Off                        // 禁止深度写入  
            Cull Off                          // 渲染双面，不剔除任何面  

            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  

            #include "UnityCG.cginc"  

            struct appdata_t  
            {  
                float4 vertex : POSITION;    // 顶点位置  
                float4 color : COLOR;        // 顶点颜色（接收 LineRenderer 的动态颜色）  
                float2 texcoord : TEXCOORD0; // 纹理坐标  
            };  

            struct v2f  
            {  
                float4 vertex : POSITION;    // 顶点坐标  
                float4 color : COLOR;        // 顶点颜色  
                float2 texcoord : TEXCOORD0; // 纹理坐标  
            };  

            sampler2D _MainTex;  
            float4 _MainTex_ST;              // 平铺和偏移参数  
            fixed4 _TintColor;               // 全局颜色调整  

            // 顶点着色器  
            v2f vert (appdata_t v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);                     // 转换到剪裁空间  
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);              // 应用平铺和偏移  
                o.color = v.color;                                            // 顶点颜色（来自 LineRenderer 设置）  
                return o;  
            }  

            // 片段着色器  
            fixed4 frag (v2f i) : SV_Target  
            {  
                fixed4 texColor = tex2D(_MainTex, i.texcoord);                 // 获取纹理颜色  
                fixed4 outputColor = texColor * i.color * _TintColor;         // 叠加纹理颜色、顶点颜色和全局颜色  
                return outputColor;                                           // 输出最终颜色  
            }  
            ENDCG  
        }  
    }  
}  