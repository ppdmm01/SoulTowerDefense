using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public class GridData
{
    public string gridName; //������
    public List<ItemData> itemDatas; //�洢����Ʒ����

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

    //����������Ʒ�ɳ�
    public void AllItemGrow()
    {
        foreach (ItemData itemData in itemDatas)
        {
            itemData.Grow();
        }
    }
}
