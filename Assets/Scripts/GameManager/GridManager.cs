using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GridManager : SingletonMono<GridManager>
{
    private GridManager() { }

    protected override void Awake()
    {
        base.Awake();
        itemsTrans = UIManager.Instance.itemTrans;
        gridDic = new Dictionary<string, BaseGrid>();
        itemPrefab = Resources.Load<GameObject>("Bag/ItemPrefab");
    }

    [HideInInspector] public Transform itemsTrans; //放置物品的父对象
    public Dictionary<string, BaseGrid> gridDic; //通过字典（背包可能有多个，比如一个背包，一个储物箱）
    private GameObject itemPrefab;

    //检测鼠标点是否在指定网格里
    public bool IsInsideGrid(Vector2 screenPoint, string bagName)
    {
        if (!gridDic.ContainsKey(bagName)) return false;
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
            gridDic[bagName].transform as RectTransform,
            screenPoint,
            null
        );

        return isInside;
    }

    /// <summary>
    /// 向管理器添加网格
    /// </summary>
    public void AddGrid(BaseGrid grid)
    {
        if (gridDic.ContainsKey(grid.gridName)) return;
        gridDic.Add(grid.gridName, grid);
    }

    /// <summary>
    /// 向管理器移除网格
    /// </summary>
    /// <param name="grid"></param>
    public void RemoveGrid(BaseGrid grid)
    {
        if (gridDic.ContainsKey(grid.gridName))
            gridDic.Remove(grid.gridName);
    }

    //获取背包
    public BaseGrid GetBagByName(string bagName)
    {
        return gridDic[bagName];
    }

    //朝指定背包添加指定名称物品
    public void AddItem(string itemName, BaseGrid grid)
    {
        //GameObject itemObj = Instantiate(Resources.Load<GameObject>("Items/"+name));
        //创建物体
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //添加并初始化Item脚本
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataByName(itemName);
        item.Init(data, grid);
        //背包满了
        if (!grid.TryAutoPlaceItem(item))
        {
            //删除多余的物品
            item.DeleteMe();
            //提示
            UIManager.Instance.ShowTipInfo("空间不足，物品放置失败");
        }
    }

    //朝指定背包添加指定Id的物品
    public void AddItem(int id, BaseGrid grid)
    {
        //创建物体
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //添加并初始化Item脚本
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataById(id);
        item.Init(data, grid);
        //背包满了
        if (!grid.TryAutoPlaceItem(item))
        {
            //删除多余的物品
            item.DeleteMe();
            //提示
            UIManager.Instance.ShowTipInfo("空间不足，物品放置失败");
        }
    }

    public void AddItem(ItemData itemData, BaseGrid grid)
    {
        //创建物体
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //添加并初始化Item脚本
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataById(itemData.id);
        item.Init(data, grid);
        item.gridPos = itemData.gridPos;
        item.currentRotation = itemData.currentRotation;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        //放置
        if (grid.CanPlaceItem(item, item.gridPos))
        {
            grid.PlaceItem(item, item.gridPos);
            item.oldGridPos = item.gridPos; //更新老位置坐标
        }
        else
        {
            //删除多余的物品
            item.DeleteMe();
            //提示
            UIManager.Instance.ShowTipInfo("空间不足，物品放置失败");
        }
    }

    //添加指定数量的随机道具
    public void AddRandomItem(int num, BaseGrid grid)
    {
        List<ItemSO> list = ItemManager.Instance.GetRandomItemData(num);
        GameObject itemObj;
        Item item;
        foreach (ItemSO itemData in list)
        {
            //创建物体
            itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //添加并初始化Item脚本
            item = itemObj.AddComponent<Item>();
            item.Init(itemData, grid);
            //背包满了
            if (!grid.TryAutoPlaceItem(item))
            {
                //删除多余的物品
                item.DeleteMe();
                //提示
                UIManager.Instance.ShowTipInfo("空间不足，物品放置失败");
                break;
            }
        }
    }

    /// <summary>
    /// 清理指定背包的所有物品
    /// </summary>
    /// <param name="grid"></param>
    public void ClearAllItem(BaseGrid grid,bool isUpdateInfo = false)
    {
        grid.DestroyAllItems(isUpdateInfo);
    }

    /// <summary>
    /// 更新战前准备阶段的防御塔信息
    /// </summary>
    public void UpdateFightTowerInfo()
    {
        //这里先默认存在，后面可能需要判空
        BagGrid bagGrid = GetBagByName("Bag") as BagGrid;
        if (bagGrid != null)
        {
            bagGrid.CalculateAttribute();
        }
    }
}