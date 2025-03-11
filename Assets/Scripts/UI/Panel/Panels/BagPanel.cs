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
        //�򱳰�����������ӱ���
        BagManager.Instance.AddGrid(bag);
        BagManager.Instance.AddGrid(storageBox);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        BagManager.Instance.ClearAllItem(storageBox);
        BagManager.Instance.ClearAllItem(bag);
        //�򱳰����������Ƴ�����
        BagManager.Instance.RemoveGrid(bag);
        BagManager.Instance.RemoveGrid(storageBox);
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
