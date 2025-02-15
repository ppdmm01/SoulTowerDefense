using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 物品格
/// </summary>
public class ItemSlot : MonoBehaviour
{
    [HideInInspector] public bool isUsed; //是否被占用
    private BagGrid bagGrid; //物品格属于哪个背包
    [HideInInspector] public Item nowItem; //当前物品格的物品

    private Image slotImg;

    void Start()
    {
        nowItem = null;
        slotImg = GetComponent<Image>();
        SetStatus(false);
    }

    /// <summary>
    /// 往格子里添加物品
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        nowItem = item;
        SetStatus(true);
    }

    /// <summary>
    /// 移除当前物品
    /// </summary>
    public void RemoveItem()
    {
        nowItem = null;
        SetStatus(false);
    }

    /// <summary>
    /// 设置所属背包
    /// </summary>
    /// <param name="bagGrid">背包</param>
    public void SetBelongs(BagGrid bagGrid)
    {
        this.bagGrid = bagGrid;
    }

    /// <summary>
    /// 设置使用状态
    /// </summary>
    /// <param name="isUsed">是否被使用</param>
    public void SetStatus(bool isUsed)
    {
        this.isUsed = isUsed;
        if (isUsed)
            SetColor(Defines.invalidColor);
        else
            SetColor(Defines.validColor);
    }

    /// <summary>
    /// 设置物品格颜色
    /// </summary>
    /// <param name="color">颜色</param>
    public void SetColor(Color color)
    {
        slotImg.color = color;
    }
}
