using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLine : MonoBehaviour
{
    private LineRenderer lR;
    public float scrollSpeed = 0.5f;  // �ƶ��ٶ�  
    private Material lineMaterial;    // �洢����ʵ��  
    private float offset = 0;             // ƫ����  

    private void Start()
    {
        lR = GetComponent<LineRenderer>();
        lineMaterial = lR.material;
        offset = 0;
    }

    private void Update()
    {
        // �����µ�ƫ����  
        offset -= Time.deltaTime * scrollSpeed;
        if (offset < 0f) offset = 1f; // ѭ����ȷ��ƫ�����������  
        if (offset > 1f) offset = 0f;
        offset = Mathf.Clamp01(offset);
        // ���ò��ʵ�����ƫ��  
        lineMaterial.mainTextureOffset = new Vector2(offset, 0);
    }
}
