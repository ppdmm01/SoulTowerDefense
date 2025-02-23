using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    [Header("按钮")]
    public Button arrangeBtn; //排序按钮
    public Button addItemBtn; //添加物品按钮
    public Button clearItemBtn; //清空物品按钮
    public Button startFightBtn; //开始战斗按钮

    [Header("防御塔信息")]
    public ScrollRect towerSr;
    private List<TowerInfo> towerInfoList; //防御塔信息列表
    private float nowHeight; //当前所有防御塔信息高度

    [Header("物品信息")]
    public GameObject ItemInfoObj;

    //放置物品的地方
    private Transform itemsTrans;
    [Header("背包相关")]
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
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("你没有上场任何防御塔，确定继续吗？", StartFight);
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
        //向背包管理器中添加背包
        BagManager.Instance.BagDic.Add(bag.bagName,bag);
        BagManager.Instance.BagDic.Add(storageBox.bagName, storageBox);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //向背包管理器中移除背包
        BagManager.Instance.BagDic.Remove(bag.bagName);
        BagManager.Instance.BagDic.Remove(storageBox.bagName);
    }

    /// <summary>
    /// 更新防御塔信息
    /// </summary>
    public void UpdateTowerInfo()
    {
        //数据溢出，清理列表中多余的数据
        if (TowerManager.Instance.towerDatas.Count < towerInfoList.Count)
        {
            for (int i= towerInfoList.Count-1 ; i >= TowerManager.Instance.towerDatas.Count; i--)
            {
                //销毁对象
                Destroy(towerInfoList[i].gameObject);
                //移除数据
                towerInfoList.RemoveAt(i);  
            }
        }
        //数据不足，创建新的数据
        else if (TowerManager.Instance.towerDatas.Count > towerInfoList.Count)
        {
            //创建对象
            GameObject towerInfoObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/TowerInfo"));
            towerInfoObj.transform.SetParent(towerSr.content, false);
            TowerInfo towerInfo = towerInfoObj.GetComponent<TowerInfo>();
            //添加数据
            towerInfoList.Add(towerInfo);
        }

        //清空剩余防御塔信息的所有属性
        foreach (TowerInfo towerInfo in towerInfoList)
            towerInfo.RemoveAllAttributeInfo();

        //更改塔的数据
        int index = 0;
        nowHeight = 0;
        foreach (string towerName in TowerManager.Instance.towerDatas.Keys)
        {
            TowerInfo towerInfo = towerInfoList[index];
            TowerData newData = TowerManager.Instance.towerDatas[towerName]; //新数据

            if (TowerManager.Instance.oldTowerDatas.ContainsKey(towerName))
            {
                TowerData oldData = TowerManager.Instance.oldTowerDatas[towerName]; //变化前的数据
                towerInfo.SetChangedInfo(newData, oldData, -nowHeight); //需要显示数据变化
            }
            else
                towerInfo.SetInfo(newData, -nowHeight);

            nowHeight += towerInfo.GetHeight();
            index++;
        }
        //更新ScrollView内容高度
        towerSr.content.sizeDelta = new Vector2(towerSr.content.sizeDelta.x, nowHeight);
    }

    /// <summary>
    /// 显示物品信息
    /// </summary>
    public void ShowItemInfo(ItemSO data)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(data);
    }

    /// <summary>
    /// 隐藏物品信息
    /// </summary>
    public void HideItemInfo()
    {
        ItemInfoObj.SetActive(false);
        ItemInfoObj.GetComponent<ItemInfo>().RemoveAllAttributeInfo();
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartFight()
    {
        //战斗开始
        LevelManager.Instance.StartLevel("LevelScene1");
        UIManager.Instance.HidePanel<BagPanel>();
    }
}
