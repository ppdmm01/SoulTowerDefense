using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : BasePanel
{
    public Button cancelBtn;
    public Button settingBtn;
    public Button quitBtn;
    private PlayerState oldPlayerState; //记录打开面板前的状态
    public override void Init()
    {
        oldPlayerState = PlayerStateManager.Instance.CurrentState;
        PlayerStateManager.Instance.ChangeState(PlayerState.Menu);

        cancelBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<MenuPanel>();
            PlayerStateManager.Instance.ChangeState(oldPlayerState); //更改回老的状态
        });
        settingBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SettingPanel>();
        });
        quitBtn.onClick.AddListener(() =>
        {
            //保存存档（地图在退出时被其他管理器处理保存了：MapManager和LevelManager）
            GameDataManager.Instance.SaveGameResData();
            GameDataManager.Instance.SaveGridData();
            //清理战场
            LevelManager.Instance.Clear();
            //回到主菜单
            UIManager.Instance.LoadScene("BeginScene", () =>
            {
                UIManager.Instance.HideAllPanel();
                UIManager.Instance.ShowPanel<BeginPanel>();
            });
        });
    }

    protected override void Update()
    {
        if (canvasGroup.alpha < 1 && isShow)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
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

    //等待一帧关闭关卡
    private IEnumerator WaitForOneFrameToCloseLevel()
    {
        yield return null; //等1帧
        LevelManager.Instance.isInLevel = false;
    }
}
