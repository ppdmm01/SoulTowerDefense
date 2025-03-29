using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button continueBtn; //������Ϸ
    public Button playBtn; //����Ϸ
    public Button settingBtn;
    public Button aboutBtn;
    public Button quitBtn;

    public override void Init()
    {
        //�Ƿ���ʾ������Ϸ��ť
        if (GameDataManager.Instance.mapData != null && GameDataManager.Instance.mapData.nodes != null) 
            continueBtn.gameObject.SetActive(true);
        else 
            continueBtn.gameObject.SetActive(false);

        continueBtn.onClick.AddListener(PlayGame);

        playBtn.onClick.AddListener(() =>
        {
            if (continueBtn.gameObject.activeSelf)
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("�Ƿ�ʼ����Ϸ��", PlayNewGame);
            else
                //��ʼ��Ϸ
                PlayNewGame();
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

    //��ʼ��Ϸ
    private void PlayGame()
    {
        canvasGroup.blocksRaycasts = false;
        UIManager.Instance.LoadScene("MapScene",
        () =>
        {
            UIManager.Instance.HidePanel<BeginPanel>(); //�رտ�ʼ����
        },
        () =>
        {
            UIManager.Instance.ShowPanel<MapPanel>(); //��ʾ��ͼ����
        });
    }

    private void PlayNewGame()
    {
        canvasGroup.blocksRaycasts = false;
        //������������
        GameDataManager.Instance.ClearGameData();
        UIManager.Instance.LoadScene("MapScene",
        () =>
        {
            UIManager.Instance.HidePanel<BeginPanel>(); //�رտ�ʼ����
            UIManager.Instance.ShowPanel<MapPanel>();
            UIManager.Instance.ShowPanel<SelectPanel>().UpdateTowerItem(); //��ʼ����Ϸ��ѡ��һ��������
        });
    }
}
