using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectPanel : BasePanel
{
    public SelectItemGrid selectGrid1;
    public SelectItemGrid selectGrid2;
    public SelectItemGrid selectGrid3;
    public BaseGrid storageBox;

    public TextMeshProUGUI selectItemName1;
    public TextMeshProUGUI selectItemName2;
    public TextMeshProUGUI selectItemName3;

    [Header("物品信息")]
    public GameObject ItemInfoObj;
    [Header("按钮")]
    public Button skipBtn; //跳过按钮
    public override void Init()
    {
        HideItemInfo();

        transform.SetAsLastSibling();

        skipBtn.onClick.AddListener(ClosePanelImmediate);

        EventCenter.Instance.AddEventListener(EventType.SelectItem, ClosePanel);
    }

    //更新防御塔物品
    public void UpdateTowerItem()
    {
        List<ItemTag> towerTag = new List<ItemTag>() { ItemTag.Tower };

        ItemSO data = ItemManager.Instance.GetItemDataByTags(towerTag);
        AddSelectItem(data, selectGrid1, selectItemName1);

        data = ItemManager.Instance.GetItemDataByTags(towerTag);
        AddSelectItem(data, selectGrid2, selectItemName2);

        data = ItemManager.Instance.GetItemDataByTags(towerTag);
        AddSelectItem(data, selectGrid3, selectItemName3);
    }

    //更新其他物品
    public void UpdateItem()
    {
        ItemSO data = ItemManager.Instance.GetRandomItemData(1)[0];
        AddSelectItem(data, selectGrid1, selectItemName1);

        data = ItemManager.Instance.GetRandomItemData(1)[0];
        AddSelectItem(data, selectGrid2, selectItemName2);

        data = ItemManager.Instance.GetRandomItemData(1)[0];
        AddSelectItem(data, selectGrid3, selectItemName3);
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //向背包管理器中添加背包
        GridManager.Instance.AddGrid(storageBox);
        //读取网格数据并更新
        GridData storageBoxData = GameDataManager.Instance.GetGridData(storageBox.gridName);
        if (storageBoxData != null)
            storageBox.UpdateGrid(storageBoxData);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //保存网格数据
        GridData storageBoxData = new GridData(storageBox.gridName, storageBox.items);
        GameDataManager.Instance.UpdateGridData(storageBoxData);
        GameDataManager.Instance.SaveGridData();
        //清空物品
        GridManager.Instance.ClearAllItem(selectGrid1, false);
        GridManager.Instance.ClearAllItem(selectGrid2, false);
        GridManager.Instance.ClearAllItem(selectGrid3, false);
        GridManager.Instance.ClearAllItem(storageBox, false);
        //向背包管理器中移除背包
        GridManager.Instance.RemoveGrid(storageBox);

        //移除事件监听
        EventCenter.Instance.RemoveEventListener(EventType.SelectItem, ClosePanel);
    }

    private void AddSelectItem(ItemSO data,SelectItemGrid grid,TextMeshProUGUI item)
    {
        grid.ForceUpdateGridLayout();
        item.text = data.itemChineseName;
        GridManager.Instance.AddItem(data.id, grid);
    }

    /// <summary>
    /// 显示物品信息
    /// </summary>
    public void ShowItemInfo(Item item)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(item);
    }

    /// <summary>
    /// 隐藏物品信息
    /// </summary>
    public void HideItemInfo()
    {
        ItemInfoObj.SetActive(false);
        ItemInfoObj.GetComponent<ItemInfo>().RemoveAllAttributeInfo();
    }

    private void ClosePanel()
    {
        StopAllCoroutines();
        //选择完物品后关闭面板
        canvasGroup.blocksRaycasts = false;
        GridManager.Instance.ClearAllItem(selectGrid1, false);
        GridManager.Instance.ClearAllItem(selectGrid2, false);
        GridManager.Instance.ClearAllItem(selectGrid3, false);
        skipBtn.gameObject.SetActive(false);
        StartCoroutine(closePanelRoutine());
    }

    private IEnumerator closePanelRoutine()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.HidePanel<SelectPanel>();
    }

    private void ClosePanelImmediate()
    {
        //选择完物品后关闭面板
        canvasGroup.blocksRaycasts = false;
        UIManager.Instance.HidePanel<SelectPanel>();
    }
}
