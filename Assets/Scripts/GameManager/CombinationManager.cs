using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class CombinationManager : Singleton<CombinationManager>
{
    private CombinationManagerSO data;

    private List<CombinationSO> oldCombinations; //��һ�����
    private List<CombinationSO> nowCombinations; //��ǰ�е����

    private Dictionary<CombinationSO, List<Item>> combinationItemDir; //��ȡ�������϶�Ӧ����Ʒ

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
        combinationItemDir = new Dictionary<CombinationSO, List<Item>>();
    }

    public List<CombinationSO> CheckAllCombination(BaseGrid grid)
    {
        combinationItemDir.Clear();
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
        switch (combination.posType)
        {
            case CombinationSO.CombinationPosType.Any:
                if (combination.combinationType == CombinationSO.ItemCombinationType.Tag)
                {
                    //�ҵ�ƥ���ǩ��������Ʒ
                    List<Item> list = grid.items.Where(item => item.data.itemTags.Any(tag => tag == combination.tag)).ToList();
                    if (list.Count < combination.itemNum)
                    {
                        return false; //�������������Ҫ��
                    }
                    else
                    {
                        //��¼�������ϵ���Ʒ
                        if (!combinationItemDir.ContainsKey(combination))
                            combinationItemDir.Add(combination, list.GetRange(0, combination.itemNum));
                    }
                }
                else if (combination.combinationType == CombinationSO.ItemCombinationType.Data)
                {
                    List<Item> list = new List<Item>();
                    foreach (ItemSO requireItem in combination.items)
                    {
                        //ע�����ܻ����һ���������Ҫ����Ʒ���ж��ͬ�����͵ģ�Ŀǰ���ǻ�ȡ��һ�������ظ�������������������Ҫ�����޸��߼���
                        if (!grid.items.Any(item => item.data == requireItem))
                            return false; //��������϶�Ӧ��ƷҪ��
                        else
                            list.Add(grid.items.FirstOrDefault(item => item.data == requireItem)); //��¼����Ҫ�����Ʒ
                    }
                    //��¼�������ϵ���Ʒ
                    if (!combinationItemDir.ContainsKey(combination))
                        combinationItemDir.Add(combination, list);
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
                    //��Χ����ָ����Ʒ ���� ����Ʒ��upitem���·�������������
                    if (aroundItems.Any(item => item.data == combination.item2 
                        && item.transform.position.y < upitem.transform.position.y))
                    {
                        //��ȡ�·���Ʒ
                        Item downItem = aroundItems.FirstOrDefault(item => item.data == combination.item2
                                        && item.transform.position.y < upitem.transform.position.y);
                        //��¼�������ϵ���Ʒ
                        if (!combinationItemDir.ContainsKey(combination))
                            combinationItemDir.Add(combination, new List<Item>() { upitem, downItem });
                        return true;
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
                    //��Χ����ָ����Ʒ ���� ����Ʒ��leftItem���ҷ�
                    if (aroundItems.Any(item => item.data == combination.item2
                        && item.transform.position.x > leftItem.transform.position.x))
                    {
                        //��ȡ�ҷ���Ʒ
                        Item rightItem = aroundItems.FirstOrDefault(item => item.data == combination.item2
                                        && item.transform.position.x > leftItem.transform.position.x);
                        //��¼�������ϵ���Ʒ
                        if (!combinationItemDir.ContainsKey(combination))
                            combinationItemDir.Add(combination, new List<Item>() { leftItem, rightItem });
                        return true;
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
                        //��ȡ��Χ��Ʒ
                        Item aroundItem = aroundItems.FirstOrDefault(item => item.data == combination.item2);
                        //��¼�������ϵ���Ʒ
                        if (!combinationItemDir.ContainsKey(combination))
                            combinationItemDir.Add(combination, new List<Item>() { item, aroundItem });
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

    public List<Item> GetItemsByCombination(CombinationSO combination)
    {
        if (combinationItemDir.ContainsKey(combination))
            return combinationItemDir[combination];
        return null;
    }
}
