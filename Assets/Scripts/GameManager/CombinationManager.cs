using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CombinationManager : Singleton<CombinationManager>
{
    private CombinationManagerSO data;

    private List<CombinationSO> oldCombinations; //上一个组合
    private List<CombinationSO> nowCombinations; //当前有的组合

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
    }

    public List<CombinationSO> CheckAllCombination(BaseGrid grid)
    {
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
        switch (combination.type)
        {
            case CombinationSO.CombinationPosType.Any:
                foreach (ItemSO requireItem in combination.items)
                {
                    if (!grid.items.Any(item => item.data == requireItem))
                        return false; //不满足组合数量要求
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
                    if (aroundItems.Any(item => item.data == combination.item2 
                        && item.transform.position.y < upitem.transform.position.y))
                    {
                        return true; //周围包含指定物品 并且 该物品在upitem的下方
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
                    if (aroundItems.Any(item => item.data == combination.item2
                        && item.transform.position.x > leftItem.transform.position.x))
                    {
                        return true; //周围包含指定物品 并且 该物品在leftItem的右方
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
}
