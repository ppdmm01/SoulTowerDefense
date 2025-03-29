using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �̵���Ʒ����BagGrid�ĸİ棩
/// </summary>
public class StoreGrid : SelectItemGrid
{
    public TextMeshProUGUI priceTxt; //�۸��ı�

    /// <summary>
    /// ˢ����Ʒ
    /// </summary>
    public void RefreshItem(ItemSO itemData)
    {
        //��������ϵ���Ʒ
        DestroyAllItems(false);
        //�����Ʒ
        GridManager.Instance.AddItem(itemData.itemName,this);
        //���ü۸�
        priceTxt.text = itemData.price.ToString();
    }
}