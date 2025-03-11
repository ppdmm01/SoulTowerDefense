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

    //�ϳ���Ʒ
    public void Synthesis()
    {
        List<SynthesisSO> list = ForgeManager.Instance.data.synthesisSOList; //��ȡ�����䷽
        foreach (SynthesisSO s in list)
        {
            if (CheckRepice(s))
            {
                if (productGrid.items.Count > 0)
                {
                    UIManager.Instance.ShowTipInfo("���Ƚ���Ʒȡ�ߣ�");
                    return;
                }
                //�ϳɳɹ�
                BagManager.Instance.AddItemById(s.product.id, productGrid);
                //����¯���ڵ�������Ʒ
                BagManager.Instance.ClearAllItem(this);
                UIManager.Instance.ShowTipInfo("�ϳɳɹ�");
                return;
            }
        }
        //�ϳ�ʧ��
        UIManager.Instance.ShowTipInfo("�ϳ�ʧ�ܣ�û�и��䷽");
        return;
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
}
