using System.Collections;
using System.Collections.Generic;
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

    public override void Init()
    {
        quitBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<StorePanel>();
            UIManager.Instance.HidePanel<BagPanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
            SceneManager.LoadSceneAsync("MapScene");
        });
        refreshBtn.onClick.AddListener(() =>
        {
            RefreshItems();
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
        StartCoroutine(RefreshRoutine()); //��һ֡��ˢ����Ʒ
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //�򱳰��������������Ʒ����
        foreach (var storeGrid in storeGrids)
            BagManager.Instance.AddGrid(storeGrid);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //�򱳰����������Ƴ���Ʒ����
        foreach (var storeGrid in storeGrids)
        {
            BagManager.Instance.ClearAllItem(storeGrid);
            BagManager.Instance.RemoveGrid(storeGrid);
        }
    }

    /// <summary>
    /// ˢ���̵���Ʒ
    /// </summary>
    public void RefreshItems()
    {
        foreach (var storeGrid in storeGrids)
            storeGrid.RefreshItem(ItemManager.Instance.GetRandomItemData(1)[0]);
    }

    private IEnumerator RefreshRoutine()
    {
        yield return null;
        RefreshItems();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
