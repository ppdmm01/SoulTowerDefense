using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网格数据
/// </summary>
public class GridData
{
    public string gridName; //网格名
    public List<ItemData> itemDatas; //存储的物品数据

    public GridData() { }

    public GridData(string gridName, List<Item> items)
    {
        this.gridName = gridName;
        List<ItemData> list = new List<ItemData>();
        foreach (Item item in items)
        {
            list.Add(new ItemData(item.data.id, item.currentRotation, item.gridPos, item.growSpeed, item.nowAttributes,item.nowItemBuffs));
        }
        this.itemDatas = list;
    }

    //触发所有物品成长
    public void AllItemGrow()
    {
        foreach (ItemData itemData in itemDatas)
        {
            itemData.Grow();
        }
    }
}
