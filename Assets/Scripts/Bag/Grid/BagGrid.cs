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

    private GameObject ItemSlotObj; //��Ʒ��Ԥ����
    private ItemSlot[,] slots; //�洢��Ʒ��

    public List<Item> items; //�洢����Ʒ

    private GridLayoutGroup gridLayoutGroup;

    private void Start()
    {
        ItemSlotObj = Resources.Load<GameObject>("Bag/ItemSlot");
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        Init();
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    private void Init()
    {
        gridLayoutGroup.constraintCount = gridWidth;
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

    #region ��Ʒ��������
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
        bool[,] matrix = item.GetRotateMatrix();

        //�������Ƿ�ռ��  
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x,y] && slots[gridPos.x + x, gridPos.y + y].isUsed)
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
        bool[,] matrix = item.GetRotateMatrix();

        Vector2Int nowPos;
        //������ǰ������һ�ж��Ƿ�Խ��
        for (int x = 0;x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0;y < ItemShape.MatrixLen; y++)
            {
                if (!matrix[x,y]) continue;
                //ֻ�����ռλ�ĸ���
                nowPos = new Vector2Int(gridPos.x + x, gridPos.y + y);
                if (!CheckPoint(nowPos)) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ���õ��Ƿ��ڱ߽���
    /// </summary>
    /// <param name="point">����</param>
    /// <returns></returns>
    public bool CheckPoint(Vector2Int point)
    {
        if (point.x < 0 || point.y < 0) return false;
        if (point.x >= gridWidth || point.y >= gridHeight) return false;
        return true;
    }

    /// <summary>
    /// ������Ʒ
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    public void PlaceItem(Item item,Vector2Int gridPos)
    {
        bool[,] matrix = item.GetRotateMatrix();

        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x, y])
                    slots[gridPos.x + x, gridPos.y + y].AddItem(item);
            }
        }
        items.Add(item);

        //����Ʒ���ŵ�����������
        Vector2Int minOffset = item.GetOriginOffset();
        Vector2Int maxOffset = item.GetEndOffset();
        ItemSlot beginSlot = slots[gridPos.x + minOffset.x, gridPos.y + minOffset.y]; //ԭ������
        ItemSlot endSlot = slots[gridPos.x + maxOffset.x, gridPos.y + maxOffset.y]; //���ϽǸ�������
        Vector2 centerPos = (beginSlot.transform.position + endSlot.transform.position) / 2; //������������
        item.transform.position = centerPos;
    }

    /// <summary>
    /// �ó���Ʒ
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    public void RemoveItem(Item item, Vector2Int gridPos)
    {
        bool[,] matrix = item.GetRotateMatrix();
        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x, y])
                    slots[gridPos.x + x, gridPos.y + y].RemoveItem();
            }
        }
        items.Remove(item);
    }
    #endregion

    #region ��ƷԤ��
    /// <summary>
    /// ��Ʒ����Ԥ��
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    /// <param name="isPreview">�Ƿ�Ԥ����trueԤ����falseȡ��Ԥ��</param>
    public void ItemPreview(Item item, Vector2Int gridPos,bool isPreview)
    {
        if (isPreview)
            Debug.Log("����Ԥ��");
        else
            Debug.Log("�ر�Ԥ��");
        bool[,] matrix = item.GetRotateMatrix();
        Vector2Int nowPos; //��¼��ǰ����
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (!matrix[x, y]) continue; //��Ʒδռ��
                nowPos = new Vector2Int(gridPos.x + x, gridPos.y + y);
                if (!CheckPoint(nowPos)) continue; //������Χ

                ItemSlot slot = slots[nowPos.x, nowPos.y];
                if (isPreview)
                    slot.SetColor(slot.isUsed ? Defines.previewInvalidColor : Defines.previewValidColor); //����Ԥ����ɫ
                else
                    slot.SetStatus(slot.isUsed); //ȡ��Ԥ��,�ָ�ԭ��״̬
            }
        }
    }
    #endregion
}
