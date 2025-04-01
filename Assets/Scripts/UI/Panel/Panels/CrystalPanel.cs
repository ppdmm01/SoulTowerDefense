using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrystalPanel : BasePanel
{
    public BagGrid bag;
    public Button closeBtn;
    [Header("物品信息")]
    public GameObject ItemInfoObj;
    public override void Init()
    {
        HideItemInfo();
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<CrystalPanel>();
            Time.timeScale = 1; //时间恢复
        });
    }

    protected override void Update()
    {
        if (canvasGroup.alpha < 1 && isShow)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                Time.timeScale = 0; //时间暂停
                canvasGroup.alpha = 1;
            }
        }

        if (canvasGroup.alpha > 0 && !isShow)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //执行事件
                hideCallBack?.Invoke();
            }
        }
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //向背包管理器中添加背包
        GridManager.Instance.AddGrid(bag);
        //读取网格数据并更新
        GridData bagData = GameDataManager.Instance.GetGridData(bag.gridName);
        if (bagData != null)
            bag.UpdateGrid(bagData);
        //背包物品禁止移动
        bag.isLocked = true;
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //背包物品恢复移动
        bag.isLocked = false;
        //保存网格数据
        GridData bagData = new GridData(bag.gridName, bag.items);
        GameDataManager.Instance.UpdateGridData(bagData);
        GameDataManager.Instance.SaveGridData();
        //清空物品
        GridManager.Instance.ClearAllItem(bag, false);
        //向背包管理器中移除背包
        GridManager.Instance.RemoveGrid(bag);
    }

    /// <summary>
    /// 显示物品信息
    /// </summary>
    public void ShowItemInfo(Item item)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(item);
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
