using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品管理器
/// </summary>
public class ItemManager : Singleton<ItemManager>
{
    private ItemManager()
    {
        if (data == null)
        {
            data = Resources.Load<ItemManagerSO>("Data/ItemManagerSO");
            if (data == null)
                Debug.LogError("加载ItemManagerSO失败！");
        }

        //将数据添加到物品字典中
        itemDataDic = new Dictionary<string, ItemSO>();
        itemDataDic2 = new Dictionary<int, ItemSO>();
        foreach (ItemSO itemSO in data.itemSOList)
        {
            if (!itemDataDic.ContainsKey(itemSO.itemName))
                itemDataDic.Add(itemSO.itemName, itemSO);
            if (!itemDataDic2.ContainsKey(itemSO.id))
                itemDataDic2.Add(itemSO.id, itemSO);
        }

        dragTarget = null;
    }

    private ItemManagerSO data; //所有物品数据
    private Dictionary<string, ItemSO> itemDataDic; //物品字典（通过名称）
    private Dictionary<int, ItemSO> itemDataDic2; //物品字典（通过ID）

    public Item dragTarget; //拖动物品的目标

    //获取指定名字的物品数据
    public ItemSO GetItemDataByName(string itemName)
    {
        if (!itemDataDic.ContainsKey(itemName)) return null;
        return itemDataDic[itemName];
    }

    //获取指定Id的物品数据
    public ItemSO GetItemDataById(int itemId)
    {
        if (!itemDataDic2.ContainsKey(itemId)) return null;
        return itemDataDic2[itemId];
    }

    //得到指定数量的随机物品数据
    public List<ItemSO> GetRandomItemData(int num)
    {
        int totalNum = data.itemSOList.Count;
        int randomNum;
        List<ItemSO> list = new List<ItemSO>();
        for (int i = 0; i < num; i++)
        {
            randomNum = Random.Range(0, totalNum);
            list.Add(data.itemSOList[randomNum]);
        }
        return list;
    }
}
