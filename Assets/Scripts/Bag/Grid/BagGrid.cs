using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��������
/// </summary>
public class BagGrid : BaseGrid
{
    public override void PlaceItem(Item item, Vector2Int gridPos)
    {
        base.PlaceItem(item, gridPos);
        //ͬʱ���������Ϣ������
        GridManager.Instance.UpdateFightTowerInfo();
    }

    public override void UpdateGrid(GridData gridData)
    {
        base.UpdateGrid(gridData);
        //ͬʱ���������Ϣ������
        GridManager.Instance.UpdateFightTowerInfo();
    }

    #region ��Ʒ����Ч�����㣨�����������ʵ����
    /// <summary>
    /// ��������
    /// </summary>
    public void CalculateAttribute()
    {
        TowerManager.Instance.RecordOldData(); //���ݱ仯ǰ�ȼ�¼������
        CalculateTower();
        CalculateItemAttribute();
        PreFightPanel panel = UIManager.Instance.GetPanel<PreFightPanel>();
        if (panel != null)
        {
            panel.UpdateTowerInfoBtn(); //������Ϣ��ť
            panel.UpdateTowerInfo(panel.nowTowerInfoName); //���µ�ǰչʾ�ķ�����������Ϣ
        }
    }

    /// <summary>
    /// ��������Щ����ʹ�õķ�����
    /// </summary>
    public void CalculateTower()
    {
        TowerManager.Instance.towerDatas.Clear();
        foreach (Item item in items)
        {
            if (item.data.itemTags.Contains(ItemTag.Tower))
            {
                TowerData towerData = new TowerData(TowerManager.Instance.GetTowerSO_ByName(item.data.itemName));
                //��ӷ�����
                TowerManager.Instance.AddTowerData(item.data.itemName, towerData);
            }
        }
    }

    /// <summary>
    /// ������Ʒ�����ԣ�Ŀǰ���Ǹ��������ӵģ�����Ҳ�Ƿ�������
    /// </summary>
    public void CalculateItemAttribute()
    {
        //����������������Ʒ
        foreach (Item item in items)
        {
            List<Item> neighbors = item.GetConnectItems(); //��ȡ����Ʒ��Χ��Ч�ļ�����Ʒ���޷�֪����������ĸ����ԣ�

            //������Ʒ����������
            foreach (ItemAttribute attribute in item.data.itemAttributes)
            {
                //ȫ������
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                {
                    if (attribute.condition.conditionType == ItemActiveCondition.ConditionType.Tag)
                        TowerManager.Instance.SetTowerDataFromTag(attribute.condition.tags, attribute.activeEffects);
                    else
                        TowerManager.Instance.SetTowerDataFromName(attribute.condition.name, attribute.activeEffects);
                }
                //��������
                else
                {
                    //�Լ�����Ʒ���б���������Ƿ��������Ʒ���ԣ�һ����Ʒ���ܻ��������������ԣ�
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