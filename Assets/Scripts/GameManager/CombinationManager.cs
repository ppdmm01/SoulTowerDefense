using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CombinationManager : Singleton<CombinationManager>
{
    private CombinationManagerSO data;

    private List<CombinationSO> oldCombinations; //��һ�����
    private List<CombinationSO> nowCombinations; //��ǰ�е����

    private CombinationManager()
    {
        if (data == null)
        {
            data = Resources.Load<CombinationManagerSO>("Data/CombinationManagerSO");
            if (data == null)
                Debug.LogError("����CombinationManagerSOʧ�ܣ�");
        }
        nowCombinations = new List<CombinationSO>();
        oldCombinations = nowCombinations;
    }

    public List<CombinationSO> CheckAllCombination(BaseGrid grid)
    {
        List<CombinationSO> activeCombinations = new List<CombinationSO>();
        foreach (CombinationSO combination in data.combinationList)
        {
            if (CheckCombination(combination, grid))
            {
                //�������
                activeCombinations.Add(combination);
            }
        }
        oldCombinations = nowCombinations;
        nowCombinations = activeCombinations;
        return activeCombinations;
    }

    /// <summary>
    /// �������Ƿ����㼤������
    /// </summary>
    private bool CheckCombination(CombinationSO combination,BaseGrid grid)
    {
        /*
         * ��Ʒ�����λ��Ŀǰ����Ʒ���ĵ����жϣ�Ŀǰû���ر���ֵ���״��
         */
        switch (combination.type)
        {
            case CombinationSO.CombinationPosType.Any:
                foreach (ItemSO requireItem in combination.items)
                {
                    if (!grid.items.Any(item => item.data == requireItem))
                        return false; //�������������Ҫ��
                }
                return true;
            case CombinationSO.CombinationPosType.UpAndDown:
                if (!grid.items.Any(item => item.data == combination.item1) ||
                    !grid.items.Any(item => item.data == combination.item2))
                    return false; 
                //������еġ��Ϸ���Ʒ���Ƿ��ж�Ӧ���·���Ʒ��
                List<Item> upItems = grid.items.Where(item => item.data == combination.item1).ToList();
                foreach (Item upitem in upItems)
                {
                    List<Item> aroundItems = upitem.GetAroundItems();
                    if (aroundItems.Any(item => item.data == combination.item2 
                        && item.transform.position.y < upitem.transform.position.y))
                    {
                        return true; //��Χ����ָ����Ʒ ���� ����Ʒ��upitem���·�
                    }
                }
                return false;
            case CombinationSO.CombinationPosType.LeftAndRight:
                if (!grid.items.Any(item => item.data == combination.item1) ||
                    !grid.items.Any(item => item.data == combination.item2))
                    return false;
                //������еġ�����Ʒ���Ƿ��ж�Ӧ���ҷ���Ʒ��
                List<Item> leftItems = grid.items.Where(item => item.data == combination.item1).ToList();
                foreach (Item leftItem in leftItems)
                {
                    List<Item> aroundItems = leftItem.GetAroundItems();
                    if (aroundItems.Any(item => item.data == combination.item2
                        && item.transform.position.x > leftItem.transform.position.x))
                    {
                        return true; //��Χ����ָ����Ʒ ���� ����Ʒ��leftItem���ҷ�
                    }
                }
                return false;
            case CombinationSO.CombinationPosType.Around:
                if (!grid.items.Any(item => item.data == combination.item1) ||
                    !grid.items.Any(item => item.data == combination.item2))
                    return false;
                List<Item> items = grid.items.Where(item => item.data == combination.item1).ToList();
                foreach (Item item in items)
                {
                    List<Item> aroundItems = item.GetAroundItems();
                    if (aroundItems.Any(item => item.data == combination.item2))
                    {
                        return true; //��Χ����ָ����Ʒ
                    }
                }
                return false;
            default:
                return false;
        }
    }

    public List<CombinationSO> GetChangeCombinationInfo()
    {
        return nowCombinations.Except(oldCombinations).ToList(); //��ȡ��ϱ仯ʱ ���������
    }

    public List<CombinationSO> GetNowCombinations()
    {
        return nowCombinations;
    }
}
