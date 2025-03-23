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

    private SynthesisSO nowSynthesis; //当前合成配方

    //合成物品
    public void Synthesis()
    {
        if (CheckAllRepices())
        {
            if (productGrid.items.Count > 0)
            {
                UIManager.Instance.ShowTipInfo("请先将成品取走！");
            }
            else
            {
                //合成成功
                GridManager.Instance.AddItem(nowSynthesis.product.id, productGrid);
                //销毁炉子内的所有物品
                GridManager.Instance.ClearAllItem(this, false);
                ForgePanel panel = UIManager.Instance.GetPanel<ForgePanel>(); //获取位置
                EffectManager.Instance.PlayUIEffect("SmokeUIEffect", panel.forgeTrans.position, 1.3f);
                AudioManager.Instance.PlaySound("SoundEffect/Forge");
                UIManager.Instance.ShowTipInfo("合成成功");
            }
        }
        else
        {
            //合成失败
            UIManager.Instance.ShowTipInfo("合成失败，没有该配方");
        }
    }

    //检查所有合成配方
    private bool CheckAllRepices()
    {
        List<SynthesisSO> list = ForgeManager.Instance.data.synthesisSOList; //获取所有配方
        foreach (SynthesisSO s in list)
        {
            if (CheckRepice(s))
            {
                nowSynthesis = s;
                return true;
            }
        }
        //合成失败
        return false;
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

    public override void PlaceItem(Item item, Vector2Int gridPos, bool isUpdateCombination = true)
    {
        base.PlaceItem(item, gridPos, isUpdateCombination);
        if (CheckAllRepices())
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(true);
        else
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(false);
    }

    public override void RemoveItem(Item item, Vector2Int gridPos)
    {
        base.RemoveItem(item, gridPos);
        if (CheckAllRepices())
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(true);
        else
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(false);
    }
}
