using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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

    private GameObject ItemSlotObj; //物品格预设体
    public ItemSlot[,] slots; //存储物品格

    [HideInInspector] public List<Item> items; //存储的物品

    private GridLayoutGroup gridLayoutGroup;

    private void Start()
    {
        ItemSlotObj = Resources.Load<GameObject>("Bag/ItemSlot");
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        items = new List<Item>();
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        gridLayoutGroup.constraintCount = gridWidth;
        gridLayoutGroup.cellSize = new Vector2(Defines.cellSize,Defines.cellSize);
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

        //测试：更新信息
        BagManager.Instance.UpdateMainBagInfo();

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
        //测试：更新信息
        //BagManager.Instance.UpdateMainBagInfo();
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

    #region 物品整理
    /// <summary>
    /// 自动整理背包（按物品面积排序）
    /// </summary>
    public void AutoArrange()
    {
        //临时存储所有物品并清空背包  
        List<Item> tempList = GetAllItems();
        ClearAllItems();

        // 按物品面积从大到小排序（提高空间利用率）  
        tempList = tempList.OrderByDescending(i => i.GetSize().x * i.GetSize().y).ToList();

        // 尝试放置每个物品  
        foreach (Item item in tempList)
        {
            if (!TryAutoPlaceItem(item))
            {
                Debug.LogError($"物品 {item.data.itemName} 无法放置，背包可能已满");
                //TODO:后续处理（删除、开新一页等）
            }
        }

        items = tempList; //赋值
    }

    /// <summary>
    /// 尝试放置物品
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryAutoPlaceItem(Item item)
    {
        // 重置旋转角度  
        item.currentRotation = 0;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        int begin = -ItemShape.MatrixLen / 2; //（物品起始点可能会在背包格外）
        //循环四个方向
        for (int step = 0; step < 4; step++)
        {
            // 遍历背包网格（从左上往右下排列，与网格坐标不一致，需转换）
            for (int y = gridHeight - 1; y >= begin; y--)
            {
                for (int x = begin; x < gridWidth; x++)
                {
                    item.gridPos = new Vector2Int(x, y);
                    if (CanPlaceItem(item, item.gridPos))
                    {
                        PlaceItem(item, item.gridPos);
                        item.oldGridPos = item.gridPos; //更新老位置坐标
                        return true;
                    }
                }
            }

            //旋转90度
            item.currentRotation += 90;
            item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        }
        return false;
    }

    /// <summary>
    /// 获得当前背包所有物品
    /// </summary>
    /// <returns></returns>
    private List<Item> GetAllItems()
    {
        List<Item> tempList = new List<Item>();
        for (int i = 0; i < items.Count; i++)
            tempList.Add(items[i]);
        return tempList;
    }

    /// <summary>
    /// 清除所有物品
    /// </summary>
    private void ClearAllItems()
    {
        for (int i = items.Count-1; i >= 0; i--)
            RemoveItem(items[i], items[i].gridPos);
    }
    #endregion

    #region 物品属性效果计算（计算最终玩家实力）

    /// <summary>
    /// 计算属性
    /// </summary>
    public void CalculateAttribute()
    {
        TowerManager.Instance.RecordOldData(); //数据变化前先记录老数据
        CalculateTower();
        CalculateItemAttribute();
        UIManager.Instance.GetPanel<BagPanel>().UpdateTowerInfo();
    }

    /// <summary>
    /// 计算有哪些可以使用的防御塔
    /// </summary>
    public void CalculateTower()
    {
        TowerManager.Instance.towers.Clear();
        foreach (Item item in items)
        {
            if (item.data.itemTags.Contains(ItemTag.Tower))
            {
                TowerData towerData = new TowerData(TowerManager.Instance.GetTowerSO_ByName(item.data.itemName));
                //添加防御塔
                TowerManager.Instance.AddTower(item.data.itemName, towerData);
            }
        }
    }

    /// <summary>
    /// 计算物品的属性（目前都是给防御塔加的，联动也是防御塔）
    /// </summary>
    public void CalculateItemAttribute()
    {
        //遍历背包中所有物品
        foreach (Item item in items)
        {
            List<Item> neighbors = item.GetConnectItems(); //获取该物品周围有效的激活物品（无法知道激活的是哪个属性）
            
            //遍历物品的所有属性
            foreach (ItemAttribute attribute in item.data.itemAttributes)
            {
                //全局属性
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                {
                    if (attribute.condition.conditionType == ItemActiveCondition.ConditionType.Tag)
                        TowerManager.Instance.SetTowerDataFromTag(attribute.condition.tags, attribute.activeEffects);
                    else
                        TowerManager.Instance.SetTowerDataFromName(attribute.condition.name, attribute.activeEffects);
                }
                //联动属性
                else
                {
                    //对激活物品进行遍历，检查是否满足该物品属性（一个物品可能会有许多个联动属性）
                    foreach (Item neighborItem in neighbors)
                    {
                        if (attribute.IsMatch(neighborItem))
                            TowerManager.Instance.SetTowerDataFromName(neighborItem.data.itemName, attribute.activeEffects);
                    }
                }
            }
        }
    }
    #endregion
}
