using System.Collections;
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
        if (CheckAllRepices())
        {
            if (productGrid.items.Count > 0)
            {
                UIManager.Instance.ShowTipInfo("���Ƚ���Ʒȡ�ߣ�");
            }
            else
            {
                //�ϳɳɹ�
                GridManager.Instance.AddItem(nowSynthesis.product.id, productGrid);
                //����¯���ڵ�������Ʒ
                GridManager.Instance.ClearAllItem(this, false);
                ForgePanel panel = UIManager.Instance.GetPanel<ForgePanel>(); //��ȡλ��
                EffectManager.Instance.PlayUIEffect("SmokeUIEffect", panel.forgeTrans.position, 1.3f);
                AudioManager.Instance.PlaySound("SoundEffect/Forge");
                UIManager.Instance.ShowTipInfo("�ϳɳɹ�");
            }
        }
        else
        {
            //�ϳ�ʧ��
            UIManager.Instance.ShowTipInfo("�ϳ�ʧ�ܣ�û�и��䷽");
        }
    }

    //������кϳ��䷽
    private bool CheckAllRepices()
    {
        List<SynthesisSO> list = ForgeManager.Instance.data.synthesisSOList; //��ȡ�����䷽
        foreach (SynthesisSO s in list)
        {
            if (CheckRepice(s))
            {
                nowSynthesis = s;
                return true;
            }
        }
        //�ϳ�ʧ��
        return false;
    }

    //����Ƿ���ϸ��䷽
    private bool CheckRepice(SynthesisSO data)
    {
        List<ItemSO> repice = data.recipe.OrderBy(item => item.id).ToList();
        List<Item> nowItems = items.OrderBy(item => item.data.id).ToList();
        
        if (repice.Count != nowItems.Count) return false; //������һ����ʧ��
        for (int i = 0; i < repice.Count; i++)
        {
            if (repice[i].id != nowItems[i].data.id) return false; //id��һ����ʧ��
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
