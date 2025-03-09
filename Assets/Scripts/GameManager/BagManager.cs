using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class BagManager : SingletonMono<BagManager>
{
    private BagManager() { }

    protected override void Awake()
    {
        base.Awake();
        itemsTrans = UIManager.Instance.itemTrans;
        gridDic = new Dictionary<string, BaseGrid>();
        itemPrefab = Resources.Load<GameObject>("Bag/ItemPrefab");
    }

    [HideInInspector] public Transform itemsTrans; //������Ʒ�ĸ�����
    public Dictionary<string, BaseGrid> gridDic; //ͨ���ֵ䣨���������ж��������һ��������һ�������䣩
    private GameObject itemPrefab;

    //��������Ƿ���ָ��������
    public bool IsInsideGrid(Vector2 screenPoint, string bagName)
    {
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
    public BaseGrid GetBagByName(string bagName)
    {
        return gridDic[bagName];
    }

    //��ָ���������ָ��������Ʒ
    public void AddItemByName(string itemName, BaseGrid grid)
    {
        //GameObject itemObj = Instantiate(Resources.Load<GameObject>("Items/"+name));
        //��������
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //��Ӳ���ʼ��Item�ű�
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataByName(itemName);
        item.Init(data, grid);
        //��������
        if (!grid.TryAutoPlaceItem(item))
        {
            //Debug.LogError($"��Ʒ {item.data.itemName} �޷����ã���������");
            //ɾ���������Ʒ
            item.DeleteMe();
            //��ʾ
            UIManager.Instance.ShowTipInfo("�ռ䲻�㣬��Ʒ����ʧ��");
        }
    }

    //��ָ���������ָ��Id����Ʒ
    public void AddItemById(int id, BaseGrid grid)
    {
        //��������
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.transform.SetParent(itemsTrans, false);
        //��Ӳ���ʼ��Item�ű�
        Item item = itemObj.AddComponent<Item>();
        ItemSO data = ItemManager.Instance.GetItemDataById(id);
        item.Init(data, grid);
        //��������
        if (!grid.TryAutoPlaceItem(item))
        {
            //Debug.LogError($"��Ʒ {item.data.itemName} �޷����ã���������");
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
                //Debug.LogError($"��Ʒ {item.data.itemName} �޷����ã���������");
                //ɾ���������Ʒ
                item.DeleteMe();
                //��ʾ
                UIManager.Instance.ShowTipInfo("�ռ䲻�㣬��Ʒ����ʧ��");
                break;
            }
        }
    }

    /// <summary>
    /// ����ָ��������������Ʒ
    /// </summary>
    /// <param name="grid"></param>
    public void ClearAllItem(BaseGrid grid)
    {
        Debug.Log("����ȫ����Ʒ");
        grid.DestroyAllItems();
    }

    /// <summary>
    /// ������������Ϣ
    /// </summary>
    public void UpdateMainBagInfo()
    {
        //������Ĭ�ϴ��ڣ����������Ҫ�п�
        (GetBagByName("Bag") as BagGrid).CalculateAttribute();
    }
}