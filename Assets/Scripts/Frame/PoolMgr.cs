using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽屉数据（池子中的数据）
/// </summary>
public class PoolData
{
    //存储抽屉中的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //存储正在使用中的对象（即抽屉外的对象）
    private List<GameObject> usedList = new List<GameObject>();
    //抽屉根对象
    private GameObject rootObj;
    //对象最大上限
    private int maxNum;

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
            Debug.LogError("请为对象挂载PoolObj脚本，用于设置对象最大上限");
            return;
        }
        //记录上限
        maxNum = poolObj.maxNum;
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
    public GameObject Pop()
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
        //如果开启布局功能
        if (PoolMgr.isOpenLayout)
        {
            //取出时断开父子关系
            obj.transform.SetParent(null);
        }
        return obj;
    }

    /// <summary>
    /// 将物体放回到抽屉中
    /// </summary>
    /// <param name="obj">放回的物体</param>
    public void Push(GameObject obj)
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

/// <summary>
/// 对象(缓存)池模块管理器
/// </summary>
public class PoolMgr : Singleton<PoolMgr>
{
    private PoolMgr() { }

    //柜子容器中有抽屉容器的体现
    Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    //柜子（缓存池管理器）根对象
    private GameObject poolObj;
    //是否开启布局功能
    public static bool isOpenLayout = true;

    /// <summary>
    /// 拿东西的方法
    /// </summary>
    /// <param name="name">抽屉容器（对象）的名字</param>
    /// <param name="maxNum">抽屉的最大容量（默认50）</param>
    /// <returns>从缓存池中取出的对象</returns>
    public GameObject GetObj(string name)
    {
        //如果柜子根对象为空并且开启布局功能，则创建根对象
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;
        //如果没有抽屉，则动态创建一个对象并记录到创建的抽屉中
        if (!poolDic.ContainsKey(name))
        {
            //通过资源加载来实例化一个对象
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //更改对象名字
            obj.name = name;
            //创建抽屉
            poolDic.Add(obj.name, new PoolData(poolObj, name, obj));
        }
        //如果有抽屉
        else
        {
            //如果抽屉中有物体 或者 抽屉外正在使用的物体超上限了，则可以直接取出来用
            if (poolDic[name].Count > 0 || poolDic[name].isOverLimit)
            {
                obj = poolDic[name].Pop();
            }
            //如果抽屉中没物体 并且 正在使用的物体也没超过上限，则需要创建
            else
            {
                //通过资源加载来实例化一个对象
                obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
                //更改对象名字
                obj.name = name;
                //记录到usedList中
                poolDic[name].PushUsedList(obj);
            }
        }
        //返回对象
        return obj;
    }

    /// <summary>
    /// 往缓存池中放入对象
    /// </summary>
    /// <param name="obj">放入的对象</param>
    public void PushObj(GameObject obj)
    {
        //将对象压入抽屉当中
        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 清除柜子中的数据
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        //根对象置空
        poolObj = null;
    }
}
