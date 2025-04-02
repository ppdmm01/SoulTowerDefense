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
        if (!CheckAllRepices())
        {
            //合成失败
            UIManager.Instance.ShowTipInfo("合成失败，没有该配方");
            return; 
        }

        if (productGrid.items.Count > 0)
        {
            UIManager.Instance.ShowTipInfo("请先将成品取走！");
            return;
        }

        if (GameResManager.Instance.GetTaixuNum() < nowSynthesis.price)
        {
            UIManager.Instance.ShowTipInfo("太虚不足，合成失败");
            return;
        }

        //合成成功
        foreach (SynthesisItem productItem in nowSynthesis.product)
        {
            if (productItem.type == SynthesisItem.ItemType.Tag)
            {
                //随机选一个满足tag的物品
                Item item = null;
                ItemSO data;
                if (nowSynthesis.isRandomTagItem)
                    data = ItemManager.Instance.GetItemDataByTags(productItem.tags); //随机选一个满足tag的物品
                else
                {
                    item = items.FirstOrDefault(item => item.data.isContainTags(productItem.tags)); //从配方中选一个包含目标标签的
                    data = item.data;
                }

                //记录物品目前有哪些buff
                List<BuffType> buffs = new List<BuffType>();
                if (item != null)
                {
                    foreach (BuffType buff in item.nowItemBuffs)
                        if (!buffs.Contains(buff)) buffs.Add(buff); //原有buff
                }
                buffs.Add(nowSynthesis.buff); //新增buff
                if (data != null)
                    GridManager.Instance.AddItem(data.id, productGrid,productItem.num, buffs);
            }
            else
            {
                GridManager.Instance.AddItem(productItem.data.id, productGrid,productItem.num, 
                    new List<BuffType>() { nowSynthesis.buff });
            }
        }
        //销毁炉子内的所有物品
        GridManager.Instance.ClearAllItem(this, false);
        //减去消耗的太虚
        GameResManager.Instance.AddTaixuNum(-nowSynthesis.price);
        ForgePanel panel = UIManager.Instance.GetPanel<ForgePanel>(); //获取位置
        EffectManager.Instance.PlayUIEffect("SmokeUIEffect", panel.forgeTrans.position, 1.3f);
        AudioManager.Instance.PlaySound("SoundEffect/Forge");
        UIManager.Instance.ShowTipInfo("合成成功");
    }

    //检查所有合成配方
    private bool CheckAllRepices()
    {
        List<SynthesisSO> list = ForgeManager.Instance.data.synthesisSOList; //获取所有配方
        List<SynthesisSO> satisfyList = new List<SynthesisSO>(); //所有满足条件的配方
        foreach (SynthesisSO s in list)
        {
            if (CheckRepice(s))
                satisfyList.Add(s);
        }

        if (satisfyList.Count > 0)
        {
            nowSynthesis = satisfyList.Random(); //成功，从中随机挑一个
            return true;
        }
        //合成失败
        return false;
    }

    //检查是否符合该配方
    private bool CheckRepice(SynthesisSO data)
    {
        Debug.Log("正在尝试组合：" + data.name + " RecipeNum:" + data.CountRecipeNum() + " itemCount:" + items.Count);
        if (data.CountRecipeNum() != items.Count) return false; //不满足数量要求

        foreach (SynthesisItem recipe in data.recipe)
        {
            if (recipe.type == SynthesisItem.ItemType.Data)
            {
                Debug.Log("组合:" + data.name + " 物品id：" + recipe.data.id + 
                    " 当前有的id物品的数量:" + CountItemsById(recipe.data.id) + " 配方需要数量:" + recipe.num);
                if (CountItemsById(recipe.data.id) != recipe.num) return false; //不满足数量要求
            }
            else if (recipe.type == SynthesisItem.ItemType.Tag)
            {
                Debug.Log("组合:" + data.name +
                    " 当前有的Tag物品的数量:" + CountItemsByTag(recipe.tags,data) + " 配方需要数量:" + recipe.num);
                if (CountItemsByTag(recipe.tags, data) != recipe.num) return false; //不满足数量要求
            }
        }

        //foreach (SynthesisItem recipe in data.recipe)
        //{
        //    if (recipe.type == SynthesisItem.ItemType.Data)
        //    {
        //        Debug.Log("组合:" + data.name + " id：" + recipe.data.id + " Have:" + CountItemsById(recipe.data.id) + " Need:" + recipe.num);
        //    }
        //    else if (recipe.type == SynthesisItem.ItemType.Tag)
        //    {
        //        Debug.Log("组合:" + data.name + " Tag：" + CountItemsByTag(recipe.tags) + " Need:" + recipe.num);
        //    }
        //}

        return true;
    }

    public override void PlaceItem(Item item, Vector2Int gridPos, bool isUpdateCombination = true)
    {
        base.PlaceItem(item, gridPos, isUpdateCombination);
        if (CheckAllRepices())
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(true,nowSynthesis.price);
        else
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(false,0);
    }

    public override void RemoveItem(Item item, Vector2Int gridPos)
    {
        base.RemoveItem(item, gridPos);
        if (CheckAllRepices())
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(true, nowSynthesis.price);
        else
            UIManager.Instance.GetPanel<ForgePanel>()?.UpdateTip(false, 0);
    }

    //获取指定id的物品数量
    private int CountItemsById(int id)
    {
        int num = 0;

        foreach (Item item in items)
        {
            if (item.data.id == id) num++;
        }
        return num;
    }

    //获取指定标签的物品数量
    private int CountItemsByTag(List<ItemTag> tags,SynthesisSO data)
    {
        int num = 0;

        foreach (Item item in items)
        {
            //满足标签并且不在配方上
            if (item.data.isContainTags(tags) && !data.IsRecipeContainsItemByData(item.data)) num++;
        }
        return num;
    }
}
