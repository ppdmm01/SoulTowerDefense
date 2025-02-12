using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagManager : SingletonMono<BagManager>
{
    private BagManager() {}

    protected override void Awake()
    {
        base.Awake();
        BagDic = new Dictionary<string, BagGrid>();
        for (int i = 0; i < bags.Count; i++)
        {
            BagDic.Add(bags[i].bagName, bags[i]);
        }
    }

    [SerializeField] private List<BagGrid> bags; //背包列表，（背包可能有多个，比如一个背包，一个储物箱）
    public Dictionary<string, BagGrid> BagDic; //通过字典查找对应背包

    //检测鼠标点是否在指定背包里
    public bool IsInsideBag(Vector2 screenPoint,string bagName)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
           BagDic[bagName].transform as RectTransform,
           screenPoint,
           null
        );

       return isInside;
    }
}
