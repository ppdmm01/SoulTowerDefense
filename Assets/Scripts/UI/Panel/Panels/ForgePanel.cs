using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ForgePanel : BasePanel
{
    [Header("��ť")]
    public Button quitBtn; //�˳���ť
    public Button SynthesisBtn; //�ϳɰ�ť

    [Header("�������")]
    public ForgeGrid forgeGrid; //�ϳ��õ�����
    public BaseGrid productGrid; //���ó�Ʒ������
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
            forgeGrid.Synthesis(); //�ϳ���Ʒ
        });

        //��ʾ�������
        BagPanel bagPanel = UIManager.Instance.ShowPanel<BagPanel>();
        if (bagPanel != null)
        {
            bagPanel.transform.SetAsLastSibling();
        }

        //���¶�����
        TopColumnPanel panel = UIManager.Instance.ShowPanel<TopColumnPanel>();
        panel.transform.SetAsLastSibling();
        if (panel != null)
        {
            panel.ShowBtn(TopColumnBtnType.Book, TopColumnBtnType.Map, TopColumnBtnType.Menu);
        }
        panel.SetTitle("���췻");
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //�򱳰�����������ӱ���
        BagManager.Instance.AddGrid(forgeGrid);
        BagManager.Instance.AddGrid(productGrid);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        BagManager.Instance.ClearAllItem(forgeGrid);
        BagManager.Instance.ClearAllItem(productGrid);
        //�򱳰����������Ƴ�����
        BagManager.Instance.RemoveGrid(forgeGrid);
        BagManager.Instance.RemoveGrid(productGrid);
    }
}
