using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PreFightPanel : BasePanel
{
    [Header("按钮")]
    public Button startFightBtn; //开始战斗按钮

    [Header("防御塔信息")]
    public ScrollRect towerSr;
    public string nowTowerInfoName; //当前展示的防御塔名
    private List<TowerInfoBtn> towerInfoBtnList; //防御塔信息按钮列表
    public TowerInfo towerInfo; //防御塔信息
    private float nowWeight; //当前所有防御塔信息宽度

    public override void Init()
    {
        towerInfoBtnList = new List<TowerInfoBtn>();
        nowTowerInfoName = "";
        nowWeight = 0;

        startFightBtn.onClick.AddListener(() =>
        {
            if (TowerManager.Instance.towerDatas.Count == 0)
            {
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("你没有上场任何防御塔，确定继续吗？", StartFight);
            }
            else
            {
                StartFight();
            }
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
    /// 开始战斗
    /// </summary>
    public void StartFight()
    {
        //战斗开始
        LevelManager.Instance.StartLevel("LevelScene1");
        UIManager.Instance.HidePanel<PreFightPanel>();
        UIManager.Instance.HidePanel<BagPanel>();
    }
}
