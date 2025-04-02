using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookPanel : BasePanel
{
    [Header("标签按钮列表")]
    public List<LabelBtn> labelBtnList;
    [Header("页面列表")]
    public Transform pages; //页面父对象
    public List<Page> pageList;
    [Header("物品属性")]
    public ItemInfo itemInfo;

    public Button closeBtn; //关闭按钮
    public Button leftBtn; //左页面按钮
    public Button rightBtn; //右页面按钮

    private GameObject nowPageObj; //当前页面的物体
    private int nowPage; //当前页面
    private int maxPage; //最大页面

    public TextMeshProUGUI pageTxt; //显示当前页面

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

    //统计最大页面
    public void CalculateMaxPage()
    {
        maxPage = 0;
        foreach (Page page in pageList)
        {
            if (page.pageNum > maxPage) maxPage = page.pageNum;
        }
    }

    /// <summary>
    /// 更新页面
    /// </summary>
    public void UpdatePage(int pageNum)
    {
        if (pageNum < 1 || pageNum > maxPage)
        {
            Debug.LogWarning("页面溢出");
            return;
        }
        nowPage = pageNum;
        //更新画面
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
