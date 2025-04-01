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
    private PlayerState oldPlayerState; //��¼�����ǰ��״̬
    public override void Init()
    {
        oldPlayerState = PlayerStateManager.Instance.CurrentState;
        PlayerStateManager.Instance.ChangeState(PlayerState.Menu);

        cancelBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<MenuPanel>();
            PlayerStateManager.Instance.ChangeState(oldPlayerState); //���Ļ��ϵ�״̬
        });
        settingBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SettingPanel>();
        });
        quitBtn.onClick.AddListener(() =>
        {
            //����浵����ͼ���˳�ʱ�������������������ˣ�MapManager��LevelManager��
            GameDataManager.Instance.SaveGameResData();
            GameDataManager.Instance.SaveGridData();
            //����ս��
            LevelManager.Instance.Clear();
            //�ص����˵�
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
                //ִ���¼�
                hideCallBack?.Invoke();
            }
        }
    }

    //�ȴ�һ֡�رչؿ�
    private IEnumerator WaitForOneFrameToCloseLevel()
    {
        yield return null; //��1֡
        LevelManager.Instance.isInLevel = false;
    }
}
