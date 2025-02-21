using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʒ������
/// </summary>
public class ItemManager : Singleton<ItemManager>
{
    private ItemManager()
    {
        if (data == null)
        {
            data = Resources.Load<ItemManagerSO>("Data/ItemManagerSO");
            if (data == null)
                Debug.LogError("����ItemManagerSOʧ�ܣ�");
        }

        //��������ӵ���Ʒ�ֵ���
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

    private ItemManagerSO data; //������Ʒ����
    private Dictionary<string, ItemSO> itemDataDic; //��Ʒ�ֵ䣨ͨ�����ƣ�
    private Dictionary<int, ItemSO> itemDataDic2; //��Ʒ�ֵ䣨ͨ��ID��

    public Item dragTarget; //�϶���Ʒ��Ŀ��

    //��ȡָ�����ֵ���Ʒ����
    public ItemSO GetItemDataByName(string itemName)
    {
        if (!itemDataDic.ContainsKey(itemName)) return null;
        return itemDataDic[itemName];
    }

    //��ȡָ��Id����Ʒ����
    public ItemSO GetItemDataById(int itemId)
    {
        if (!itemDataDic2.ContainsKey(itemId)) return null;
        return itemDataDic2[itemId];
    }

    //�õ�ָ�������������Ʒ����
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
