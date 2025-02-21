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

    [HideInInspector] public Transform itemsTrans; //放置物品的父对象
    public Dictionary<string, BagGrid> BagDic; //通过字典（背包可能有多个，比如一个背包，一个储物箱）
    private GameObject itemPrefab;

    //检测鼠标点是否在指定背包里
    public bool IsInsideBag(Vector2 screenPoint,string bagName)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
           BagDic[bagName].transform as RectTransform,
           screenPoint,
           null
        );

       return isInside;
    }

    //获取背包
    public BagGrid GetBagByName(string bagName)
    {
        return BagDic[bagName];
    }

    //朝指定背包添加指定名称物品
    public void AddItemByName(string itemName,BagGrid bag)
    {
        //GameObject itemObj = Instantiate(Resources.Load<GameObject>("Items/"+name));
        //创建物体
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans,false);
        //添加并初始化Item脚本
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataByName(itemName);
        item.Init(data, bag);
        //背包满了
        if (!bag.TryAutoPlaceItem(item))
        {
            Debug.LogError($"物品 {item.data.itemName} 无法放置，背包已满");
            //删除多余的物品
            item.DeleteMe();
        }
    }

    //朝指定背包添加指定Id的物品
    public void AddItemById(int id,BagGrid bag)
    {
        //创建物体
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //添加并初始化Item脚本
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataById(id);
        item.Init(data,bag);
        //背包满了
        if (!bag.TryAutoPlaceItem(item))
        {
            Debug.LogError($"物品 {item.data.itemName} 无法放置，背包已满");
            //删除多余的物品
            item.DeleteMe();
        }
    }

    //添加指定数量的随机道具
    public void AddRandomItem(int num, BagGrid bag)
    {
        List<ItemSO> list = ItemManager.Instance.GetRandomItemData(num);
        GameObject itemObj;
        Item item;
        foreach (ItemSO itemData in list)
        {
            //创建物体
            itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //添加并初始化Item脚本
            item = itemObj.AddComponent<Item>();
            item.Init(itemData, bag);
            //背包满了
            if (!bag.TryAutoPlaceItem(item))
            {
                Debug.LogError($"物品 {item.data.itemName} 无法放置，背包已满");
                //删除多余的物品
                item.DeleteMe();
                break;
            }
        }
    }

    /// <summary>
    /// 更新主背包信息
    /// </summary>
    public void UpdateMainBagInfo()
    {
        //这里先默认存在，后面可能需要判空
        GetBagByName("bag").CalculateTower();
    }
}
