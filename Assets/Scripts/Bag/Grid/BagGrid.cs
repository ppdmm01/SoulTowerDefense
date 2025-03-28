using System;
using System.Collections;
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
    }

    public override void RemoveItem(Item item, Vector2Int gridPos)
    {
        base.RemoveItem(item, gridPos);
        //同时更新面板信息防御塔
        if (item.isUpdateInfo)
            CalculateAttribute(item.transform);
    }

    public override void UpdateGrid(GridData gridData)
    {
        base.UpdateGrid(gridData);
        CalculateAttribute();
    }

    #region 物品属性效果计算（计算最终玩家实力）
    /// <summary>
    /// 计算属性
    /// </summary>
    public void CalculateAttribute(Transform itemTrans = null)
    {
        TowerManager.Instance.RecordOldData(); //数据变化前先记录老数据
        AttributeManager.Instance.ClearAllGrowSpeed(); //清理上一次操作的数据
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
                TowerData towerData = new TowerData(TowerManager.Instance.GetTowerSO_ByName(item.data.itemName),item.nowItemBuffs);
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
            List<ConnectItemInfo> neighborItemInfos = item.GetConnectItems(); //获取该物品周围有效的激活物品信息

            //先计算该物品所有的全局属性
            foreach (ItemAttribute attribute in item.nowAttributes)
            {
                //全局属性
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                {
                    if (attribute.condition.conditionType == ItemActiveCondition.ConditionType.Tag)
                        AttributeManager.Instance.SetAttributeFromTag(attribute.condition.tags, attribute.activeEffects);
                    else
                        AttributeManager.Instance.SetAttributeFromName(attribute.condition.name, attribute.activeEffects);
                }
            }
            //再计算已激活的联动属性
            foreach (ConnectItemInfo info in neighborItemInfos)
            {
                AttributeManager.Instance.SetAttributeFromName(info.item.data.itemName, info.activateAttribute.activeEffects);
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
                    AttributeManager.Instance.SetAttributeFromTag(attribute.condition.tags, attribute.activeEffects);
                else
                    AttributeManager.Instance.SetAttributeFromName(attribute.condition.name, attribute.activeEffects);
            }
        }
        //显示该次操作所形成的组合
        if (itemTrans == null) return;
        List<CombinationSO> newCombinations = CombinationManager.Instance.GetChangeCombinationInfo();
        if (newCombinations.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(ShowCombination(newCombinations, itemTrans));
        }
    }

    /// <summary>
    /// 显示组合
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowCombination(List<CombinationSO> newCombinations, Transform itemTrans)
    {
        foreach (CombinationSO combination in newCombinations)
        {
            AudioManager.Instance.PlaySound("SoundEffect/Combination");
            UIManager.Instance.ShowTxtPopup(combination.combinationName, Color.green, 64, itemTrans.position, true);
            //获取激活组合的物品
            List<Item> items = CombinationManager.Instance.GetItemsByCombination(combination);
            //物品的格子颜色闪亮
            foreach (Item item in items)
            {
                foreach (Vector2Int pos in item.usedPos)
                {
                    item.grid.slots[pos.x, pos.y].Flash();
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion
}