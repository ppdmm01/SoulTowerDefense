using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 锻造炉网格
/// </summary>
public class ForgeGrid : BaseGrid
{
    public BaseGrid productGrid; //合成物品后放置的目标网格

    //合成物品
    public void Synthesis()
    {
        List<SynthesisSO> list = ForgeManager.Instance.data.synthesisSOList; //获取所有配方
        foreach (SynthesisSO s in list)
        {
            if (CheckRepice(s))
            {
                if (productGrid.items.Count > 0)
                {
                    UIManager.Instance.ShowTipInfo("请先将成品取走！");
                    return;
                }
                //合成成功
                BagManager.Instance.AddItemById(s.product.id, productGrid);
                //销毁炉子内的所有物品
                BagManager.Instance.ClearAllItem(this);
                UIManager.Instance.ShowTipInfo("合成成功");
                return;
            }
        }
        //合成失败
        UIManager.Instance.ShowTipInfo("合成失败，没有该配方");
        return;
    }

    //检查是否符合该配方
    private bool CheckRepice(SynthesisSO data)
    {
        List<ItemSO> repice = data.recipe.OrderBy(item => item.id).ToList();
        List<Item> nowItems = items.OrderBy(item => item.data.id).ToList();
        
        if (repice.Count != nowItems.Count) return false; //数量不一样，失败
        for (int i = 0; i < repice.Count; i++)
        {
            if (repice[i].id != nowItems[i].data.id) return false; //id不一样，失败
        }
        return true;
    }
}
