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
    [Header("按钮")]
    public Button startFightBtn; //开始战斗按钮
    public Button CombinationBtn; //组合信息按钮

    [Header("防御塔信息")]
    public ScrollRect towerSr;
    public string nowTowerInfoName; //当前展示的防御塔名
    private List<TowerInfoBtn> towerInfoBtnList; //防御塔信息按钮列表
    public TowerInfo towerInfo; //防御塔信息
    private float nowWeight; //当前所有防御塔信息宽度
    private List<CombinationSO> combinationList; //激活的组合信息列表

    [Header("防御塔buff相关")]
    public TextMeshProUGUI buffTxt; //buff描述
    public TextMeshProUGUI buffTitle; //buff标题
    public GameObject buffInfo; //buff信息面板
    public Button closeBtn; //关闭buff信息按钮
    public List<TowerBuffInfoBtn> towerBuffBtnList; //防御塔buff信息按钮列表

    public Transform buffBtnContainer; //存放buff按钮
    public BuffType nowBuffType; //当前展示的buff类型

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
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("你没有上场任何防御塔，确定继续吗？", StartFight);
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

        //显示背包面板
        BagPanel bagPanel = UIManager.Instance.ShowPanel<BagPanel>();
        if (bagPanel != null)
        {
            bagPanel.transform.SetAsLastSibling();
        }

        //更新顶部栏
        TopColumnPanel topPanel = UIManager.Instance.ShowPanel<TopColumnPanel>();
        if (topPanel != null)
        {
            topPanel.transform.SetAsLastSibling();
            topPanel.ShowBtn(TopColumnBtnType.Book, TopColumnBtnType.Map, TopColumnBtnType.Menu);
        }
        topPanel.SetTitle("战前准备");

        buffInfo.SetActive(false);
    }

    /// <summary>
    /// 更新展示的防御塔信息
    /// </summary>
    public void UpdateTowerInfo(string towerName)
    {
        if (towerName == "" || !TowerManager.Instance.towerDatas.ContainsKey(towerName)) //置空
        {
            towerInfo.SetNull();
            towerInfo.RemoveAllAttributeInfo();
            UpdateTowerBuffInfoBtn(null);
            return;
        }

        TowerData data = TowerManager.Instance.towerDatas[towerName]; //获取当前展示的防御塔数据
        towerInfo.RemoveAllAttributeInfo(); //清空防御塔信息的所有属性

        //更改塔的数据
        if (TowerManager.Instance.oldTowerDatas.ContainsKey(towerName) && towerName == nowTowerInfoName) //展示防御塔不变时才需要显示数值变化
        {
            TowerData oldData = TowerManager.Instance.oldTowerDatas[towerName]; //变化前的数据
            towerInfo.SetChangedInfo(data, oldData); //显示数据变化
        }
        else
            towerInfo.SetInfo(data);

        //更新buff按钮
        UpdateTowerBuffInfoBtn(data);
        //更新变化（buff和防御塔都没变过则更新）
        if (data.buffDatas.Any(buffData => buffData.buffType == nowBuffType)
            && TowerManager.Instance.oldTowerDatas.ContainsKey(towerName) && towerName == nowTowerInfoName)
        {
            UpdateBuffInfo(data.buffDatas.FirstOrDefault(buffData => buffData.buffType == nowBuffType));
        }
    }

    /// <summary>
    /// 更新防御塔信息按钮
    /// </summary>
    public void UpdateTowerInfoBtn()
    {
        //数据溢出，清理列表中多余的数据
        if (TowerManager.Instance.towerDatas.Count < towerInfoBtnList.Count)
        {
            for (int i = towerInfoBtnList.Count - 1; i >= TowerManager.Instance.towerDatas.Count; i--)
            {
                //销毁对象
                Destroy(towerInfoBtnList[i].gameObject);
                //移除数据
                towerInfoBtnList.RemoveAt(i);
            }
        }
        //数据不足，创建新的数据
        else if (TowerManager.Instance.towerDatas.Count > towerInfoBtnList.Count)
        {
            //创建对象
            GameObject towerInfoObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/TowerInfoBtn"));
            towerInfoObj.transform.SetParent(towerSr.content, false);
            TowerInfoBtn towerInfo = towerInfoObj.GetComponent<TowerInfoBtn>();
            //添加数据
            towerInfoBtnList.Add(towerInfo);
        }

        //更改塔的数据
        int index = 0;
        nowWeight = 0;
        foreach (string towerName in TowerManager.Instance.towerDatas.Keys)
        {
            if(nowTowerInfoName == "" || !TowerManager.Instance.towerDatas.ContainsKey(nowTowerInfoName))
            {
                nowTowerInfoName = towerName;
                UpdateTowerInfo(nowTowerInfoName); //如果是第一次更新按钮，则自动显示第一个创建的
            }

            TowerInfoBtn towerInfoBtn = towerInfoBtnList[index];
            TowerData data = TowerManager.Instance.towerDatas[towerName]; //新数据
            towerInfoBtn.InitInfo(data); //初始化按钮
            nowWeight += towerSr.content.GetComponent<GridLayoutGroup>().cellSize.x;
            index++;
        }
        
        //如果没有内容，则置空
        if (TowerManager.Instance.towerDatas.Count == 0)
        {
            nowTowerInfoName = "";
            UpdateTowerInfo(nowTowerInfoName);
        }
        //更新ScrollView内容宽度
        towerSr.content.sizeDelta = new Vector2(nowWeight, towerSr.content.sizeDelta.y);
    }

    /// <summary>
    /// 更新防御塔buff信息
    /// </summary>
    public void UpdateBuffInfo(BuffData data)
    {
        buffTitle.text = data.buffName;
        string info = "";
        switch (data.buffType)
        {
            case BuffType.Burn:
                info = $"攻击时<color=red>{data.triggerChance}%</color>几率附带<color=red>灼烧</color>：" +
                    $"每秒造成<color=red>{data.damage}</color>点伤害，持续<color=red>{data.duration}s</color>。";
                break;
            case BuffType.Slow:
                info = $"攻击时<color=red>{data.triggerChance}%</color>几率附带<color=red>缓慢</color>：" +
                    $"敌人速度变为<color=red>50%</color>,持续<color=red>{data.duration}s</color>。";
                break;
            case BuffType.Stun:
                info = $"攻击时<color=red>{data.triggerChance}%</color>几率附带<color=red>眩晕</color>：" +
                    $"敌人停住不动，持续<color=red>{data.duration}s</color>。";
                break;
        }
        buffTxt.text = info;
    }

    /// <summary>
    /// 更新防御塔buff按钮
    /// </summary>
    /// <param name="towerData"></param>
    public void UpdateTowerBuffInfoBtn(TowerData towerData)
    {
        foreach (TowerBuffInfoBtn btn in towerBuffBtnList)
        {
            Destroy(btn.gameObject);
        }
        towerBuffBtnList.Clear();

        if (towerData == null) return; //如果传null，代表清空不展示

        foreach (BuffData data in towerData.buffDatas)
        {
            //创建对象
            GameObject towerBuffInfoObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/TowerBuffInfoBtn"));
            towerBuffInfoObj.transform.SetParent(buffBtnContainer, false);
            TowerBuffInfoBtn towerBuffInfoBtn = towerBuffInfoObj.GetComponent<TowerBuffInfoBtn>();
            towerBuffInfoBtn.InitInfo(data);
            //添加数据
            towerBuffBtnList.Add(towerBuffInfoBtn);
        }
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartFight()
    {
        LevelManager.Instance.StartLevel("LevelScene1");
    }
}
