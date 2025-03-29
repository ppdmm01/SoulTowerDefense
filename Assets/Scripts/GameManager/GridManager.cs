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

        //������������Ʒ�����ɳ�
        EventCenter.Instance.AddEventListener(EventType.EnterMapNode, AllItemGrow);
    }

    [HideInInspector] public Transform itemsTrans; //������Ʒ�ĸ�����
    public Dictionary<string, BaseGrid> gridDic; //ͨ���ֵ�洢����������������ж��������һ��������һ�������䣩
    private GameObject itemPrefab;

    //��������Ƿ���ָ��������
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
    /// ��������������
    /// </summary>
    public void AddGrid(BaseGrid grid)
    {
        if (gridDic.ContainsKey(grid.gridName)) return;
        gridDic.Add(grid.gridName, grid);
    }

    /// <summary>
    /// ��������Ƴ�����
    /// </summary>
    /// <param name="grid"></param>
    public void RemoveGrid(BaseGrid grid)
    {
        if (gridDic.ContainsKey(grid.gridName))
            gridDic.Remove(grid.gridName);
    }

    //��ȡ����
    public BaseGrid GetGridByName(string gridName)
    {
        if (gridDic.ContainsKey(gridName))
            return gridDic[gridName];
        return null;
    }

    //��ָ���������ָ��������Ʒ
    public void AddItem(string itemName, BaseGrid grid,int num = 1, List<BuffType> buffs = null)
    {
        for (int i=0;i<num; i++)
        {
            //��������
            GameObject itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //��Ӳ���ʼ��Item�ű�
            Item item = itemObj.AddComponent<Item>();
            ItemSO data = ItemManager.Instance.GetItemDataByName(itemName);
            item.Init(data, grid);
            //��ӱ�ǣ���ħ��
            if (buffs != null)
                foreach (BuffType buff in buffs)
                  if (buff != BuffType.None && !item.nowItemBuffs.Contains(buff))
                     item.nowItemBuffs.Add(buff);
            //��������
            if (!grid.TryAutoPlaceItem(item))
            {
                //ɾ���������Ʒ
                item.DeleteMe();
                //��ʾ
                UIManager.Instance.ShowTipInfo("�ռ䲻�㣬��Ʒ����ʧ��");
            }
        }
    }

    //��ָ���������ָ��Id����Ʒ
    public void AddItem(int id, BaseGrid grid,int num = 1,List<BuffType> buffs = null)
    {
        for (int i = 0; i < num; i++)
        {
            //��������
            GameObject itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //��Ӳ���ʼ��Item�ű�
            Item item = itemObj.AddComponent<Item>();
            ItemSO data = ItemManager.Instance.GetItemDataById(id);
            item.Init(data, grid);
            //��ӱ�ǣ���ħ��
            if (buffs != null)
                foreach (BuffType buff in buffs)
                    if (buff != BuffType.None && !item.nowItemBuffs.Contains(buff))
                        item.nowItemBuffs.Add(buff);
            //��������
            if (!grid.TryAutoPlaceItem(item))
            {
                //ɾ���������Ʒ
                item.DeleteMe();
                //��ʾ
                UIManager.Instance.ShowTipInfo("�ռ䲻�㣬��Ʒ����ʧ��");
            }
        }
    }

    //��ʼ����������Ʒ
    public void InitGridItems(ItemData itemData, BaseGrid grid)
    {
        //��������
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //��Ӳ���ʼ��Item�ű�
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataById(itemData.id);
        item.Init(data, grid);
        item.growSpeed = itemData.growSpeed;
        item.nowAttributes = itemData.itemAttributes;
        item.gridPos = itemData.gridPos;
        item.nowItemBuffs = itemData.itemBuffs;
        item.currentRotation = itemData.currentRotation;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        //����
        if (grid.CanPlaceItem(item, item.gridPos))
        {
            grid.PlaceItem(item, item.gridPos,false);  //��ʼ�����ò���Ҫ��ʾ�����Ϣ 
            item.oldGridPos = item.gridPos; //������λ������
        }
        else
        {
            //ɾ���������Ʒ
            item.DeleteMe();
            //��ʾ
            UIManager.Instance.ShowTipInfo("�ռ䲻�㣬��Ʒ����ʧ��");
        }
    }

    //���ָ���������������
    public void AddRandomItem(int num, BaseGrid grid)
    {
        List<ItemSO> list = ItemManager.Instance.GetRandomItemData(num);
        GameObject itemObj;
        Item item;
        foreach (ItemSO itemData in list)
        {
            //��������
            itemObj = Instantiate(itemPrefab);
            itemObj.transform.SetParent(itemsTrans, false);
            //��Ӳ���ʼ��Item�ű�
            item = itemObj.AddComponent<Item>();
            item.Init(itemData, grid);
            //��������
            if (!grid.TryAutoPlaceItem(item))
            {
                //ɾ���������Ʒ
                item.DeleteMe();
                //��ʾ
                UIManager.Instance.ShowTipInfo("�ռ䲻�㣬��Ʒ����ʧ��");
                break;
            }
        }
    }

    /// <summary>
    /// ������Ʒ�ĳɳ��ٶ�
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="growSpeed"></param>
    public void SetItemGrowSpeed(string itemName,int growSpeed)
    {
        //���÷������ֵ���Ʒ�ɳ��ٶ�
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
    /// ����ָ��������������Ʒ
    /// </summary>
    /// <param name="grid"></param>
    public void ClearAllItem(BaseGrid grid,bool isUpdateInfo = false)
    {
        grid.DestroyAllItems(isUpdateInfo);
    }

    /// <summary>
    /// ����սǰ׼���׶εķ�������Ϣ
    /// </summary>
    public void UpdateFightTowerInfo()
    {
        //������Ĭ�ϴ��ڣ����������Ҫ�п�
        BagGrid bagGrid = GetGridByName("Bag") as BagGrid;
        if (bagGrid != null)
        {
            bagGrid.CalculateAttribute();
        }
    }

    //����Ʒ�ɳ�
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