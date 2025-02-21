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

    [Header("��ť")]
    public Button arrangeBtn; //����ť
    //�����Ʒ��ť
    public Button AdditemBtn;

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
        AdditemBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.AddRandomItem(3, BagManager.Instance.GetBagByName("storageBox"));
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
    /// ������Ϣ
    /// </summary>
    public void UpdateMessage()
    {
        string info = "";
        foreach (TowerData towerData in TowerManager.Instance.towers.Values)
        {
            info += "���֣�" + towerData.towerName + "\n";
            info += "������" + towerData.description + "\n";
            info += "�˺���" + towerData.damage + "\n";
            info += "������Χ��" + towerData.range + "\n";
            info += "���������" + towerData.interval + "\n";
            info += "������" + towerData.output + "\n";
            info += "���������" + towerData.cooldown + "\n";
            info += "----------------------\n";
        }

        bagItemInfo.text = info;
    }
}
