using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    [Header("��ť")]
    public Button arrangeBtn; //����ť
    public Button addItemBtn; //�����Ʒ��ť
    public Button clearItemBtn; //�����Ʒ��ť

    [Header("��Ʒ��Ϣ")]
    public GameObject ItemInfoObj;

    [Header("�������")]
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
        //�򱳰�����������ӱ���
        GridManager.Instance.AddGrid(bag);
        GridManager.Instance.AddGrid(storageBox);
        //��ȡ�������ݲ�����
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
        //������������
        GridData bagData = new GridData(bag.gridName,bag.items);
        GridData storageBoxData = new GridData(storageBox.gridName, storageBox.items);
        GameDataManager.Instance.UpdateGridData(bagData);
        GameDataManager.Instance.UpdateGridData(storageBoxData);
        GameDataManager.Instance.SaveGridData();
        //�����Ʒ
        GridManager.Instance.ClearAllItem(storageBox,false);
        GridManager.Instance.ClearAllItem(bag, false);
        //�򱳰����������Ƴ�����
        GridManager.Instance.RemoveGrid(bag);
        GridManager.Instance.RemoveGrid(storageBox);
    }

    /// <summary>
    /// ��ʾ��Ʒ��Ϣ
    /// </summary>
    public void ShowItemInfo(ItemSO data)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(data);
    }

    /// <summary>
    /// ������Ʒ��Ϣ
    /// </summary>
    public void HideItemInfo()
    {
        ItemInfoObj.SetActive(false);
        ItemInfoObj.GetComponent<ItemInfo>().RemoveAllAttributeInfo();
    }
}
