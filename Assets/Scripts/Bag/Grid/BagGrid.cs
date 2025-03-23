using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包网格
/// </summary>
public class BagGrid : BaseGrid
{
    public override void PlaceItem(Item item, Vector2Int gridPos, bool isUpdateCombination = true)
    {
        base.PlaceItem(item, gridPos, isUpdateCombination);
        //同时更新面板信息防御塔
        if (item.isUpdateInfo)
        {
            //判断是否要更新组合信息
            if (isUpdateCombination) CalculateAttribute(item.transform);
            else CalculateAttribute(null);
        }
        //GridManager.Instance.UpdateFightTowerInfo();
    }

    public override void RemoveItem(Item item, Vector2Int gridPos)
    {
        base.RemoveItem(item, gridPos);
        //同时更新面板信息防御塔
        if (item.isUpdateInfo)
            CalculateAttribute(item.transform);
        //GridManager.Instance.UpdateFightTowerInfo();
    }

    public override void UpdateGrid(GridData gridData)
    {
        base.UpdateGrid(gridData);
        //同时更新面板信息防御塔
        //GridManager.Instance.UpdateFightTowerInfo();
        CalculateAttribute();
    }

    #region 物品属性效果计算（计算最终玩家实力）
    /// <summary>
    /// 计算属性
    /// </summary>
    public void CalculateAttribute(Transform itemTrans = null)
    {
        TowerManager.Instance.RecordOldData(); //数据变化前先记录老数据
        CalculateTower(); //计算有哪些防御塔可用
        CalculateItemAttribute(); //计算物品属性
        CalculateCombination(itemTrans); //计算组合
        PreFightPanel panel = UIManager.Instance.GetPanel<PreFightPanel>();
        if (panel != null)
        {
            panel.UpdateTowerInfoBtn(); //更新信息按钮
            panel.UpdateTowerInfo(panel.nowTowerInfoName); //更新当前展示的防御塔属性信息
        }
    }

    /// <summary>
    /// 计算有哪些可以使用的防御塔
    /// </summary>
    public void CalculateTower()
    {
        TowerManager.Instance.towerDatas.Clear();
        foreach (Item item in items)
        {
            if (item.data.itemTags.Contains(ItemTag.Tower))
            {
                TowerData towerData = new TowerData(TowerManager.Instance.GetTowerSO_ByName(item.data.itemName));
                //添加防御塔
                TowerManager.Instance.AddTowerData(item.data.itemName, towerData);
            }
        }
    }

    /// <summary>
    /// 计算物品的属性（目前都是给防御塔加的，联动也是防御塔）
    /// </summary>
    public void CalculateItemAttribute()
    {
        //遍历背包中所有物品
        foreach (Item item in items)
        {
            List<ConnectItemInfo> neighborItemInfos = item.GetConnectItems(); //获取该物品周围有效的激活物品（无法知道激活的是哪个属性）

            //先计算该物品所有的全局属性
            foreach (ItemAttribute attribute in item.nowAttributes)
            {
                //全局属性
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                {
                    if (attribute.condition.conditionType == ItemActiveCondition.ConditionType.Tag)
                        TowerManager.Instance.SetTowerDataFromTag(attribute.condition.tags, attribute.activeEffects);
                    else
                        TowerManager.Instance.SetTowerDataFromName(attribute.condition.name, attribute.activeEffects);
                }
            }
            //再计算已激活的联动属性
            foreach (ConnectItemInfo info in neighborItemInfos)
            {
                TowerManager.Instance.SetTowerDataFromName(info.item.data.itemName, info.activateAttribute.activeEffects);
            }
        }
    }

    //计算组合
    public void CalculateCombination(Transform itemTrans)
    {
        List<CombinationSO> activeCombinations = CombinationManager.Instance.CheckAllCombination(this);
        //目前只有全局属性
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
        //显示该次操作所形成的组合
        if (itemTrans == null) return;
        List<CombinationSO> newCombinations = CombinationManager.Instance.GetChangeCombinationInfo();
        if (newCombinations.Count > 0)
        {
            AudioManager.Instance.PlaySound("SoundEffect/Combination");
            //特效
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