using System.Collections;
using System.Collections.Generic;
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

    //初始化
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

        //检查格子是否被占用  
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
    /// 检测边界是否合法
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    /// <returns>边界情况是否合法</returns>
    public bool CheckBound(Item item, Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0) return false;
        if (gridPos.x + item.data.size.x > gridWidth) return false;
        if (gridPos.y + item.data.size.y > gridHeight) return false;
        return true;
    }

    /// <summary>
    /// 放置物品
    /// </summary>
    /// <param name="item">放入的物品</param>
    /// <param name="gridPos">物品放置的起始坐标</param>
    public void PlaceItem(Item item,Vector2Int gridPos)
    {
        //对应物品格更新状态
        for (int x = 0; x < item.data.size.x; x++)
        {
            for (int y = 0; y < item.data.size.y; y++)
            {
                slots[gridPos.x + x, gridPos.y + y].AddItem(item);
            }
        }
        //将物品附着到格子上
        ItemSlot beginSlot = slots[gridPos.x, gridPos.y]; //原点坐标
        ItemSlot endSlot = slots[gridPos.x + item.data.size.x-1, gridPos.y + item.data.size.y-1]; //右上角格子坐标
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
        //对应物品格更新状态
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
