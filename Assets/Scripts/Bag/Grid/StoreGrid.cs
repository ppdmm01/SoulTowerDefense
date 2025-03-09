using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �̵���Ʒ����BagGrid�ĸİ棩
/// </summary>
public class StoreGrid : BaseGrid
{
    public TextMeshProUGUI priceTxt; //�۸��ı�

    /// <summary>
    /// ˢ����Ʒ
    /// </summary>
    public void RefreshItem(ItemSO itemData)
    {
        //��������ϵ���Ʒ
        DestroyAllItems();
        //�����Ʒ
        BagManager.Instance.AddItemByName(itemData.itemName,this);
        //���ü۸�
        priceTxt.text = itemData.price.ToString();
    }

    /// <summary>
    /// ���Է�����Ʒ
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool TryAutoPlaceItem(Item item)
    {
        // ������ת�Ƕ�  
        item.currentRotation = 0;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        //�̶�λ�÷���
        int x = gridWidth / 2 - 1;
        int y = gridHeight / 2;
        item.gridPos = new Vector2Int(x, y);
        if (CanPlaceItem(item, item.gridPos))
        {
            PlaceItem(item, item.gridPos);
            item.oldGridPos = item.gridPos; //������λ������
            return true;
        }
        return false;
    }
}