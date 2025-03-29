using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店物品栏（BagGrid的改版）
/// </summary>
public class StoreGrid : SelectItemGrid
{
    public TextMeshProUGUI priceTxt; //价格文本

    /// <summary>
    /// 刷新物品
    /// </summary>
    public void RefreshItem(ItemSO itemData)
    {
        //清楚货架上的物品
        DestroyAllItems(false);
        //添加物品
        GridManager.Instance.AddItem(itemData.itemName,this);
        //设置价格
        priceTxt.text = itemData.price.ToString();
    }
}