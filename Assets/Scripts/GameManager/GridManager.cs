using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : SingletonMono<GridManager>
{
    private GridManager() { }

    protected override void Awake()
    {
        base.Awake();
        itemsTrans = UIManager.Instance.itemTrans;
        gridDic = new Dictionary<string, BaseGrid>();
        itemPrefab = Resources.Load<GameObject>("Bag/ItemPrefab");

        //背包内所有物品触发成长
        EventCenter.Instance.AddEventListener(EventType.EnterMapNode, AllItemGrow);
    }

    [HideInInspector] public Transform itemsTrans; //放置物品的父对象
    public Dictionary<string, BaseGrid> gridDic; //通过字典存储所有网格（网格可能有多个，比如一个背包，一个储物箱）
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
    public BaseGrid GetGridByName(string gridName)
    {
        if (gridDic.ContainsKey(gridName))
            return gridDic[gridName];
        return null;
    }

    //朝指定背包添加指定名称物品
    public void AddItem(string itemName, BaseGrid grid,int num = 1, List<BuffType> buffs = null)
    {
        for (int i=0;i<num; i++)
        {
            //创建物体
            GameObject itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //添加并初始化Item脚本
            Item item = itemObj.AddComponent<Item>();
            ItemSO data = ItemManager.Instance.GetItemDataByName(itemName);
            item.Init(data, grid);
            //添加标记（附魔）
            if (buffs != null)
                foreach (BuffType buff in buffs)
                  if (buff != BuffType.None && !item.nowItemBuffs.Contains(buff))
                     item.nowItemBuffs.Add(buff);
            //背包满了
            if (!grid.TryAutoPlaceItem(item))
            {
                //删除多余的物品
                item.DeleteMe();
                //提示
                UIManager.Instance.ShowTipInfo("空间不足，物品放置失败");
            }
        }
    }

    //朝指定背包添加指定Id的物品
    public void AddItem(int id, BaseGrid grid,int num = 1,List<BuffType> buffs = null)
    {
        for (int i = 0; i < num; i++)
        {
            //创建物体
            GameObject itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //添加并初始化Item脚本
            Item item = itemObj.AddComponent<Item>();
            ItemSO data = ItemManager.Instance.GetItemDataById(id);
            item.Init(data, grid);
            //添加标记（附魔）
            if (buffs != null)
                foreach (BuffType buff in buffs)
                    if (buff != BuffType.None && !item.nowItemBuffs.Contains(buff))
                        item.nowItemBuffs.Add(buff);
            //背包满了
            if (!grid.TryAutoPlaceItem(item))
            {
                //删除多余的物品
                item.DeleteMe();
                //提示
                UIManager.Instance.ShowTipInfo("空间不足，物品放置失败");
            }
        }
    }

    //初始化网格内物品
    public void InitGridItems(ItemData itemData, BaseGrid grid)
    {
        //创建物体
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //添加并初始化Item脚本
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataById(itemData.id);
        item.Init(data, grid);
        item.growSpeed = itemData.growSpeed;
        item.nowAttributes = itemData.itemAttributes;
        item.gridPos = itemData.gridPos;
        item.nowItemBuffs = itemData.itemBuffs;
        item.currentRotation = itemData.currentRotation;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        //放置
        if (grid.CanPlaceItem(item, item.gridPos))
        {
            grid.PlaceItem(item, item.gridPos,false);  //初始化放置不需要显示组合信息 
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
    /// 设置物品的成长速度
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="growSpeed"></param>
    public void SetItemGrowSpeed(string itemName,int growSpeed)
    {
        //设置符合名字的物品成长速度
        BaseGrid storageBox = GetGridByName("StorageBox");
        BaseGrid bag = GetGridByName("Bag");
        if (storageBox != null)
            foreach (Item item in storageBox.items.Where(item => item.data.itemName == itemName).ToList())
            {
                item.growSpeed = growSpeed;
            }
        if (bag != null)
            foreach (Item item in bag.items.Where(item => item.data.itemName == itemName).ToList())
                item.growSpeed = growSpeed;
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
        BagGrid bagGrid = GetGridByName("Bag") as BagGrid;
        if (bagGrid != null)
        {
            bagGrid.CalculateAttribute();
        }
    }

    //让物品成长
    public void AllItemGrow()
    {
        GridData bagData = GameDataManager.Instance.GetGridData("Bag");
        if (bagData != null)
        {
            bagData.AllItemGrow();
            GameDataManager.Instance.UpdateGridData(bagData);
            GameDataManager.Instance.SaveGridData();
        }
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(EventType.EnterMapNode, AllItemGrow);
    }
}