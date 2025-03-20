using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��Ʒ��
/// </summary>
public class ItemSlot : MonoBehaviour
{
    [HideInInspector] public bool isUsed; //�Ƿ�ռ��
    private BaseGrid grid; //��Ʒ�������ĸ�����
    [HideInInspector] public Item nowItem; //��ǰ��Ʒ�����Ʒ

    private Image slotImg;

    void Awake()
    {
        nowItem = null;
        slotImg = GetComponent<Image>();
        SetStatus(false);
    }

    /// <summary>
    /// �������������Ʒ
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        nowItem = item;
        SetStatus(true);
    }

    /// <summary>
    /// �Ƴ���ǰ��Ʒ
    /// </summary>
    public void RemoveItem()
    {
        nowItem = null;
        SetStatus(false);
    }

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="bagGrid">����</param>
    public void SetBelongs(BaseGrid grid)
    {
        this.grid = grid;
    }

    /// <summary>
    /// ����ʹ��״̬
    /// </summary>
    /// <param name="isUsed">�Ƿ�ʹ��</param>
    public void SetStatus(bool isUsed)
    {
        this.isUsed = isUsed;
        if (isUsed)
            SetColor(Defines.GetSlotColor(nowItem.data.slotColorType));
        else
            SetColor(Defines.validColor);
    }

    /// <summary>
    /// ������Ʒ����ɫ
    /// </summary>
    /// <param name="color">��ɫ</param>
    public void SetColor(Color color)
    {
        slotImg.color = color;
    }

    /// <summary>
    /// ����ͼƬ
    /// </summary>
    public void HideImg()
    {
        slotImg.enabled = false;
    }
}