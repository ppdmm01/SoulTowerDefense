using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相邻物品的信息，用于存放激活物品的信息
/// </summary>
public class ConnectItemInfo
{
    public Item item; //物品数据
    public ItemAttribute activateAttribute; //激活的属性

    public ConnectItemInfo() { }
    public ConnectItemInfo(Item item, ItemAttribute activateAttribute)
    {
        this.item = item;
        this.activateAttribute = activateAttribute;
    }
}
