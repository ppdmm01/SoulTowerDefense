using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public class BagGrid : BaseGrid
{
    public override void PlaceItem(Item item, Vector2Int gridPos, bool isUpdateCombination = true)
    {
        base.PlaceItem(item, gridPos, isUpdateCombination);
        //ͬʱ���������Ϣ������
        if (item.isUpdateInfo)
        {
            //�ж��Ƿ�Ҫ���������Ϣ
            if (isUpdateCombination) CalculateAttribute(item.transform);
            else CalculateAttribute(null);
        }
        //GridManager.Instance.UpdateFightTowerInfo();
    }

    public override void RemoveItem(Item item, Vector2Int gridPos)
    {
        base.RemoveItem(item, gridPos);
        //ͬʱ���������Ϣ������
        if (item.isUpdateInfo)
            CalculateAttribute(item.transform);
        //GridManager.Instance.UpdateFightTowerInfo();
    }

    public override void UpdateGrid(GridData gridData)
    {
        base.UpdateGrid(gridData);
        //ͬʱ���������Ϣ������
        //GridManager.Instance.UpdateFightTowerInfo();
        CalculateAttribute();
    }

    #region ��Ʒ����Ч�����㣨�����������ʵ����
    /// <summary>
    /// ��������
    /// </summary>
    public void CalculateAttribute(Transform itemTrans = null)
    {
        TowerManager.Instance.RecordOldData(); //���ݱ仯ǰ�ȼ�¼������
        CalculateTower(); //��������Щ����������
        CalculateItemAttribute(); //������Ʒ����
        CalculateCombination(itemTrans); //�������
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
            List<ConnectItemInfo> neighborItemInfos = item.GetConnectItems(); //��ȡ����Ʒ��Χ��Ч�ļ�����Ʒ���޷�֪����������ĸ����ԣ�

            //�ȼ������Ʒ���е�ȫ������
            foreach (ItemAttribute attribute in item.nowAttributes)
            {
                //ȫ������
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                {
                    if (attribute.condition.conditionType == ItemActiveCondition.ConditionType.Tag)
                        TowerManager.Instance.SetTowerDataFromTag(attribute.condition.tags, attribute.activeEffects);
                    else
                        TowerManager.Instance.SetTowerDataFromName(attribute.condition.name, attribute.activeEffects);
                }
            }
            //�ټ����Ѽ������������
            foreach (ConnectItemInfo info in neighborItemInfos)
            {
                TowerManager.Instance.SetTowerDataFromName(info.item.data.itemName, info.activateAttribute.activeEffects);
            }
        }
    }

    //�������
    public void CalculateCombination(Transform itemTrans)
    {
        List<CombinationSO> activeCombinations = CombinationManager.Instance.CheckAllCombination(this);
        //Ŀǰֻ��ȫ������
        foreach (CombinationSO combination in activeCombinations)
        {
            ItemAttribute attribute = combination.activeAttribute;
            if (attribute.attributeType == ItemAttribute.AttributeType.Global)
            {
                if (attribute.condition.conditionType == ItemActiveCondition.ConditionType.Tag)
                    TowerManager.Instance.SetTowerDataFromTag(attribute.condition.tags, attribute.activeEffects);
                else
                    TowerManager.Instance.SetTowerDataFromName(attribute.condition.name, attribute.activeEffects);
            }
        }
        //��ʾ�ôβ������γɵ����
        if (itemTrans == null) return;
        List<CombinationSO> newCombinations = CombinationManager.Instance.GetChangeCombinationInfo();
        if (newCombinations.Count > 0)
        {
            AudioManager.Instance.PlaySound("SoundEffect/Combination");
            //��Ч
            //GameObject effObj = PoolMgr.Instance.GetObj("Effect/CombinationEffect");
            //effObj.transform.position = itemTrans.position;
        }
        foreach (CombinationSO combination in newCombinations)
        {
            UIManager.Instance.ShowTxtPopup(combination.combinationName,Color.yellow,64, itemTrans.position,true);
        }
    }
    #endregion
}