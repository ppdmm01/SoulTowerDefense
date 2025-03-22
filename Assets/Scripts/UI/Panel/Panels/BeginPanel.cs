using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            canvasGroup.blocksRaycasts = false;
            //��ʼ��Ϸ
            UIManager.Instance.LoadScene("MapScene", 
            () =>
            {
                Debug.Log("�رտ�ʼ���");
                UIManager.Instance.HidePanel<BeginPanel>();
            },
            () =>
            {
                Debug.Log("��ʾ��ͼ���");
                UIManager.Instance.ShowPanel<MapPanel>();
            });
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
