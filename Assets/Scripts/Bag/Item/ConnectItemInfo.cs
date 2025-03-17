using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ʒ����Ϣ�����ڴ�ż�����Ʒ����Ϣ
/// </summary>
public class ConnectItemInfo
{
    public Item item; //��Ʒ����
    public ItemAttribute activateAttribute; //���������

    public ConnectItemInfo() { }
    public ConnectItemInfo(Item item, ItemAttribute activateAttribute)
    {
        this.item = item;
        this.activateAttribute = activateAttribute;
    }
}
