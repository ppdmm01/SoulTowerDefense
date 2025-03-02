using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button playBtn;
    public Button settingBtn;
    public Button aboutBtn;
    public Button quitBtn;

    public override void Init()
    {
        playBtn.onClick.AddListener(() =>
        {
            //TODO：开始游戏
            UIManager.Instance.HidePanel<BeginPanel>();
            UIManager.Instance.ShowPanel<BagPanel>();
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
}
