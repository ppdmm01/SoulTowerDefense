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
    public Button addItemBtn; //�����Ʒ��ť
    public Button clearItemBtn; //�����Ʒ��ť
    public Button startFightBtn; //��ʼս����ť

    [Header("��������Ϣ")]
    public ScrollRect towerSr;
    //private List<TowerInfo> towerInfoList; //��������Ϣ�б�
    public string nowTowerInfoName; //��ǰչʾ�ķ�������
    private List<TowerInfoBtn> towerInfoBtnList; //��������Ϣ��ť�б�
    public TowerInfo towerInfo; //��������Ϣ
    private float nowWeight; //��ǰ���з�������Ϣ���

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
        //towerInfoList = new List<TowerInfo>();
        towerInfoBtnList = new List<TowerInfoBtn>();
        nowTowerInfoName = "";
        nowWeight = 0;

        itemsTrans = transform.Find("Items");
        BagManager.Instance.itemsTrans = itemsTrans;

        arrangeBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.BagDic["storageBox"].AutoArrange();
        });
        addItemBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.AddRandomItem(3, storageBox);
        });
        clearItemBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.ClearAllItem(storageBox);
        });
        startFightBtn.onClick.AddListener(() =>
        {
            if (TowerManager.Instance.towerDatas.Count == 0)
            {
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("��û���ϳ��κη�������ȷ��������", StartFight);
            }
            else
            {
                StartFight();
            }
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
    //public void UpdateTowerInfo()
    //{
    //    //��������������б��ж��������
    //    if (TowerManager.Instance.towerDatas.Count < towerInfoList.Count)
    //    {
    //        for (int i= towerInfoList.Count-1 ; i >= TowerManager.Instance.towerDatas.Count; i--)
    //        {
    //            //���ٶ���
    //            Destroy(towerInfoList[i].gameObject);
    //            //�Ƴ�����
    //            towerInfoList.RemoveAt(i);  
    //        }
    //    }
    //    //���ݲ��㣬�����µ�����
    //    else if (TowerManager.Instance.towerDatas.Count > towerInfoList.Count)
    //    {
    //        //��������
    //        GameObject towerInfoObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/TowerInfo"));
    //        towerInfoObj.transform.SetParent(towerSr.content, false);
    //        TowerInfo towerInfo = towerInfoObj.GetComponent<TowerInfo>();
    //        //�������
    //        towerInfoList.Add(towerInfo);
    //    }

    //    //���ʣ���������Ϣ����������
    //    foreach (TowerInfo towerInfo in towerInfoList)
    //        towerInfo.RemoveAllAttributeInfo();

    //    //������������
    //    int index = 0;
    //    nowHeight = 0;
    //    foreach (string towerName in TowerManager.Instance.towerDatas.Keys)
    //    {
    //        TowerInfo towerInfo = towerInfoList[index];
    //        TowerData newData = TowerManager.Instance.towerDatas[towerName]; //������

    //        if (TowerManager.Instance.oldTowerDatas.ContainsKey(towerName))
    //        {
    //            TowerData oldData = TowerManager.Instance.oldTowerDatas[towerName]; //�仯ǰ������
    //            towerInfo.SetChangedInfo(newData, oldData, -nowHeight); //��Ҫ��ʾ���ݱ仯
    //        }
    //        else
    //            towerInfo.SetInfo(newData, -nowHeight);

    //        nowHeight += towerInfo.GetHeight();
    //        index++;
    //    }
    //    //����ScrollView���ݸ߶�
    //    towerSr.content.sizeDelta = new Vector2(towerSr.content.sizeDelta.x, nowHeight);
    //}

    /// <summary>
    /// ����չʾ�ķ�������Ϣ
    /// </summary>
    public void UpdateTowerInfo(string towerName)
    {
        if (towerName == "" || !TowerManager.Instance.towerDatas.ContainsKey(towerName)) //�ÿ�
        {
            towerInfo.SetNull();
            towerInfo.RemoveAllAttributeInfo();
            return;
        }

        TowerData data = TowerManager.Instance.towerDatas[towerName]; //��ȡ��ǰչʾ�ķ���������
        towerInfo.RemoveAllAttributeInfo(); //��շ�������Ϣ����������

        //������������
        if (TowerManager.Instance.oldTowerDatas.ContainsKey(towerName) && towerName == nowTowerInfoName) //չʾ����������ʱ����Ҫ��ʾ��ֵ�仯
        {
            TowerData oldData = TowerManager.Instance.oldTowerDatas[towerName]; //�仯ǰ������
            towerInfo.SetChangedInfo(data, oldData); //��ʾ���ݱ仯
        }
        else
            towerInfo.SetInfo(data);
    }

    /// <summary>
    /// ���·�������Ϣ��ť
    /// </summary>
    public void UpdateTowerInfoBtn()
    {
        //��������������б��ж��������
        if (TowerManager.Instance.towerDatas.Count < towerInfoBtnList.Count)
        {
            for (int i = towerInfoBtnList.Count - 1; i >= TowerManager.Instance.towerDatas.Count; i--)
            {
                //���ٶ���
                Destroy(towerInfoBtnList[i].gameObject);
                //�Ƴ�����
                towerInfoBtnList.RemoveAt(i);
            }
        }
        //���ݲ��㣬�����µ�����
        else if (TowerManager.Instance.towerDatas.Count > towerInfoBtnList.Count)
        {
            //��������
            GameObject towerInfoObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/TowerInfoBtn"));
            towerInfoObj.transform.SetParent(towerSr.content, false);
            TowerInfoBtn towerInfo = towerInfoObj.GetComponent<TowerInfoBtn>();
            //�������
            towerInfoBtnList.Add(towerInfo);
        }

        //������������
        int index = 0;
        nowWeight = 0;
        foreach (string towerName in TowerManager.Instance.towerDatas.Keys)
        {
            if(nowTowerInfoName == "" || !TowerManager.Instance.towerDatas.ContainsKey(nowTowerInfoName))
            {
                nowTowerInfoName = towerName;
                UpdateTowerInfo(nowTowerInfoName); //����ǵ�һ�θ��°�ť�����Զ���ʾ��һ��������
            }

            TowerInfoBtn towerInfoBtn = towerInfoBtnList[index];
            TowerData data = TowerManager.Instance.towerDatas[towerName]; //������
            towerInfoBtn.InitInfo(data); //��ʼ����ť
            nowWeight += towerSr.content.GetComponent<GridLayoutGroup>().cellSize.x;
            index++;
        }
        
        //���û�����ݣ����ÿ�
        if (TowerManager.Instance.towerDatas.Count == 0)
        {
            nowTowerInfoName = "";
            UpdateTowerInfo(nowTowerInfoName);
        }
        //����ScrollView���ݿ��
        towerSr.content.sizeDelta = new Vector2(nowWeight, towerSr.content.sizeDelta.y);
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

    /// <summary>
    /// ��ʼս��
    /// </summary>
    public void StartFight()
    {
        //ս����ʼ
        LevelManager.Instance.StartLevel("LevelScene1");
        UIManager.Instance.HidePanel<BagPanel>();
    }
}
