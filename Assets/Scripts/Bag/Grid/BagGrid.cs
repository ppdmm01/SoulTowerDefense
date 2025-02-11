using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��������
/// </summary>
public class BagGrid : MonoBehaviour
{
    public string bagName; //������
    [SerializeField] private int gridWidth = 10; //�����
    [SerializeField] private int gridHeight = 10; //�����
    private float cellSize = Defines.cellSize; //ÿ�����ӵĴ�С

    private GameObject ItemSlotObj; //��Ʒ��
    private ItemSlot[,] slots; //�洢��Ʒ��

    private void Start()
    {
        ItemSlotObj = Resources.Load<GameObject>("Bag/ItemSlot");
        Init();
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    private void Init()
    {
        slots = new ItemSlot[gridWidth, gridHeight];
        //����Ʒ����䱳��
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //ʵ������Ʒ�񲢳�ʼ��
                GameObject obj = Instantiate(ItemSlotObj);
                obj.transform.SetParent(transform,false); //��Ϊ����������
                ItemSlot slot = obj.GetComponent<ItemSlot>();
                slot.SetBelongs(this);
                slots[x, y] = slot;
            }
        }
    }

    /// <summary>
    /// ����Ƿ���Է�����Ʒ
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    /// <returns>���ֵ</returns>
    public bool CanPlaceItem(Item item, Vector2Int gridPos)
    {
        //����Ƿ񳬳��߽�  
        if (!CheckBound(item, gridPos)) return false;

        Vector2Int size = item.GetSize();
        //�������Ƿ�ռ��  
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (slots[gridPos.x + x, gridPos.y + y].isUsed)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// �����Ʒ�߽��Ƿ�Ϸ�
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    /// <returns>�߽�����Ƿ�Ϸ�</returns>
    public bool CheckBound(Item item, Vector2Int gridPos)
    {
        Vector2Int size = item.GetSize();
        if (gridPos.x < 0 || gridPos.y < 0) return false;
        if (gridPos.x + size.x > gridWidth) return false;
        if (gridPos.y + size.y > gridHeight) return false;
        return true;
    }

    /// <summary>
    /// ���õ��Ƿ��ڱ߽���
    /// </summary>
    /// <param name="gridPos">����</param>
    /// <returns></returns>
    public bool CheckPoint(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0) return false;
        if (gridPos.x >= gridWidth || gridPos.y >= gridHeight) return false;
        return true;
    }

    /// <summary>
    /// ������Ʒ
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    public void PlaceItem(Item item,Vector2Int gridPos)
    {
        Vector2Int size = item.GetSize();
        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                slots[gridPos.x + x, gridPos.y + y].AddItem(item);
            }
        }
        //����Ʒ���ŵ�������
        ItemSlot beginSlot = slots[gridPos.x, gridPos.y]; //ԭ������
        ItemSlot endSlot = slots[gridPos.x + size.x-1, gridPos.y + size.y-1]; //���ϽǸ�������
        Vector2 centerPos = (beginSlot.transform.position+endSlot.transform.position) / 2; //������������
        item.transform.position = centerPos;
    }

    /// <summary>
    /// �ó���Ʒ
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    public void RemoveItem(Item item, Vector2Int gridPos)
    {
        Vector2Int size = item.GetSize();
        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                slots[gridPos.x + x, gridPos.y + y].RemoveItem();
            }
        }
    }

    /// <summary>
    /// ��Ʒ����Ԥ��
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    /// <param name="isPreview">�Ƿ�Ԥ����trueԤ����falseȡ��Ԥ��</param>
    public void ItemPreview(Item item, Vector2Int gridPos,bool isPreview)
    {
        Vector2Int size = item.GetSize();
        Vector2Int nowPos = Vector2Int.zero; //��¼��ǰ����
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                nowPos.x = gridPos.x + x;
                nowPos.y = gridPos.y + y;
                //�����ǰ���ӳ�����Χ��������
                if (!CheckPoint(nowPos)) continue;
                ItemSlot slot = slots[nowPos.x, nowPos.y];
                //ȡ��Ԥ��
                if (!isPreview)
                {
                    slot.SetStatus(slot.isUsed); //�ָ�ԭ��״̬
                    continue;
                }

                //����Ԥ����ɫ
                if (slot.isUsed)
                    slot.SetColor(Defines.previewInvalidColor);
                else
                    slot.SetColor(Defines.previewValidColor);
            }
        }
    }
}
