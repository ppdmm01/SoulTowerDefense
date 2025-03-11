using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class BookPanel : BasePanel
{
    [Header("��ǩ��ť�б�")]
    public List<LabelBtn> labelBtnList;
    [Header("ҳ���б�")]
    public List<GameObject> pageList;
    [Header("��Ʒ����")]
    public ItemInfo itemInfo;

    private GameObject nowPageObj; //��ǰҳ�������
    private int nowPage; //��ǰҳ��

    public override void Init()
    {
        HideAllPage();
        UpdatePage(1);
    }

    /// <summary>
    /// ����ҳ��
    /// </summary>
    public void UpdatePage(int page)
    {
        if (page < 1 || page > pageList.Count)
        {
            Debug.LogWarning("ҳ�����");
            return;
        }
        nowPage = page;
        //���»���
    }

    public void HideAllPage()
    {
        foreach (var page in pageList)
        {
            page.SetActive(false);
        }
    }
}
