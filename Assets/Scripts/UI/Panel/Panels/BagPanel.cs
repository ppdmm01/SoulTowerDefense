using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    [Header("��ť")]
    public Button arrangeBtn; //����ť
    public Button AdditemBtn; //�����Ʒ��ť
    public Button startFightBtn; //��ʼս����ť

    [Header("��������Ϣ")]
    public ScrollRect towerSr;
    private List<TowerInfo> towerInfoList; //��������Ϣ�б�
    private float nowHeight; //��ǰ���з�������Ϣ�߶�

    [Header("��Ʒ��Ϣ")]
    public GameObject ItemInfoObj;

    //������Ʒ�ĵط�
    private Transform itemsTrans;
    [Header("�������")]
    public BagGrid bag;
    public BagGrid storageBox;

    public override void Init()
    {
        HideItemInfo();
        towerInfoList = new List<TowerInfo>();
        nowHeight = 0;

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
        startFightBtn.onClick.AddListener(() =>
        {
            //ս����ʼ
            LevelManager.Instance.StartLevel("LevelScene1");
            UIManager.Instance.HidePanel<BagPanel>();
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
    /// ���·�������Ϣ
    /// </summary>
    public void UpdateTowerInfo()
    {
        //��������������б��ж��������
        if (TowerManager.Instance.towers.Count < towerInfoList.Count)
        {
            for (int i= towerInfoList.Count-1 ; i >= TowerManager.Instance.towers.Count; i--)
            {
                //���ٶ���
                Destroy(towerInfoList[i].gameObject);
                //�Ƴ�����
                towerInfoList.RemoveAt(i);  
            }
        }
        //���ݲ��㣬�����µ�����
        else if (TowerManager.Instance.towers.Count > towerInfoList.Count)
        {
            //��������
            GameObject towerInfoObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/TowerInfo"));
            towerInfoObj.transform.SetParent(towerSr.content, false);
            TowerInfo towerInfo = towerInfoObj.GetComponent<TowerInfo>();
            //�������
            towerInfoList.Add(towerInfo);
        }

        //���ʣ���������Ϣ����������
        foreach (TowerInfo towerInfo in towerInfoList)
            towerInfo.RemoveAllAttributeInfo();

        //������������
        int index = 0;
        nowHeight = 0;
        foreach (TowerData towerData in TowerManager.Instance.towers.Values)
        {
            TowerInfo towerInfo = towerInfoList[index];
            towerInfo.SetInfo(towerData, -nowHeight);
            nowHeight += towerInfo.GetHeight();
            index++;
        }
        //����ScrollView���ݸ߶�
        towerSr.content.sizeDelta = new Vector2(towerSr.content.sizeDelta.x, nowHeight);
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
