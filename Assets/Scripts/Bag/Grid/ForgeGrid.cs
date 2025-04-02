using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ����¯����
/// </summary>
public class ForgeGrid : BaseGrid
{
    public BaseGrid productGrid; //�ϳ���Ʒ����õ�Ŀ������

    private SynthesisSO nowSynthesis; //��ǰ�ϳ��䷽

    //�ϳ���Ʒ
    public void Synthesis()
    {
        if (!CheckAllRepices())
        {
            //�ϳ�ʧ��
            UIManager.Instance.ShowTipInfo("�ϳ�ʧ�ܣ�û�и��䷽");
            return; 
        }

        if (productGrid.items.Count > 0)
        {
            UIManager.Instance.ShowTipInfo("���Ƚ���Ʒȡ�ߣ�");
            return;
        }

        if (GameResManager.Instance.GetTaixuNum() < nowSynthesis.price)
        {
            UIManager.Instance.ShowTipInfo("̫�鲻�㣬�ϳ�ʧ��");
            return;
        }

        //�ϳɳɹ�
        foreach (SynthesisItem productItem in nowSynthesis.product)
        {
            if (productItem.type == SynthesisItem.ItemType.Tag)
            {
                //���ѡһ������tag����Ʒ
                Item item = null;
                ItemSO data;
                if (nowSynthesis.isRandomTagItem)
                    data = ItemManager.Instance.GetItemDataByTags(productItem.tags); //���ѡһ������tag����Ʒ
                else
                {
                    item = items.FirstOrDefault(item => item.data.isContainTags(productItem.tags)); //���䷽��ѡһ������Ŀ���ǩ��
                    data = item.data;
                }

                //��¼��ƷĿǰ����Щbuff
                List<BuffType> buffs = new List<BuffType>();
                if (item != null)
                {
                    foreach (BuffType buff in item.nowItemBuffs)
                        if (!buffs.Contains(buff)) buffs.Add(buff); //ԭ��buff
                }
                buffs.Add(nowSynthesis.buff); //����buff
                if (data != null)
                    GridManager.Instance.AddItem(data.id, productGrid,productItem.num, buffs);
            }
            else
            {
                GridManager.Instance.AddItem(productItem.data.id, productGrid,productItem.num, 
                    new List<BuffType>() { nowSynthesis.buff });
            }
        }
        //����¯���ڵ�������Ʒ
        GridManager.Instance.ClearAllItem(this, false);
        //��ȥ���ĵ�̫��
        GameResManager.Instance.AddTaixuNum(-nowSynthesis.price);
        ForgePanel panel = UIManager.Instance.GetPanel<ForgePanel>(); //��ȡλ��
        EffectManager.Instance.PlayUIEffect("SmokeUIEffect", panel.forgeTrans.position, 1.3f);
        AudioManager.Instance.PlaySound("SoundEffect/Forge");
        UIManager.Instance.ShowTipInfo("�ϳɳɹ�");
    }

    //������кϳ��䷽
    private bool CheckAllRepices()
    {
        List<SynthesisSO> list = ForgeManager.Instance.data.synthesisSOList; //��ȡ�����䷽
        List<SynthesisSO> satisfyList = new List<SynthesisSO>(); //���������������䷽
        foreach (SynthesisSO s in list)
        {
            if (CheckRepice(s))
                satisfyList.Add(s);
        }

        if (satisfyList.Count > 0)
        {
            nowSynthesis = satisfyList.Random(); //�ɹ������������һ��
            return true;
        }
        //�ϳ�ʧ��
        return false;
    }

    //����Ƿ���ϸ��䷽
    private bool CheckRepice(SynthesisSO data)
    {
        Debug.Log("���ڳ�����ϣ�" + data.name + " RecipeNum:" + data.CountRecipeNum() + " itemCount:" + items.Count);
        if (data.CountRecipeNum() != items.Count) return false; //����������Ҫ��

        foreach (SynthesisItem recipe in data.recipe)
        {
            if (recipe.type == SynthesisItem.ItemType.Data)
            {
                Debug.Log("���:" + data.name + " ��Ʒid��" + recipe.data.id + 
                    " ��ǰ�е�id��Ʒ������:" + CountItemsById(recipe.data.id) + " �䷽��Ҫ����:" + recipe.num);
                if (CountItemsById(recipe.data.id) != recipe.num) return false; //����������Ҫ��
            }
            else if (recipe.type == SynthesisItem.ItemType.Tag)
            {
                Debug.Log("���:" + data.name +
                    " ��ǰ�е�Tag��Ʒ������:" + CountItemsByTag(recipe.tags,data) + " �䷽��Ҫ����:" + recipe.num);
                if (CountItemsByTag(recipe.tags, data) != recipe.num) return false; //����������Ҫ��
            }
        }

        //foreach (SynthesisItem recipe in data.recipe)
        //{
        //    if (recipe.type == SynthesisItem.ItemType.Data)
        //    {
        //        Debug.Log("���:" + data.name + " id��" + recipe.data.id + " Have:" + CountItemsById(recipe.data.id) + " Need:" + recipe.num);
        //    }
        //    else if (recipe.type == SynthesisItem.ItemType.Tag)
        //    {
        //        Debug.Log("���:" + data.name + " Tag��" + CountItemsByTag(recipe.tags) + " Need:" + recipe.num);
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

    //��ȡָ��id����Ʒ����
    private int CountItemsById(int id)
    {
        int num = 0;

        foreach (Item item in items)
        {
            if (item.data.id == id) num++;
        }
        return num;
    }

    //��ȡָ����ǩ����Ʒ����
    private int CountItemsByTag(List<ItemTag> tags,SynthesisSO data)
    {
        int num = 0;

        foreach (Item item in items)
        {
            //�����ǩ���Ҳ����䷽��
            if (item.data.isContainTags(tags) && !data.IsRecipeContainsItemByData(item.data)) num++;
        }
        return num;
    }
}
