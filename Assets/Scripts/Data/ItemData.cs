using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��Ʒ����
public class ItemData
{
    public int id; //��ƷId
    public int currentRotation; //��ǰ��ת����
    public Vector2Int gridPos; //����λ��

    public ItemData() { }
    public ItemData(int id, int currentRotation, Vector2Int gridPos)
    {
        this.id = id;
        this.currentRotation = currentRotation;
        this.gridPos = gridPos;
    }
}
