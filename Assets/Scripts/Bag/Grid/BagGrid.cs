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

    private GameObject ItemSlotObj; //物品格
    private ItemSlot[,] slots; //存储物品格

    private void Start()
    {
        ItemSlotObj = Resources.Load<GameObject>("Bag/ItemSlot");
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
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
        //检查格子是否被占用  
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
    /// 检测物品边界是否合法
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    /// <returns>边界情况是否合法</returns>
    public bool CheckBound(Item item, Vector2Int gridPos)
    {
        Vector2Int size = item.GetSize();
        if (gridPos.x < 0 || gridPos.y < 0) return false;
        if (gridPos.x + size.x > gridWidth) return false;
        if (gridPos.y + size.y > gridHeight) return false;
        return true;
    }

    /// <summary>
    /// 检测该点是否在边界内
    /// </summary>
    /// <param name="gridPos">检测点</param>
    /// <returns></returns>
    public bool CheckPoint(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0) return false;
        if (gridPos.x >= gridWidth || gridPos.y >= gridHeight) return false;
        return true;
    }

    /// <summary>
    /// 放置物品
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    public void PlaceItem(Item item,Vector2Int gridPos)
    {
        Vector2Int size = item.GetSize();
        //对应物品格更新状态
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                slots[gridPos.x + x, gridPos.y + y].AddItem(item);
            }
        }
        //将物品附着到格子上
        ItemSlot beginSlot = slots[gridPos.x, gridPos.y]; //原点坐标
        ItemSlot endSlot = slots[gridPos.x + size.x-1, gridPos.y + size.y-1]; //右上角格子坐标
        Vector2 centerPos = (beginSlot.transform.position+endSlot.transform.position) / 2; //计算中心坐标
        item.transform.position = centerPos;
    }

    /// <summary>
    /// 拿出物品
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    public void RemoveItem(Item item, Vector2Int gridPos)
    {
        Vector2Int size = item.GetSize();
        //对应物品格更新状态
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                slots[gridPos.x + x, gridPos.y + y].RemoveItem();
            }
        }
    }

    /// <summary>
    /// 物品放置预览
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    /// <param name="isPreview">是否预览，true预览，false取消预览</param>
    public void ItemPreview(Item item, Vector2Int gridPos,bool isPreview)
    {
        Vector2Int size = item.GetSize();
        Vector2Int nowPos = Vector2Int.zero; //记录当前格子
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                nowPos.x = gridPos.x + x;
                nowPos.y = gridPos.y + y;
                //如果当前格子超出范围，则跳过
                if (!CheckPoint(nowPos)) continue;
                ItemSlot slot = slots[nowPos.x, nowPos.y];
                //取消预览
                if (!isPreview)
                {
                    slot.SetStatus(slot.isUsed); //恢复原本状态
                    continue;
                }

                //设置预览颜色
                if (slot.isUsed)
                    slot.SetColor(Defines.previewInvalidColor);
                else
                    slot.SetColor(Defines.previewValidColor);
            }
        }
    }
}
