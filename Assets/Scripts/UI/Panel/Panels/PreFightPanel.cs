using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PreFightPanel : BasePanel
{
    [Header("��ť")]
    public Button startFightBtn; //��ʼս����ť
    public Button CombinationBtn; //�����Ϣ��ť

    [Header("��������Ϣ")]
    public ScrollRect towerSr;
    public string nowTowerInfoName; //��ǰչʾ�ķ�������
    private List<TowerInfoBtn> towerInfoBtnList; //��������Ϣ��ť�б�
    public TowerInfo towerInfo; //��������Ϣ
    private float nowWeight; //��ǰ���з�������Ϣ���
    private List<CombinationSO> combinationList; //����������Ϣ�б�

    [Header("������buff���")]
    public TextMeshProUGUI buffTxt; //buff����
    public TextMeshProUGUI buffTitle; //buff����
    public GameObject buffInfo; //buff��Ϣ���
    public Button closeBtn; //�ر�buff��Ϣ��ť
    public List<TowerBuffInfoBtn> towerBuffBtnList; //������buff��Ϣ��ť�б�

    public Transform buffBtnContainer; //���buff��ť
    public BuffType nowBuffType; //��ǰչʾ��buff����

    public override void Init()
    {
        towerInfoBtnList = new List<TowerInfoBtn>();
        towerBuffBtnList = new List<TowerBuffInfoBtn>();
        combinationList = new List<CombinationSO>();
        nowTowerInfoName = "";
        nowWeight = 0;

        startFightBtn.onClick.AddListener(() =>
        {
            canvasGroup.blocksRaycasts = false;
            if (TowerManager.Instance.towerDatas.Count == 0)
            {
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("��û���ϳ��κη�������ȷ��������", StartFight);
            }
            else
            {
                StartFight();
            }
        });

        closeBtn.onClick.AddListener(() =>
        {
            buffInfo.SetActive(false);
        });

        CombinationBtn.onClick.AddListener(() =>
        {
            CombinationPanel panel = UIManager.Instance.ShowPanel<CombinationPanel>();
            panel.UpdateCombinationInfo();
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

        buffInfo.SetActive(false);
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
            UpdateTowerBuffInfoBtn(null);
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

        //����buff��ť
        UpdateTowerBuffInfoBtn(data);
        //���±仯��buff�ͷ�������û�������£�
        if (data.buffDatas.Any(buffData => buffData.buffType == nowBuffType)
            && TowerManager.Instance.oldTowerDatas.ContainsKey(towerName) && towerName == nowTowerInfoName)
        {
            UpdateBuffInfo(data.buffDatas.FirstOrDefault(buffData => buffData.buffType == nowBuffType));
        }
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
    /// ���·�����buff��Ϣ
    /// </summary>
    public void UpdateBuffInfo(BuffData data)
    {
        buffTitle.text = data.buffName;
        string info = "";
        switch (data.buffType)
        {
            case BuffType.Burn:
                info = $"����ʱ<color=red>{data.triggerChance}%</color>���ʸ���<color=red>����</color>��" +
                    $"ÿ�����<color=red>{data.damage}</color>���˺�������<color=red>{data.duration}s</color>��";
                break;
            case BuffType.Slow:
                info = $"����ʱ<color=red>{data.triggerChance}%</color>���ʸ���<color=red>����</color>��" +
                    $"�����ٶȱ�Ϊ<color=red>50%</color>,����<color=red>{data.duration}s</color>��";
                break;
            case BuffType.Stun:
                info = $"����ʱ<color=red>{data.triggerChance}%</color>���ʸ���<color=red>ѣ��</color>��" +
                    $"����ͣס����������<color=red>{data.duration}s</color>��";
                break;
        }
        buffTxt.text = info;
    }

    /// <summary>
    /// ���·�����buff��ť
    /// </summary>
    /// <param name="towerData"></param>
    public void UpdateTowerBuffInfoBtn(TowerData towerData)
    {
        foreach (TowerBuffInfoBtn btn in towerBuffBtnList)
        {
            Destroy(btn.gameObject);
        }
        towerBuffBtnList.Clear();

        if (towerData == null) return; //�����null��������ղ�չʾ

        foreach (BuffData data in towerData.buffDatas)
        {
            //��������
            GameObject towerBuffInfoObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/TowerBuffInfoBtn"));
            towerBuffInfoObj.transform.SetParent(buffBtnContainer, false);
            TowerBuffInfoBtn towerBuffInfoBtn = towerBuffInfoObj.GetComponent<TowerBuffInfoBtn>();
            towerBuffInfoBtn.InitInfo(data);
            //�������
            towerBuffBtnList.Add(towerBuffInfoBtn);
        }
    }

    /// <summary>
    /// ��ʼս��
    /// </summary>
    public void StartFight()
    {
        LevelManager.Instance.StartLevel("LevelScene1");
    }
}
