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
            //TODO����ʼ��Ϸ
            UIManager.Instance.HidePanel<BeginPanel>();
            UIManager.Instance.ShowPanel<BagPanel>();
        });

        settingBtn.onClick.AddListener(() =>
        {
            //���������
            UIManager.Instance.ShowPanel<SettingPanel>();
        });

        aboutBtn.onClick.AddListener(() =>
        {
            //����������
            UIManager.Instance.ShowPanel<AboutPanel>();
        });

        quitBtn.onClick.AddListener(() =>
        {
            //�˳���Ϸ
            Application.Quit();
            //TODO����ʾ�Ƿ�ȷ���˳�
        });
    }
}
