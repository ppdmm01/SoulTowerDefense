using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    [Header("按钮")]
    public Button arrangeBtn; //排序按钮
    public Button addItemBtn; //添加物品按钮
    public Button clearItemBtn; //清空物品按钮

    [Header("物品信息")]
    public GameObject ItemInfoObj;

    [Header("背包相关")]
    public BagGrid bag;
    public BagGrid storageBox;

    public override void Init()
    {
        HideItemInfo();
        //BagManager.Instance.itemsTrans = itemsTrans;

        arrangeBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.GetBagByName(storageBox.gridName).AutoArrange();
        });
        addItemBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.AddRandomItem(3, storageBox);
        });
        clearItemBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.ClearAllItem(storageBox);
        });
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //向背包管理器中添加背包
        BagManager.Instance.AddGrid(bag);
        BagManager.Instance.AddGrid(storageBox);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        BagManager.Instance.ClearAllItem(storageBox);
        BagManager.Instance.ClearAllItem(bag);
        //向背包管理器中移除背包
        BagManager.Instance.RemoveGrid(bag);
        BagManager.Instance.RemoveGrid(storageBox);
    }

    /// <summary>
    /// 显示物品信息
    /// </summary>
    public void ShowItemInfo(ItemSO data)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(data);
    }

    /// <summary>
    /// 隐藏物品信息
    /// </summary>
    public void HideItemInfo()
    {
        ItemInfoObj.SetActive(false);
        ItemInfoObj.GetComponent<ItemInfo>().RemoveAllAttributeInfo();
    }
}
