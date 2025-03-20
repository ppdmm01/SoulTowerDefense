using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����
/// </summary>
public class BaseGrid : MonoBehaviour
{
    public string gridName; //������
    public bool isOpenPreview = true; //�Ƿ���Ԥ��
    public bool isShowSlot = true; //�Ƿ���ʾ����
    public bool isLocked = false; //�Ƿ���ס�����ڵ�������Ʒ���޷����������ƶ���
    [SerializeField] protected int gridWidth = 10; //�����
    [SerializeField] protected int gridHeight = 10; //�����

    protected GameObject ItemSlotObj; //��Ʒ��Ԥ����
    public ItemSlot[,] slots; //�洢��Ʒ��

    [HideInInspector] public List<Item> items; //�洢����Ʒ

    protected GridLayoutGroup gridLayoutGroup;

    private bool isInit; //�Ƿ��ʼ��

    protected virtual void Awake()
    {
        isInit = false;
        ItemSlotObj = Resources.Load<GameObject>("Bag/ItemSlot");
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        items = new List<Item>();
        Init();
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    protected virtual void Init()
    {
        gridLayoutGroup.constraintCount = gridWidth;
        gridLayoutGroup.cellSize = new Vector2(Defines.cellSize, Defines.cellSize);
        gridLayoutGroup.spacing = new Vector2(Defines.spacing, Defines.spacing);
        slots = new ItemSlot[gridWidth, gridHeight];
        //����Ʒ����䱳��
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //ʵ������Ʒ�񲢳�ʼ��
                GameObject obj = Instantiate(ItemSlotObj);
                obj.transform.SetParent(transform, false); //��Ϊ����������
                ItemSlot slot = obj.GetComponent<ItemSlot>();
                slot.SetBelongs(this);
                slots[x, y] = slot;
                if (!isShowSlot)
                    slot.HideImg();
            }
        }
        isInit = true;
    }

    /// <summary>
    /// �������ݸ���������Ϣ
    /// </summary>
    public virtual void UpdateGrid(GridData gridData)
    {
        if (!isInit) Init();

        ForceUpdateGridLayout();
        //��ʼ��ԭ�����ڵ���Ʒ��λ��
        foreach (ItemData itemData in gridData.itemDatas)
        {
            GridManager.Instance.InitGridItems(itemData, this);
        }
    }

    //ǿ���������²���
    public void ForceUpdateGridLayout()
    {
        //���ֵ�һ�����⣺�����Grid�ճ�ʼ����������Ʒ��GridLayoutGroup�ܿ��ܻ�û���ú�slot��λ�ã����Ի������Ʒλ�ò���������
        //��������ǵ�һ֡��ִ�У�����ǿ�Ʋ��֣��Ҳ���ǿ�Ʋ��ֵķ�ʽ
        if (gridLayoutGroup != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());
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
                if (matrix[x, y] && slots[gridPos.x + x, gridPos.y + y].isUsed)
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
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (!matrix[x, y]) continue;
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
    public virtual void PlaceItem(Item item, Vector2Int gridPos, bool isUpdateCombination = true)
    {
        item.ClearUsedPos();
        bool[,] matrix = item.GetRotateMatrix();
        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x, y])
                {
                    item.AddUsedPos(new Vector2Int(gridPos.x + x, gridPos.y + y));
                    slots[gridPos.x + x, gridPos.y + y].AddItem(item);
                }
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
    public virtual void RemoveItem(Item item, Vector2Int gridPos)
    {
        item.ClearUsedPos();
        bool[,] matrix = item.GetRotateMatrix();
        //��Ӧ��Ʒ�����״̬
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x, y])
                {
                    slots[gridPos.x + x, gridPos.y + y].RemoveItem();
                }
            }
        }
        items.Remove(item);
    }
    #endregion

    #region ��Ʒ����
    /// <summary>
    /// �Զ�������������Ʒ�������
    /// </summary>
    public void AutoArrange()
    {
        //��ʱ�洢������Ʒ����ձ���  
        List<Item> tempList = GetAllItems();
        ClearAllItems();

        // ����Ʒ����Ӵ�С������߿ռ������ʣ�  
        tempList = tempList.OrderByDescending(i => i.GetSize().x * i.GetSize().y).ToList();

        // ���Է���ÿ����Ʒ  
        foreach (Item item in tempList)
        {
            if (!TryAutoPlaceItem(item))
            {
                //��ʾ
                UIManager.Instance.ShowTipInfo("�ռ䲻�㣬��Ʒ����ʧ��");
            }
        }

        items = tempList; //��ֵ
    }

    /// <summary>
    /// ���Է�����Ʒ
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool TryAutoPlaceItem(Item item)
    {
        // ������ת�Ƕ�  
        item.currentRotation = 0;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        int begin = -ItemShape.MatrixLen / 2; //����Ʒ��ʼ����ܻ��ڱ������⣩
                                                //ѭ���ĸ�����
        for (int step = 0; step < 4; step++)
        {
            // �����������񣨴��������������У����������겻һ�£���ת����
            for (int y = gridHeight - 1; y >= begin; y--)
            {
                for (int x = begin; x < gridWidth; x++)
                {
                    item.gridPos = new Vector2Int(x, y);
                    if (CanPlaceItem(item, item.gridPos))
                    {
                        PlaceItem(item, item.gridPos);
                        item.oldGridPos = item.gridPos; //������λ������
                        return true;
                    }
                }
            }

            //��ת90��
            item.currentRotation += 90;
            item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        }
        return false;
    }
    #endregion

    #region ��ȡ�������
    /// <summary>
    /// ��õ�ǰ����������Ʒ
    /// </summary>
    /// <returns></returns>
    protected List<Item> GetAllItems()
    {
        List<Item> tempList = new List<Item>();
        for (int i = 0; i < items.Count; i++)
            tempList.Add(items[i]);
        return tempList;
    }

    /// <summary>
    /// ���������Ʒ��������ݣ�
    /// </summary>
    protected void ClearAllItems()
    {
        for (int i = items.Count - 1; i >= 0; i--)
            RemoveItem(items[i], items[i].gridPos);
    }

    /// <summary>
    /// ����������Ʒ
    /// </summary>
    public void DestroyAllItems(bool isUpdateInfo)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            items[i].SetIsUpdateInfo(isUpdateInfo);
            items[i].DeleteMe();
        }
    }

    public ItemSlot GetSlot(Vector2Int pos)
    {
        if (CheckPoint(pos)){
            return slots[pos.x,pos.y];
        }
        return null;
    }
    #endregion

    #region ��ƷԤ��
    /// <summary>
    /// ��Ʒ����Ԥ��
    /// </summary>
    /// <param name="item">�������Ʒ</param>
    /// <param name="gridPos">��Ʒ���õ���ʼ����</param>
    /// <param name="isPreview">�Ƿ�Ԥ����trueԤ����falseȡ��Ԥ��</param>
    public void ItemPreview(Item item, Vector2Int gridPos, bool isPreview)
    {
        if (!isOpenPreview) return;

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
