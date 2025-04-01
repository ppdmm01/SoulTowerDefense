using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 商铺面板
/// </summary>
public class StorePanel : BasePanel
{
    [Header("按钮")]
    public Button quitBtn; //退出商店按钮
    public Button refreshBtn; //刷新按钮

    [Header("商店栏相关")]
    public List<StoreGrid> storeGrids; //商店栏

    public TextMeshProUGUI refreshPriceTxt; //刷新价格文本

    private int nowRefreshPrice; //当前刷新所需太虚

    public override void Init()
    {
        nowRefreshPrice = 10;
        refreshPriceTxt.text = nowRefreshPrice.ToString();
        quitBtn.onClick.AddListener(() =>
        {
            canvasGroup.blocksRaycasts = false;
            UIManager.Instance.HidePanel<BagPanel>();
            UIManager.Instance.HidePanel<StorePanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
        });

        refreshBtn.onClick.AddListener(() =>
        {
            //花费一定资源率先你
            AudioManager.Instance.PlaySound("SoundEffect/Bell");
            if (GameResManager.Instance.GetTaixuNum() >= nowRefreshPrice)
            {
                GameResManager.Instance.AddTaixuNum(-nowRefreshPrice);
                nowRefreshPrice += 5; //刷新价格上升
                refreshPriceTxt.text = nowRefreshPrice.ToString();
                RefreshItems();
            }
            else
            {
                UIManager.Instance.ShowTipInfo("太虚不足，刷新失败");
            }
        });

        //显示背包面板
        BagPanel bagPanel = UIManager.Instance.ShowPanel<BagPanel>();
        if (bagPanel != null)
        {
            bagPanel.transform.SetAsLastSibling();
        }

        //更新顶部栏
        TopColumnPanel panel = UIManager.Instance.ShowPanel<TopColumnPanel>();
        panel.transform.SetAsLastSibling();
        if (panel != null)
        {
            panel.ShowBtn(TopColumnBtnType.Book, TopColumnBtnType.Map, TopColumnBtnType.Menu);
        }
        panel.SetTitle("商铺");

        //刷新物品
        RefreshItems();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //向背包管理器中添加商品网格
        foreach (var storeGrid in storeGrids)
            GridManager.Instance.AddGrid(storeGrid);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //向背包管理器中移除商品网格
        foreach (var storeGrid in storeGrids)
        {
            GridManager.Instance.ClearAllItem(storeGrid, false);
            GridManager.Instance.RemoveGrid(storeGrid);
        }
    }

    /// <summary>
    /// 刷新商店物品
    /// </summary>
    public void RefreshItems()
    {
        foreach (var storeGrid in storeGrids)
        {
            storeGrid.ForceUpdateGridLayout();
            storeGrid.RefreshItem(ItemManager.Instance.GetRandomItemData(1)[0]);
        }
    }
}
