using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//物品数据
public class ItemData
{
    public int id; //物品Id
    public int currentRotation; //当前旋转度数
    public Vector2Int gridPos; //网格位置

    public ItemData() { }
    public ItemData(int id, int currentRotation, Vector2Int gridPos)
    {
        this.id = id;
        this.currentRotation = currentRotation;
        this.gridPos = gridPos;
    }
}
