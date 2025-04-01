using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public Transform forgeTrans; //��Ʒλ��

    [Header("�ϳ���ʾ")]
    public GameObject OkObj; //���Ժϳ�
    public GameObject NoObj; //�����Ժϳ�

    public TextMeshProUGUI priceTxt; //�ϳɼ۸�

    public override void Init()
    {
        priceTxt.gameObject.SetActive(false);

        quitBtn.onClick.AddListener(() =>
        {
            if (forgeGrid.items.Count > 0 || productGrid.items.Count > 0)
            {
                UIManager.Instance.ShowPanel<TipPanel>().SetInfo("�㻹��δȡ�ߵ���Ʒ��ȷ���뿪��", Quit);
            }
            else
            {
                Quit();
            }
        });
        SynthesisBtn.onClick.AddListener(() =>
        {
            forgeGrid.Synthesis(); //�ϳ���Ʒ
        });

        UpdateTip(false,0);

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
        GridManager.Instance.AddGrid(forgeGrid);
        GridManager.Instance.AddGrid(productGrid);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        GridManager.Instance.ClearAllItem(forgeGrid, false);
        GridManager.Instance.ClearAllItem(productGrid, false);
        //�򱳰����������Ƴ�����
        GridManager.Instance.RemoveGrid(forgeGrid);
        GridManager.Instance.RemoveGrid(productGrid);
    }

    /// <summary>
    /// ������ʾ
    /// </summary>
    public void UpdateTip(bool isOk,int price)
    {
        if (isOk)
        {
            OkObj.SetActive(true);
            NoObj.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = $"<sprite=8>{price.ToString()}";
        }
        else
        {
            OkObj.SetActive(false);
            NoObj.SetActive(true);
            priceTxt.gameObject.SetActive(false);
        }
    }

    //�˳����
    private void Quit()
    {
        canvasGroup.blocksRaycasts = false;
        UIManager.Instance.HidePanel<BagPanel>();
        UIManager.Instance.HidePanel<ForgePanel>();
        UIManager.Instance.ShowPanel<MapPanel>();
    }
}
