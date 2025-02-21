using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    [Header("文本信息")]
    public TextMeshProUGUI bagItemInfo;

    [Header("按钮")]
    public Button arrangeBtn; //排序按钮
    //添加物品按钮
    public Button AdditemBtn;

    //放置物品的地方
    private Transform itemsTrans;
    [Header("背包相关")]
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
    /// 更新信息
    /// </summary>
    public void UpdateMessage()
    {
        string info = "";
        foreach (TowerData towerData in TowerManager.Instance.towers.Values)
        {
            info += "名字：" + towerData.towerName + "\n";
            info += "描述：" + towerData.description + "\n";
            info += "伤害：" + towerData.damage + "\n";
            info += "攻击范围：" + towerData.range + "\n";
            info += "攻击间隔：" + towerData.interval + "\n";
            info += "产量：" + towerData.output + "\n";
            info += "生产间隔：" + towerData.cooldown + "\n";
            info += "----------------------\n";
        }

        bagItemInfo.text = info;
    }
}
