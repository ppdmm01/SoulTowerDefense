using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包网格
/// </summary>
public class BagGrid : MonoBehaviour
{
    public string bagName; //背包名
    [SerializeField] private int gridWidth = 10; //网格宽
    [SerializeField] private int gridHeight = 10; //网格高
    private float cellSize = Defines.cellSize; //每个格子的大小

    private GameObject ItemSlotObj; //物品格预设体
    private ItemSlot[,] slots; //存储物品格

    public List<Item> items; //存储的物品

    private GridLayoutGroup gridLayoutGroup;

    private void Start()
    {
        ItemSlotObj = Resources.Load<GameObject>("Bag/ItemSlot");
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        gridLayoutGroup.constraintCount = gridWidth;
        slots = new ItemSlot[gridWidth, gridHeight];
        //将物品格填充背包
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //实例化物品格并初始化
                GameObject obj = Instantiate(ItemSlotObj);
                obj.transform.SetParent(transform,false); //作为背包子物体
                ItemSlot slot = obj.GetComponent<ItemSlot>();
                slot.SetBelongs(this);
                slots[x, y] = slot;
            }
        }
    }

    #region 物品放置与检测
    /// <summary>
    /// 检测是否可以放入物品
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    /// <returns>检测值</returns>
    public bool CanPlaceItem(Item item, Vector2Int gridPos)
    {
        //检查是否超出边界  
        if (!CheckBound(item, gridPos)) return false;

        Vector2Int size = item.GetSize();
        bool[,] matrix = item.GetRotateMatrix();

        //检查格子是否被占用  
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
    /// 检测物品边界是否合法
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    /// <returns>边界情况是否合法</returns>
    public bool CheckBound(Item item, Vector2Int gridPos)
    {
        bool[,] matrix = item.GetRotateMatrix();

        Vector2Int nowPos;
        //遍历当前矩阵，逐一判断是否越界
        for (int x = 0;x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0;y < ItemShape.MatrixLen; y++)
            {
                if (!matrix[x,y]) continue;
                //只检测有占位的格子
                nowPos = new Vector2Int(gridPos.x + x, gridPos.y + y);
                if (!CheckPoint(nowPos)) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检测该点是否在边界内
    /// </summary>
    /// <param name="point">检测点</param>
    /// <returns></returns>
    public bool CheckPoint(Vector2Int point)
    {
        if (point.x < 0 || point.y < 0) return false;
        if (point.x >= gridWidth || point.y >= gridHeight) return false;
        return true;
    }

    /// <summary>
    /// 放置物品
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    public void PlaceItem(Item item,Vector2Int gridPos)
    {
        bool[,] matrix = item.GetRotateMatrix();

        //对应物品格更新状态
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x, y])
                    slots[gridPos.x + x, gridPos.y + y].AddItem(item);
            }
        }
        items.Add(item);

        //将物品附着到格子中心上
        Vector2Int minOffset = item.GetOriginOffset();
        Vector2Int maxOffset = item.GetEndOffset();
        ItemSlot beginSlot = slots[gridPos.x + minOffset.x, gridPos.y + minOffset.y]; //原点坐标
        ItemSlot endSlot = slots[gridPos.x + maxOffset.x, gridPos.y + maxOffset.y]; //右上角格子坐标
        Vector2 centerPos = (beginSlot.transform.position + endSlot.transform.position) / 2; //计算中心坐标
        item.transform.position = centerPos;
    }

    /// <summary>
    /// 拿出物品
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    public void RemoveItem(Item item, Vector2Int gridPos)
    {
        bool[,] matrix = item.GetRotateMatrix();
        //对应物品格更新状态
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

    #region 物品预览
    /// <summary>
    /// 物品放置预览
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    /// <param name="isPreview">是否预览，true预览，false取消预览</param>
    public void ItemPreview(Item item, Vector2Int gridPos,bool isPreview)
    {
        if (isPreview)
            Debug.Log("开启预览");
        else
            Debug.Log("关闭预览");
        bool[,] matrix = item.GetRotateMatrix();
        Vector2Int nowPos; //记录当前格子
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (!matrix[x, y]) continue; //物品未占用
                nowPos = new Vector2Int(gridPos.x + x, gridPos.y + y);
                if (!CheckPoint(nowPos)) continue; //超出范围

                ItemSlot slot = slots[nowPos.x, nowPos.y];
                if (isPreview)
                    slot.SetColor(slot.isUsed ? Defines.previewInvalidColor : Defines.previewValidColor); //设置预览颜色
                else
                    slot.SetStatus(slot.isUsed); //取消预览,恢复原本状态
            }
        }
    }
    #endregion
}
