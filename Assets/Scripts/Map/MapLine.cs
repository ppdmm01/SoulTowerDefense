using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLine : MonoBehaviour
{
    private LineRenderer lR;
    public float scrollSpeed = 0.5f;  // 移动速度  
    private Material lineMaterial;    // 存储材质实例  
    private float offset = 0;             // 偏移量  

    private void Start()
    {
        lR = GetComponent<LineRenderer>();
        lineMaterial = lR.material;
        offset = 0;
    }

    private void Update()
    {
        // 计算新的偏移量  
        offset -= Time.deltaTime * scrollSpeed;
        if (offset < 0f) offset = 1f; // 循环，确保偏移量不会过大  
        if (offset > 1f) offset = 0f;
        offset = Mathf.Clamp01(offset);
        // 设置材质的纹理偏移  
        lineMaterial.mainTextureOffset = new Vector2(offset, 0);
    }
}
