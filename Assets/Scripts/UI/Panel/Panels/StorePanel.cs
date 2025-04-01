using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �������
/// </summary>
public class StorePanel : BasePanel
{
    [Header("��ť")]
    public Button quitBtn; //�˳��̵갴ť
    public Button refreshBtn; //ˢ�°�ť

    [Header("�̵������")]
    public List<StoreGrid> storeGrids; //�̵���

    public TextMeshProUGUI refreshPriceTxt; //ˢ�¼۸��ı�

    private int nowRefreshPrice; //��ǰˢ������̫��

    public override void Init()
    {
        nowRefreshPrice = 10;
        refreshPriceTxt.text = nowRefreshPrice.ToString();
        quitBtn.onClick.AddListener(() =>
        {
            canvasGroup.blocksRaycasts = false;
            UIManager.Instance.HidePanel<BagPanel>();
            UIManager.Instance.HidePanel<StorePanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
        });

        refreshBtn.onClick.AddListener(() =>
        {
            //����һ����Դ������
            AudioManager.Instance.PlaySound("SoundEffect/Bell");
            if (GameResManager.Instance.GetTaixuNum() >= nowRefreshPrice)
            {
                GameResManager.Instance.AddTaixuNum(-nowRefreshPrice);
                nowRefreshPrice += 5; //ˢ�¼۸�����
                refreshPriceTxt.text = nowRefreshPrice.ToString();
                RefreshItems();
            }
            else
            {
                UIManager.Instance.ShowTipInfo("̫�鲻�㣬ˢ��ʧ��");
            }
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
        panel.SetTitle("����");

        //ˢ����Ʒ
        RefreshItems();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //�򱳰��������������Ʒ����
        foreach (var storeGrid in storeGrids)
            GridManager.Instance.AddGrid(storeGrid);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //�򱳰����������Ƴ���Ʒ����
        foreach (var storeGrid in storeGrids)
        {
            GridManager.Instance.ClearAllItem(storeGrid, false);
            GridManager.Instance.RemoveGrid(storeGrid);
        }
    }

    /// <summary>
    /// ˢ���̵���Ʒ
    /// </summary>
    public void RefreshItems()
    {
        foreach (var storeGrid in storeGrids)
        {
            storeGrid.ForceUpdateGridLayout();
            storeGrid.RefreshItem(ItemManager.Instance.GetRandomItemData(1)[0]);
        }
    }
}
