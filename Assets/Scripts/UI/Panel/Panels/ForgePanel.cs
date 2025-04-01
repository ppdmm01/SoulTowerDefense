using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ForgePanel : BasePanel
{
    [Header("按钮")]
    public Button quitBtn; //退出按钮
    public Button SynthesisBtn; //合成按钮

    [Header("背包相关")]
    public ForgeGrid forgeGrid; //合成用的网格
    public BaseGrid productGrid; //放置成品的网格

    public Transform forgeTrans; //成品位置

    [Header("合成提示")]
    public GameObject OkObj; //可以合成
    public GameObject NoObj; //不可以合成

    public TextMeshProUGUI priceTxt; //合成价格

    public override void Init()
    {
        priceTxt.gameObject.SetActive(false);

        quitBtn.onClick.AddListener(() =>
        {
            if (forgeGrid.items.Count > 0 || productGrid.items.Count > 0)
            {
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("你还有未取走的物品，确定离开吗？", Quit);
            }
            else
            {
                Quit();
            }
        });
        SynthesisBtn.onClick.AddListener(() =>
        {
            forgeGrid.Synthesis(); //合成物品
        });

        UpdateTip(false,0);

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
        panel.SetTitle("锻造坊");
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //向背包管理器中添加背包
        GridManager.Instance.AddGrid(forgeGrid);
        GridManager.Instance.AddGrid(productGrid);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        GridManager.Instance.ClearAllItem(forgeGrid, false);
        GridManager.Instance.ClearAllItem(productGrid, false);
        //向背包管理器中移除背包
        GridManager.Instance.RemoveGrid(forgeGrid);
        GridManager.Instance.RemoveGrid(productGrid);
    }

    /// <summary>
    /// 更新提示
    /// </summary>
    public void UpdateTip(bool isOk,int price)
    {
        if (isOk)
        {
            OkObj.SetActive(true);
            NoObj.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = $"<sprite=8>{price.ToString()}";
        }
        else
        {
            OkObj.SetActive(false);
            NoObj.SetActive(true);
            priceTxt.gameObject.SetActive(false);
        }
    }

    //退出面板
    private void Quit()
    {
        canvasGroup.blocksRaycasts = false;
        UIManager.Instance.HidePanel<BagPanel>();
        UIManager.Instance.HidePanel<ForgePanel>();
        UIManager.Instance.ShowPanel<MapPanel>();
    }
}
