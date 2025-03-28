using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//物品数据
public class ItemData
{
    public int id; //物品Id
    public int currentRotation; //当前旋转度数
    public Vector2Int gridPos; //网格位置
    public int growSpeed; //成长速度
    public List<ItemAttribute> itemAttributes; //物品当前属性（可能有成长，需要记录）
    public List<BuffType> itemBuffs; //物品当前拥有的buff（针对防御塔道具）

    public ItemData() { }
    public ItemData(int id, int currentRotation, Vector2Int gridPos,int growSpeed, 
        List<ItemAttribute> itemAttributes, List<BuffType> itemBuffs)
    {
        this.id = id;
        this.currentRotation = currentRotation;
        this.gridPos = gridPos;
        this.growSpeed = growSpeed;
        this.itemAttributes = itemAttributes;
        this.itemBuffs = itemBuffs;
    }

    //触发物品成长
    public void Grow()
    {
        foreach (ItemAttribute itemAttribute in itemAttributes)
        {
            itemAttribute.Grow(growSpeed);
        }
    }
}
