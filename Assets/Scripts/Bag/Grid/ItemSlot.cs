using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// ��Ʒ��
/// </summary>
public class ItemSlot : MonoBehaviour
{
    public bool isUsed; //�Ƿ�ռ��
    private BagGrid bagGrid; //��Ʒ�������ĸ�����
    private Item nowItem; //��ǰ��Ʒ�����Ʒ
    private Image slotImg;

    void Start()
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
    public void SetBelongs(BagGrid bagGrid)
    {
        this.bagGrid = bagGrid;
    }

    /// <summary>
    /// ����ʹ��״̬
    /// </summary>
    /// <param name="isUsed">�Ƿ�ʹ��</param>
    private void SetStatus(bool isUsed)
    {
        this.isUsed = isUsed;
        slotImg.color = isUsed ? Defines.invalidColor : Defines.validColor;
    }
}
