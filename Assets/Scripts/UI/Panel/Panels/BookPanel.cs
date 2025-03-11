using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class BookPanel : BasePanel
{
    [Header("标签按钮列表")]
    public List<LabelBtn> labelBtnList;
    [Header("页面列表")]
    public List<GameObject> pageList;
    [Header("物品属性")]
    public ItemInfo itemInfo;

    private GameObject nowPageObj; //当前页面的物体
    private int nowPage; //当前页面

    public override void Init()
    {
        HideAllPage();
        UpdatePage(1);
    }

    /// <summary>
    /// 更新页面
    /// </summary>
    public void UpdatePage(int page)
    {
        if (page < 1 || page > pageList.Count)
        {
            Debug.LogWarning("页面溢出");
            return;
        }
        nowPage = page;
        //更新画面
    }

    public void HideAllPage()
    {
        foreach (var page in pageList)
        {
            page.SetActive(false);
        }
    }
}
