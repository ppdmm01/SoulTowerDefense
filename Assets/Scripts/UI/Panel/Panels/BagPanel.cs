using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    [Header("�ı���Ϣ")]
    public TextMeshProUGUI bagItemInfo;
    public TextMeshProUGUI boxItemInfo;

    [Header("��ť")]
    public Button arrangeBtn; //����ť
    //�����Ʒ��ť
    public Button AdditemBtn1;
    public Button AdditemBtn2;
    public Button AdditemBtn3;
    public Button AdditemBtn4;
    public Button AdditemBtn5;
    public Button AdditemBtn6;

    //������Ʒ�ĵط�
    private Transform itemsTrans;
    [Header("�������")]
    public BagGrid bag;
    public BagGrid storageBox;

    public override void Init()
    {
        itemsTrans = transform.Find("Items");
        BagManager.Instance.itemsTrans = itemsTrans;

        arrangeBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.BagDic["storageBox"].AutoArrange();
        });
        AdditemBtn1.onClick.AddListener(() =>
        {
            BagManager.Instance.AddItemByName("Ammo", BagManager.Instance.BagDic["storageBox"]);
        });
        AdditemBtn2.onClick.AddListener(() =>
        {
            BagManager.Instance.AddItemByName("Grenade", BagManager.Instance.BagDic["storageBox"]);
        });
        AdditemBtn3.onClick.AddListener(() =>
        {
            BagManager.Instance.AddItemByName("Katana", BagManager.Instance.BagDic["storageBox"]);
        });
        AdditemBtn4.onClick.AddListener(() =>
        {
            BagManager.Instance.AddItemByName("Medkit", BagManager.Instance.BagDic["storageBox"]);
        });
        AdditemBtn5.onClick.AddListener(() =>
        {
            BagManager.Instance.AddItemByName("Rifle", BagManager.Instance.BagDic["storageBox"]);
        });
        AdditemBtn6.onClick.AddListener(() =>
        {
            BagManager.Instance.AddItemByName("ShotGun", BagManager.Instance.BagDic["storageBox"]);
        });
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //�򱳰�����������ӱ���
        BagManager.Instance.BagDic.Add(bag.bagName,bag);
        BagManager.Instance.BagDic.Add(storageBox.bagName, storageBox);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //�򱳰����������Ƴ�����
        BagManager.Instance.BagDic.Remove(bag.bagName);
        BagManager.Instance.BagDic.Remove(storageBox.bagName);
    }

    /// <summary>
    /// ���±�����Ϣ
    /// </summary>
    public void UpdateBagInfo()
    {
        string bagInfo = "";
        string boxInfo = "";
        List<Item> list = BagManager.Instance.BagDic["bag"].items;
        for (int i = 0; i < list.Count; i++)
        {
            bagInfo += $"{i + 1}��{list[i].data.itemName}\n";
        }
        list = BagManager.Instance.BagDic["storageBox"].items;
        for (int i = 0; i < list.Count; i++)
        {
            boxInfo += $"{i + 1}��{list[i].data.itemName}\n";
        }
        bagItemInfo.text = bagInfo;
        boxItemInfo.text = boxInfo;
    }
}
