using System.Collections;
using System.Collections.Generic;
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
    public override void Init()
    {
        quitBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ForgePanel>();
            UIManager.Instance.HidePanel<BagPanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
            SceneManager.LoadSceneAsync("MapScene");
        });
        SynthesisBtn.onClick.AddListener(() =>
        {
            forgeGrid.Synthesis(); //合成物品
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
        GridManager.Instance.ClearAllItem(forgeGrid);
        GridManager.Instance.ClearAllItem(productGrid);
        //向背包管理器中移除背包
        GridManager.Instance.RemoveGrid(forgeGrid);
        GridManager.Instance.RemoveGrid(productGrid);
    }
}
