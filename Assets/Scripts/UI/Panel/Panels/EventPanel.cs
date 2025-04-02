using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 事件面板（最后一天才加上的，写的很赶，所以代码不太美观）
/// </summary>
public class EventPanel : BasePanel
{
    public TextMeshProUGUI contentText;
    public Button OkBtn;
    public Button CancelBtn;
    public TextMeshProUGUI OkBtnText;
    public TextMeshProUGUI CancelBtnText;

    public GameObject EventBg; //刚进入事件的两个按钮的面板
    public GameObject rewardBg; //奖励面板
    [Header("回答问题的面板")]
    public GameObject QuestionBg; //回答问题的面板
    public GameObject questionRewardBg; //问题奖励面板
    [Header("宝藏相关面板")]
    public GameObject treasureBg; //宝箱面板
    public GameObject enemyBg; //敌人面板
    [Header("水晶碎片相关面板")]
    public GameObject crystalBg; //水晶碎片面板

    [Header("确定按钮")]
    public Button sureBtn;

    [Header("回答问题的按钮")]
    public Button btnA;
    public TextMeshProUGUI txtA;
    public Button btnB;
    public TextMeshProUGUI txtB;
    public Button btnC;
    public TextMeshProUGUI txtC;
    public Button btnD;
    public TextMeshProUGUI txtD;
    private int nowSelect; //当前选择

    [Header("储物箱")]
    public BaseGrid storageBox;
    public Button arrangeBtn;

    [Header("奖励")]
    public SelectItemGrid rewardGrid;
    [Header("物品信息")]
    public GameObject ItemInfoObj;

    public override void Init()
    {
        HideItemInfo();
        sureBtn.gameObject.SetActive(false);
        treasureBg.SetActive(false);
        QuestionBg.SetActive(false);
        EventBg.SetActive(true);
        rewardBg.SetActive(false);
        enemyBg.SetActive(false);
        questionRewardBg.SetActive(false);
        crystalBg.SetActive(false);

        sureBtn.onClick.AddListener(() =>
        {
            canvasGroup.blocksRaycasts = false;
            UIManager.Instance.HidePanel<EventPanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
        });
        arrangeBtn.onClick.AddListener(() =>
        {
            GridManager.Instance.GetGridByName(storageBox.gridName).AutoArrange();
        });
        //随机一个事件
        RandomEvent();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //向背包管理器中添加背包
        GridManager.Instance.AddGrid(storageBox);
        //读取网格数据并更新
        GridData storageBoxData = GameDataManager.Instance.GetGridData(storageBox.gridName);
        if (storageBoxData != null)
            storageBox.UpdateGrid(storageBoxData);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //保存网格数据
        GridData storageBoxData = new GridData(storageBox.gridName, storageBox.items);
        GameDataManager.Instance.UpdateGridData(storageBoxData);
        GameDataManager.Instance.SaveGridData();
        //清空物品
        GridManager.Instance.ClearAllItem(rewardGrid, false);
        GridManager.Instance.ClearAllItem(storageBox, false);
        //向背包管理器中移除背包
        GridManager.Instance.RemoveGrid(storageBox);
    }

    //随机一个事件
    public void RandomEvent()
    {
        int randomNum = Random.Range(0, 100);
        if (randomNum < 40)
            QuestionEvent();
        else if (randomNum < 70)
            TreasureEvent();
        else
            CrystalEvent();
    }

    //问号事件
    private void QuestionEvent()
    {
        contentText.text = "你遇到了一个头带问号的小精灵，小精灵说：\n" +
            "<color=purple>“嘻嘻嘻~，你好冒险者，要玩问答游戏吗？ 回答对了有奖励哦！”</color>。";
        OkBtn.onClick.RemoveAllListeners();
        CancelBtn.onClick.RemoveAllListeners();
        OkBtnText.text = "同意";
        CancelBtnText.text = "不理他";
        OkBtn.onClick.AddListener(() =>
        {
            EventBg.SetActive(false);
            //进入回答问题
            BeginQuestion();
        });
        CancelBtn.onClick.AddListener(() =>
        {
            contentText.text = "你头也不回的走了";
            EventBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
    }

    //宝藏事件
    private void TreasureEvent()
    {
        treasureBg.SetActive(true);
        contentText.text = "你发现了一个宝箱！你决定...";
        OkBtn.onClick.RemoveAllListeners();
        CancelBtn.onClick.RemoveAllListeners();
        OkBtnText.text = "打开";
        CancelBtnText.text = "放弃";
        OkBtn.onClick.AddListener(() =>
        {
            EventBg.SetActive(false);
            //打开宝箱
            OpenChest();
        });
        CancelBtn.onClick.AddListener(() =>
        {
            contentText.text = "你决定不冒这个险";
            EventBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
    }

    //水晶事件
    private void CrystalEvent()
    {
        crystalBg.SetActive(true);
        contentText.text = "你发现了一个水晶碎片！你决定...";
        OkBtn.onClick.RemoveAllListeners();
        CancelBtn.onClick.RemoveAllListeners();
        OkBtnText.text = "吸收";
        CancelBtnText.text = "打碎";
        OkBtn.onClick.AddListener(() =>
        {
            contentText.text = $"你吸收了水晶，元气水晶生命值<color=green>+20</color>！";
            TopColumnPanel panel = UIManager.Instance.ShowPanel<TopColumnPanel>();
            int nowHp = Mathf.Clamp(GameResManager.Instance.gameRes.coreNowHp + 20,0,GameResManager.Instance.gameRes.coreMaxHp);
            GameResManager.Instance.UpdateCoreHp(nowHp);
            panel.UpdateHp(nowHp, GameResManager.Instance.gameRes.coreMaxHp);
            EventBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
        CancelBtn.onClick.AddListener(() =>
        {
            int randomValue = Random.Range(50,80);
            contentText.text = $"你打碎了水晶，获得<sprite=8><color=purple>{randomValue}</color>！";
            GameResManager.Instance.AddTaixuNum(randomValue);
            EventBg.SetActive(false);
            crystalBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
    }

    #region 问答事件
    //开始问答
    public void BeginQuestion()
    {
        QuestionBg.gameObject.SetActive(true);
        QuestionSO data = QuestionManager.Instance.GetRandomQuestion();
        contentText.text = data.questionContent;
        txtA.text = data.ansA;
        txtB.text = data.ansB;
        txtC.text = data.ansC;
        txtD.text = data.ansD;
        btnA.onClick.RemoveAllListeners();
        btnB.onClick.RemoveAllListeners();
        btnC.onClick.RemoveAllListeners();
        btnD.onClick.RemoveAllListeners();
        btnA.onClick.AddListener(() =>
        {
            nowSelect = 1;
            CheckAns(data);
        });
        btnB.onClick.AddListener(() =>
        {
            nowSelect = 2;
            CheckAns(data);
        });
        btnC.onClick.AddListener(() =>
        {
            nowSelect = 3;
            CheckAns(data);
        });
        btnD.onClick.AddListener(() =>
        {
            nowSelect = 4;
            CheckAns(data);
        });
    }

    //检测答案
    public void CheckAns(QuestionSO data)
    {
        btnA.interactable = false;
        btnB.interactable = false;
        btnC.interactable = false;
        btnD.interactable = false;
        string ans = "";
        switch (data.ans)
        {
            case 1: ans = "A"; break;
            case 2: ans = "B"; break;
            case 3: ans = "C"; break;
            case 4: ans = "D"; break;
        }
        if (nowSelect != data.ans)
        {
            //回答错误
            contentText.text = $"小精灵：<color=purple>”回答错误！ 正确答案是{ans}，下次再来吧，嘻嘻~“</color>";
        }
        else
        {
            //回答正确，获得奖励
            contentText.text = "小精灵：<color=purple>”回答正确！ 领取你的奖励吧，嘻嘻~“</color>";
            questionRewardBg.SetActive(true);
            QuestionBg.SetActive(false);
            rewardBg.SetActive(true);
            AddRewardItem(data.reward);
        }

        //列出退出按钮
        sureBtn.gameObject.SetActive(true);
    }
    #endregion

    #region 宝箱事件
    //打开宝箱
    public void OpenChest()
    {
        treasureBg.SetActive(false);
        Debug.Log(treasureBg.activeSelf);
        int randomNum = Random.Range(0, 100);
        if (randomNum < 40)
        {
            //受到袭击
            contentText.text = "你遭受到了袭击！水晶生命值<color=red>-5</color>";
            enemyBg.SetActive(true);
            TopColumnPanel panel = UIManager.Instance.ShowPanel<TopColumnPanel>();
            GameResManager.Instance.UpdateCoreHp(GameResManager.Instance.gameRes.coreNowHp - 5);
            panel.UpdateHp(GameResManager.Instance.gameRes.coreNowHp,GameResManager.Instance.gameRes.coreMaxHp);
            if (GameResManager.Instance.gameRes.coreNowHp <= 0)
            {
                LevelManager.Instance.LoseGame(); //游戏失败
            }
        }
        else
        {
            contentText.text = "你获得了奖励！";
            //奖励
            rewardBg.SetActive(true);
            AddRandomItem();
        }

        //列出退出按钮
        sureBtn.gameObject.SetActive(true);
    }
    #endregion

    //获得奖励物品
    public void AddRewardItem(ItemSO data)
    {
        data = ItemManager.Instance.GetItemDataById(data.id);
        rewardGrid.ForceUpdateGridLayout();
        GridManager.Instance.AddItem(data.id, rewardGrid);
    }

    //添加随机奖励物品
    public void AddRandomItem()
    {
        ItemSO data = ItemManager.Instance.GetRandomItemData(1)[0];
        rewardGrid.ForceUpdateGridLayout();
        GridManager.Instance.AddItem(data.id, rewardGrid);
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
