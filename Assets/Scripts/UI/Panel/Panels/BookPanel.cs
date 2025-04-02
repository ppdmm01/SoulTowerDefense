using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookPanel : BasePanel
{
    [Header("��ǩ��ť�б�")]
    public List<LabelBtn> labelBtnList;
    [Header("ҳ���б�")]
    public Transform pages; //ҳ�游����
    public List<Page> pageList;
    [Header("��Ʒ����")]
    public ItemInfo itemInfo;

    public Button closeBtn; //�رհ�ť
    public Button leftBtn; //��ҳ�水ť
    public Button rightBtn; //��ҳ�水ť

    private GameObject nowPageObj; //��ǰҳ�������
    private int nowPage; //��ǰҳ��
    private int maxPage; //���ҳ��

    public TextMeshProUGUI pageTxt; //��ʾ��ǰҳ��

    public override void Init()
    {
        pageList = new List<Page>();
        foreach (Transform child in pages)
        {
            pageList.Add(child.GetComponent<Page>());
        }
        CalculateMaxPage();
        UpdatePage(1);
        

        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<BookPanel>();
        });
        leftBtn.onClick.AddListener(() =>
        {
            nowPage -= 1;
            if (nowPage <= 0) nowPage = maxPage;
            UpdatePage(nowPage);
        });
        rightBtn.onClick.AddListener(() =>
        {
            nowPage += 1;
            if (nowPage > maxPage) nowPage = 1;
            UpdatePage(nowPage);
        });
    }

    //ͳ�����ҳ��
    public void CalculateMaxPage()
    {
        maxPage = 0;
        foreach (Page page in pageList)
        {
            if (page.pageNum > maxPage) maxPage = page.pageNum;
        }
    }

    /// <summary>
    /// ����ҳ��
    /// </summary>
    public void UpdatePage(int pageNum)
    {
        if (pageNum < 1 || pageNum > maxPage)
        {
            Debug.LogWarning("ҳ�����");
            return;
        }
        nowPage = pageNum;
        //���»���
        HideAllPage();
        foreach (Page page in pageList)
        {
            if (page.pageNum == pageNum) page.gameObject.SetActive(true);
        }
        pageTxt.text = nowPage + "/" + maxPage;
    }

    public void HideAllPage()
    {
        foreach (var page in pageList)
        {
            page.gameObject.SetActive(false);
        }
    }
}
