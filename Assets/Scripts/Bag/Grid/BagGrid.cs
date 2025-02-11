using System.Collections;
using System.Collections.Generic;
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

    //��ʼ��
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

        //�������Ƿ�ռ��  
        for (int x = 0; x < item.data.size.x; x++)
        {
            for (int y = 0; y < item.data.size.y; y++)
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
    /// ���߽��Ƿ�Ϸ�
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    /// <returns>�߽�����Ƿ�Ϸ�</returns>
    public bool CheckBound(Item item, Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0) return false;
        if (gridPos.x + item.data.size.x > gridWidth) return false;
        if (gridPos.y + item.data.size.y > gridHeight) return false;
        return true;
    }

    /// <summary>
    /// ������Ʒ
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    public void PlaceItem(Item item,Vector2Int gridPos)
    {
        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < item.data.size.x; x++)
        {
            for (int y = 0; y < item.data.size.y; y++)
            {
                slots[gridPos.x + x, gridPos.y + y].AddItem(item);
            }
        }
        //����Ʒ���ŵ�������
        ItemSlot beginSlot = slots[gridPos.x, gridPos.y]; //ԭ������
        ItemSlot endSlot = slots[gridPos.x + item.data.size.x-1, gridPos.y + item.data.size.y-1]; //���ϽǸ�������
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
        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < item.data.size.x; x++)
        {
            for (int y = 0; y < item.data.size.y; y++)
            {
                slots[gridPos.x + x, gridPos.y + y].RemoveItem();
            }
        }
    }

    //public Vector2 GridToWorldPosition(Vector2Int gridPos)
    //{
    //    return _grid.GetWorldPosition(gridPos.x, gridPos.y);
    //}

    //public Vector2Int WorldToGridPosition(Vector2 worldPos)
    //{
    //    _grid.GetXY(worldPos, out int x, out int y);
    //    return new Vector2Int(x, y);
    //}

    //private class GridCell
    //{
    //    public ItemSO CurrentItem { get; private set; }
    //    public bool HasItem => CurrentItem != null;

    //    public void SetItem(ItemSO item)
    //    {
    //        CurrentItem = item;
    //    }
    //}
}
