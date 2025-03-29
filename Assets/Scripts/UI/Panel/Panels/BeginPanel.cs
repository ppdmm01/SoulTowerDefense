using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button continueBtn; //继续游戏
    public Button playBtn; //新游戏
    public Button settingBtn;
    public Button aboutBtn;
    public Button quitBtn;

    public override void Init()
    {
        //是否显示继续游戏按钮
        if (GameDataManager.Instance.mapData != null && GameDataManager.Instance.mapData.nodes != null) 
            continueBtn.gameObject.SetActive(true);
        else 
            continueBtn.gameObject.SetActive(false);

        continueBtn.onClick.AddListener(PlayGame);

        playBtn.onClick.AddListener(() =>
        {
            if (continueBtn.gameObject.activeSelf)
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("是否开始新游戏？", PlayNewGame);
            else
                //开始游戏
                PlayNewGame();
        });

        settingBtn.onClick.AddListener(() =>
        {
            //打开设置面板
            UIManager.Instance.ShowPanel<SettingPanel>();
        });

        aboutBtn.onClick.AddListener(() =>
        {
            //打开制作名单
            UIManager.Instance.ShowPanel<AboutPanel>();
        });

        quitBtn.onClick.AddListener(() =>
        {
            //退出游戏
            Application.Quit();
            //TODO：提示是否确定退出
        });
    }

    //开始游戏
    private void PlayGame()
    {
        canvasGroup.blocksRaycasts = false;
        UIManager.Instance.LoadScene("MapScene",
        () =>
        {
            UIManager.Instance.HidePanel<BeginPanel>(); //关闭开始界面
        },
        () =>
        {
            UIManager.Instance.ShowPanel<MapPanel>(); //显示地图界面
        });
    }

    private void PlayNewGame()
    {
        canvasGroup.blocksRaycasts = false;
        //清理所有数据
        GameDataManager.Instance.ClearGameData();
        UIManager.Instance.LoadScene("MapScene",
        () =>
        {
            UIManager.Instance.HidePanel<BeginPanel>(); //关闭开始界面
            UIManager.Instance.ShowPanel<MapPanel>();
            UIManager.Instance.ShowPanel<SelectPanel>().UpdateTowerItem(); //开始新游戏，选择一个防御塔
        });
    }
}
