using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包网格
/// </summary>
public class BagGrid : BaseGrid
{
    public override void PlaceItem(Item item, Vector2Int gridPos)
    {
        base.PlaceItem(item, gridPos);
        //同时更新面板信息防御塔
        GridManager.Instance.UpdateFightTowerInfo();
    }

    public override void UpdateGrid(GridData gridData)
    {
        base.UpdateGrid(gridData);
        //同时更新面板信息防御塔
        GridManager.Instance.UpdateFightTowerInfo();
    }

    #region 物品属性效果计算（计算最终玩家实力）
    /// <summary>
    /// 计算属性
    /// </summary>
    public void CalculateAttribute()
    {
        TowerManager.Instance.RecordOldData(); //数据变化前先记录老数据
        CalculateTower();
        CalculateItemAttribute();
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
            List<Item> neighbors = item.GetConnectItems(); //获取该物品周围有效的激活物品（无法知道激活的是哪个属性）

            //遍历物品的所有属性
            foreach (ItemAttribute attribute in item.data.itemAttributes)
            {
                //全局属性
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                {
                    if (attribute.condition.conditionType == ItemActiveCondition.ConditionType.Tag)
                        TowerManager.Instance.SetTowerDataFromTag(attribute.condition.tags, attribute.activeEffects);
                    else
                        TowerManager.Instance.SetTowerDataFromName(attribute.condition.name, attribute.activeEffects);
                }
                //联动属性
                else
                {
                    //对激活物品进行遍历，检查是否满足该物品属性（一个物品可能会有许多个联动属性）
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