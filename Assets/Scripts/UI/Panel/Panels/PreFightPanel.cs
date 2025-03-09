using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PreFightPanel : BasePanel
{
    [Header("��ť")]
    public Button startFightBtn; //��ʼս����ť

    [Header("��������Ϣ")]
    public ScrollRect towerSr;
    public string nowTowerInfoName; //��ǰչʾ�ķ�������
    private List<TowerInfoBtn> towerInfoBtnList; //��������Ϣ��ť�б�
    public TowerInfo towerInfo; //��������Ϣ
    private float nowWeight; //��ǰ���з�������Ϣ���

    public override void Init()
    {
        towerInfoBtnList = new List<TowerInfoBtn>();
        nowTowerInfoName = "";
        nowWeight = 0;

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

        //��ʾ�������
        BagPanel bagPanel = UIManager.Instance.ShowPanel<BagPanel>();
        if (bagPanel != null)
        {
            bagPanel.transform.SetAsLastSibling();
        }

        //���¶�����
        TopColumnPanel topPanel = UIManager.Instance.ShowPanel<TopColumnPanel>();
        if (topPanel != null)
        {
            topPanel.transform.SetAsLastSibling();
            topPanel.ShowBtn(TopColumnBtnType.Book, TopColumnBtnType.Map, TopColumnBtnType.Menu);
        }
        topPanel.SetTitle("սǰ׼��");
    }

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
    /// ��ʼս��
    /// </summary>
    public void StartFight()
    {
        //ս����ʼ
        LevelManager.Instance.StartLevel("LevelScene1");
        UIManager.Instance.HidePanel<PreFightPanel>();
        UIManager.Instance.HidePanel<BagPanel>();
    }
}
