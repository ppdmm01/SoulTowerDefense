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
    }

    [HideInInspector] public Transform itemsTrans; //放置物品的父对象
    public Dictionary<string, BagGrid> BagDic; //通过字典（背包可能有多个，比如一个背包，一个储物箱）

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

    //朝指定背包添加指定物品
    public void AddItemByName(string name,BagGrid bag)
    {
        GameObject itemObj = Instantiate(Resources.Load<GameObject>("Items/"+name));
        itemObj.transform.SetParent(itemsTrans,false);
        Item item = itemObj.GetComponent<Item>();
        item.Init(bag);
        if (!bag.TryAutoPlaceItem(item))
        {
            Debug.LogError($"物品 {item.data.itemName} 无法放置，背包可能已满");
            //TODO:后续处理（删除、开新一页等）
            bag.items.Remove(item);
            Destroy(itemObj);
        }
    }
}
