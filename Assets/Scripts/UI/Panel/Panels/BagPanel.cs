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
    public Button AdditemBtn; //添加物品按钮
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
        AdditemBtn.onClick.AddListener(() =>
        {
            BagManager.Instance.AddRandomItem(3, BagManager.Instance.GetBagByName("storageBox"));
        });
        startFightBtn.onClick.AddListener(() =>
        {
            //战斗开始
            LevelManager.Instance.StartLevel("LevelScene1");
            UIManager.Instance.HidePanel<BagPanel>();
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
        if (TowerManager.Instance.towers.Count < towerInfoList.Count)
        {
            for (int i= towerInfoList.Count-1 ; i >= TowerManager.Instance.towers.Count; i--)
            {
                //销毁对象
                Destroy(towerInfoList[i].gameObject);
                //移除数据
                towerInfoList.RemoveAt(i);  
            }
        }
        //数据不足，创建新的数据
        else if (TowerManager.Instance.towers.Count > towerInfoList.Count)
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
        foreach (TowerData towerData in TowerManager.Instance.towers.Values)
        {
            TowerInfo towerInfo = towerInfoList[index];
            towerInfo.SetInfo(towerData, -nowHeight);
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
}
