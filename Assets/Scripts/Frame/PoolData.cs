using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// 抽屉数据（池子中的数据）
/// </summary>
public class PoolData
{
    //存储抽屉中的对象
    protected Stack<GameObject> dataStack = new Stack<GameObject>();
    //存储正在使用中的对象（即抽屉外的对象）
    protected List<GameObject> usedList = new List<GameObject>();
    //抽屉根对象
    protected GameObject rootObj;
    //对象最大上限
    protected int maxNum;

    public PoolData() { }

    /// <summary>
    /// 初始化构造函数，创建抽屉并放进柜子中，同时将对象添加进使用列表中
    /// </summary>
    /// <param name="root">缓存池（柜子）根对象</param>
    /// <param name="name">抽屉名</param>
    /// <param name="usedObj">使用的对象</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        //如果开启布局功能
        if (PoolMgr.isOpenLayout)
        {
            //创建抽屉根对象
            rootObj = new GameObject(name);
            //将抽屉放在柜子根对象下，建立父子关系
            rootObj.transform.SetParent(root.transform);
        }
        //将使用的对象添加进列表中（创建抽屉时肯定会创建一个对象）
        PushUsedList(usedObj);

        //获取对象最大上限
        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            maxNum = 1000;
            Debug.Log("请为对象挂载PoolObj脚本，用于设置对象最大上限，已默认上限为1000");
            return;
        }
        else
        {
            //记录上限
            maxNum = poolObj.maxNum;
        }
    }

    //抽屉中对象数量
    public int Count => dataStack.Count;

    //使用中的对象的数量
    public int UsedCount => usedList.Count;

    //对象是否超过最大上限
    public bool isOverLimit => usedList.Count >= maxNum;

    /// <summary>
    /// 弹出抽屉中的数据对象
    /// </summary>
    /// <returns>对应的数据对象</returns>
    public virtual GameObject Pop()
    {
        //获取对象
        GameObject obj;
        if (Count > 0)
        {
            //如果抽屉有对象就直接取
            obj = dataStack.Pop();
            //添加进usedList中
            usedList.Add(obj);
        }
        else
        {
            //否则就在usedList中直接抢使用最久的资源来用
            //第0号索引就是使用时间最久的
            obj = usedList[0];
            //将使用的物体放到最后一个索引,表示最新的物体（先移除再添加进去）
            usedList.RemoveAt(0);
            usedList.Add(obj);
        }
        //将对象激活
        obj.SetActive(true);
        //删除对象身上所有动画
        DOTween.Kill(obj.transform);
        //如果开启布局功能
        if (PoolMgr.isOpenLayout)
        {
            //取出时断开父子关系
            obj.transform.SetParent(rootObj.transform.parent);
        }
        return obj;
    }

    /// <summary>
    /// 将物体放回到抽屉中
    /// </summary>
    /// <param name="obj">放回的物体</param>
    public virtual void Push(GameObject obj)
    {
        //添加对象到抽屉中
        dataStack.Push(obj);
        //将对象失活
        obj.SetActive(false);
        //如果开启布局功能
        if (PoolMgr.isOpenLayout)
        {
            //放到抽屉根对象下
            obj.transform.SetParent(rootObj.transform);
        }
        //将对象从usedList中移除
        usedList.Remove(obj);
    }

    /// <summary>
    /// 往使用中的对象列表中添加对象
    /// </summary>
    /// <param name="obj">添加的对象</param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}
