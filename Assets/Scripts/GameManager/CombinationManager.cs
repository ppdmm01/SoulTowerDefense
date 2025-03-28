using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class CombinationManager : Singleton<CombinationManager>
{
    private CombinationManagerSO data;

    private List<CombinationSO> oldCombinations; //上一个组合
    private List<CombinationSO> nowCombinations; //当前有的组合

    private Dictionary<CombinationSO, List<Item>> combinationItemDir; //获取激活的组合对应的物品

    private CombinationManager()
    {
        if (data == null)
        {
            data = Resources.Load<CombinationManagerSO>("Data/CombinationManagerSO");
            if (data == null)
                Debug.LogError("加载CombinationManagerSO失败！");
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
                //激活组合
                activeCombinations.Add(combination);
            }
        }
        oldCombinations = nowCombinations;
        nowCombinations = activeCombinations;
        return activeCombinations;
    }

    /// <summary>
    /// 检测组合是否满足激活条件
    /// </summary>
    private bool CheckCombination(CombinationSO combination,BaseGrid grid)
    {
        /*
         * 物品的相对位置目前以物品中心点来判断（目前没有特别奇怪的形状）
         */
        switch (combination.posType)
        {
            case CombinationSO.CombinationPosType.Any:
                if (combination.combinationType == CombinationSO.ItemCombinationType.Tag)
                {
                    //找到匹配标签的所有物品
                    List<Item> list = grid.items.Where(item => item.data.itemTags.Any(tag => tag == combination.tag)).ToList();
                    if (list.Count < combination.itemNum)
                    {
                        return false; //不满足组合数量要求
                    }
                    else
                    {
                        //记录激活的组合的物品
                        if (!combinationItemDir.ContainsKey(combination))
                            combinationItemDir.Add(combination, list.GetRange(0, combination.itemNum));
                    }
                }
                else if (combination.combinationType == CombinationSO.ItemCombinationType.Data)
                {
                    List<Item> list = new List<Item>();
                    foreach (ItemSO requireItem in combination.items)
                    {
                        //注：可能会出现一种情况：需要的物品里有多个同样类型的，目前都是获取第一个，会重复，如果遇到这种情况，要重新修改逻辑。
                        if (!grid.items.Any(item => item.data == requireItem))
                            return false; //不满足组合对应物品要求
                        else
                            list.Add(grid.items.FirstOrDefault(item => item.data == requireItem)); //记录符合要求的物品
                    }
                    //记录激活的组合的物品
                    if (!combinationItemDir.ContainsKey(combination))
                        combinationItemDir.Add(combination, list);
                }
                return true;
            case CombinationSO.CombinationPosType.UpAndDown:
                if (!grid.items.Any(item => item.data == combination.item1) ||
                    !grid.items.Any(item => item.data == combination.item2))
                    return false; 
                //检查所有的“上方物品”是否有对应“下方物品”
                List<Item> upItems = grid.items.Where(item => item.data == combination.item1).ToList();
                foreach (Item upitem in upItems)
                {
                    List<Item> aroundItems = upitem.GetAroundItems();
                    //周围包含指定物品 并且 该物品在upitem的下方，则满足条件
                    if (aroundItems.Any(item => item.data == combination.item2 
                        && item.transform.position.y < upitem.transform.position.y))
                    {
                        //获取下方物品
                        Item downItem = aroundItems.FirstOrDefault(item => item.data == combination.item2
                                        && item.transform.position.y < upitem.transform.position.y);
                        //记录激活的组合的物品
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
                //检查所有的“左方物品”是否有对应“右方物品”
                List<Item> leftItems = grid.items.Where(item => item.data == combination.item1).ToList();
                foreach (Item leftItem in leftItems)
                {
                    List<Item> aroundItems = leftItem.GetAroundItems();
                    //周围包含指定物品 并且 该物品在leftItem的右方
                    if (aroundItems.Any(item => item.data == combination.item2
                        && item.transform.position.x > leftItem.transform.position.x))
                    {
                        //获取右方物品
                        Item rightItem = aroundItems.FirstOrDefault(item => item.data == combination.item2
                                        && item.transform.position.x > leftItem.transform.position.x);
                        //记录激活的组合的物品
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
                        //获取周围物品
                        Item aroundItem = aroundItems.FirstOrDefault(item => item.data == combination.item2);
                        //记录激活的组合的物品
                        if (!combinationItemDir.ContainsKey(combination))
                            combinationItemDir.Add(combination, new List<Item>() { item, aroundItem });
                        return true; //周围包含指定物品
                    }
                }
                return false;
            default:
                return false;
        }
    }

    public List<CombinationSO> GetChangeCombinationInfo()
    {
        return nowCombinations.Except(oldCombinations).ToList(); //获取组合变化时 新增的组合
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
