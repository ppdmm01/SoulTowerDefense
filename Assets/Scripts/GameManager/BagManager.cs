using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class BagManager : SingletonMono<BagManager>
{
    private BagManager() {}

    protected override void Awake()
    {
        base.Awake();
        BagDic = new Dictionary<string, BagGrid>();
        itemPrefab = Resources.Load<GameObject>("Bag/ItemPrefab");
    }

    [HideInInspector] public Transform itemsTrans; //������Ʒ�ĸ�����
    public Dictionary<string, BagGrid> BagDic; //ͨ���ֵ䣨���������ж��������һ��������һ�������䣩
    private GameObject itemPrefab;

    //��������Ƿ���ָ��������
    public bool IsInsideBag(Vector2 screenPoint,string bagName)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
           BagDic[bagName].transform as RectTransform,
           screenPoint,
           null
        );

       return isInside;
    }

    //��ȡ����
    public BagGrid GetBagByName(string bagName)
    {
        return BagDic[bagName];
    }

    //��ָ���������ָ��������Ʒ
    public void AddItemByName(string itemName,BagGrid bag)
    {
        //GameObject itemObj = Instantiate(Resources.Load<GameObject>("Items/"+name));
        //��������
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans,false);
        //��Ӳ���ʼ��Item�ű�
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataByName(itemName);
        item.Init(data, bag);
        //��������
        if (!bag.TryAutoPlaceItem(item))
        {
            Debug.LogError($"��Ʒ {item.data.itemName} �޷����ã���������");
            //ɾ���������Ʒ
            item.DeleteMe();
        }
    }

    //��ָ���������ָ��Id����Ʒ
    public void AddItemById(int id,BagGrid bag)
    {
        //��������
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //��Ӳ���ʼ��Item�ű�
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataById(id);
        item.Init(data,bag);
        //��������
        if (!bag.TryAutoPlaceItem(item))
        {
            Debug.LogError($"��Ʒ {item.data.itemName} �޷����ã���������");
            //ɾ���������Ʒ
            item.DeleteMe();
        }
    }

    //���ָ���������������
    public void AddRandomItem(int num, BagGrid bag)
    {
        List<ItemSO> list = ItemManager.Instance.GetRandomItemData(num);
        GameObject itemObj;
        Item item;
        foreach (ItemSO itemData in list)
        {
            //��������
            itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //��Ӳ���ʼ��Item�ű�
            item = itemObj.AddComponent<Item>();
            item.Init(itemData, bag);
            //��������
            if (!bag.TryAutoPlaceItem(item))
            {
                Debug.LogError($"��Ʒ {item.data.itemName} �޷����ã���������");
                //ɾ���������Ʒ
                item.DeleteMe();
                break;
            }
        }
    }

    /// <summary>
    /// ������������Ϣ
    /// </summary>
    public void UpdateMainBagInfo()
    {
        //������Ĭ�ϴ��ڣ����������Ҫ�п�
        GetBagByName("bag").CalculateTower();
    }
}
