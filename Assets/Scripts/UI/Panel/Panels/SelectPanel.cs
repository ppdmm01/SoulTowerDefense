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

    [Header("��Ʒ��Ϣ")]
    public GameObject ItemInfoObj;
    [Header("��ť")]
    public Button skipBtn; //������ť
    public override void Init()
    {
        HideItemInfo();

        transform.SetAsLastSibling();

        skipBtn.onClick.AddListener(ClosePanelImmediate);

        EventCenter.Instance.AddEventListener(EventType.SelectItem, ClosePanel);
    }

    //���·�������Ʒ
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

    //����������Ʒ
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
        //�򱳰�����������ӱ���
        GridManager.Instance.AddGrid(storageBox);
        //��ȡ�������ݲ�����
        GridData storageBoxData = GameDataManager.Instance.GetGridData(storageBox.gridName);
        if (storageBoxData != null)
            storageBox.UpdateGrid(storageBoxData);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //������������
        GridData storageBoxData = new GridData(storageBox.gridName, storageBox.items);
        GameDataManager.Instance.UpdateGridData(storageBoxData);
        GameDataManager.Instance.SaveGridData();
        //�����Ʒ
        GridManager.Instance.ClearAllItem(selectGrid1, false);
        GridManager.Instance.ClearAllItem(selectGrid2, false);
        GridManager.Instance.ClearAllItem(selectGrid3, false);
        GridManager.Instance.ClearAllItem(storageBox, false);
        //�򱳰����������Ƴ�����
        GridManager.Instance.RemoveGrid(storageBox);

        //�Ƴ��¼�����
        EventCenter.Instance.RemoveEventListener(EventType.SelectItem, ClosePanel);
    }

    private void AddSelectItem(ItemSO data,SelectItemGrid grid,TextMeshProUGUI item)
    {
        grid.ForceUpdateGridLayout();
        item.text = data.itemChineseName;
        GridManager.Instance.AddItem(data.id, grid);
    }

    /// <summary>
    /// ��ʾ��Ʒ��Ϣ
    /// </summary>
    public void ShowItemInfo(Item item)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(item);
    }

    /// <summary>
    /// ������Ʒ��Ϣ
    /// </summary>
    public void HideItemInfo()
    {
        ItemInfoObj.SetActive(false);
        ItemInfoObj.GetComponent<ItemInfo>().RemoveAllAttributeInfo();
    }

    private void ClosePanel()
    {
        StopAllCoroutines();
        //ѡ������Ʒ��ر����
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
        //ѡ������Ʒ��ر����
        canvasGroup.blocksRaycasts = false;
        UIManager.Instance.HidePanel<SelectPanel>();
    }
}
