using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrystalPanel : BasePanel
{
    public BagGrid bag;
    public Button closeBtn;
    [Header("��Ʒ��Ϣ")]
    public GameObject ItemInfoObj;
    public override void Init()
    {
        HideItemInfo();
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<CrystalPanel>();
            Time.timeScale = 1; //ʱ��ָ�
        });
    }

    protected override void Update()
    {
        if (canvasGroup.alpha < 1 && isShow)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                Time.timeScale = 0; //ʱ����ͣ
                canvasGroup.alpha = 1;
            }
        }

        if (canvasGroup.alpha > 0 && !isShow)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //ִ���¼�
                hideCallBack?.Invoke();
            }
        }
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //�򱳰�����������ӱ���
        GridManager.Instance.AddGrid(bag);
        //��ȡ�������ݲ�����
        GridData bagData = GameDataManager.Instance.GetGridData(bag.gridName);
        if (bagData != null)
            bag.UpdateGrid(bagData);
        //������Ʒ��ֹ�ƶ�
        bag.isLocked = true;
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //������Ʒ�ָ��ƶ�
        bag.isLocked = false;
        //������������
        GridData bagData = new GridData(bag.gridName, bag.items);
        GameDataManager.Instance.UpdateGridData(bagData);
        GameDataManager.Instance.SaveGridData();
        //�����Ʒ
        GridManager.Instance.ClearAllItem(bag, false);
        //�򱳰����������Ƴ�����
        GridManager.Instance.RemoveGrid(bag);
    }

    /// <summary>
    /// ��ʾ��Ʒ��Ϣ
    /// </summary>
    public void ShowItemInfo(Item item)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(item);
    }

    /// <summary>
    /// ������Ʒ��Ϣ
    /// </summary>
    public void HideItemInfo()
    {
        ItemInfoObj.SetActive(false);
        ItemInfoObj.GetComponent<ItemInfo>().RemoveAllAttributeInfo();
    }


}
