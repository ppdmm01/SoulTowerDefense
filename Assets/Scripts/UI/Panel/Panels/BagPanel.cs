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
    public BaseGrid storageBox;

    public override void Init()
    {
        HideItemInfo();
        //BagManager.Instance.itemsTrans = itemsTrans;

        arrangeBtn.onClick.AddListener(() =>
        {
            GridManager.Instance.GetBagByName(storageBox.gridName).AutoArrange();
        });
        addItemBtn.onClick.AddListener(() =>
        {
            GridManager.Instance.AddRandomItem(3, storageBox);
        });
        clearItemBtn.onClick.AddListener(() =>
        {
            GridManager.Instance.ClearAllItem(storageBox, false);
        });
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //向背包管理器中添加背包
        GridManager.Instance.AddGrid(bag);
        GridManager.Instance.AddGrid(storageBox);
        //读取网格数据并更新
        GridData bagData = GameDataManager.Instance.GetGridData(bag.gridName);
        GridData storageBoxData = GameDataManager.Instance.GetGridData(storageBox.gridName);
        if (bagData != null)
            bag.UpdateGrid(bagData);
        if (storageBoxData != null)
            storageBox.UpdateGrid(storageBoxData);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //保存网格数据
        GridData bagData = new GridData(bag.gridName,bag.items);
        GridData storageBoxData = new GridData(storageBox.gridName, storageBox.items);
        GameDataManager.Instance.UpdateGridData(bagData);
        GameDataManager.Instance.UpdateGridData(storageBoxData);
        GameDataManager.Instance.SaveGridData();
        //清空物品
        GridManager.Instance.ClearAllItem(storageBox,false);
        GridManager.Instance.ClearAllItem(bag, false);
        //向背包管理器中移除背包
        GridManager.Instance.RemoveGrid(bag);
        GridManager.Instance.RemoveGrid(storageBox);
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
