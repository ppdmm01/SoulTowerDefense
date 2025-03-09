using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对象(缓存)池模块管理器
/// </summary>
public class PoolMgr : Singleton<PoolMgr>
{
    private PoolMgr() { }

    //柜子容器中有抽屉容器的体现
    Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //存储UI相关的对象池
    Dictionary<string, PoolData> UIPoolDic = new Dictionary<string, PoolData>();

    //柜子（缓存池管理器）根对象
    private GameObject poolObj;
    //UI柜子根对象
    private GameObject UIPoolObj;
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
        //如果柜子根对象为空，则创建根对象
        if (poolObj == null)
            poolObj = new GameObject("Pool");

        GameObject obj;
        //如果没有抽屉，则动态创建一个对象并记录到创建的抽屉中
        if (!poolDic.ContainsKey(name))
        {
            //通过资源加载来实例化一个对象
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name), poolObj.transform);
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
                obj = GameObject.Instantiate(Resources.Load<GameObject>(name), poolObj.transform);
                //更改对象名字
                obj.name = name;
                //记录到usedList中
                poolDic[name].PushUsedList(obj);
            }
        }
        //返回对象
        return obj;
    }

    public GameObject GetUIObj(string name)
    {
        //如果柜子根对象为空，则创建根对象
        if (UIPoolObj == null)
            UIPoolObj = UIManager.Instance.topCanvasTrans.gameObject;

        GameObject obj;
        //如果没有抽屉，则动态创建一个对象并记录到创建的抽屉中
        if (!UIPoolDic.ContainsKey(name))
        {
            //通过资源加载来实例化一个对象
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name), UIPoolObj.transform);
            //更改对象名字
            obj.name = name;
            //创建抽屉
            UIPoolDic.Add(obj.name, new PoolData(UIPoolObj, name, obj));
        }
        //如果有抽屉
        else
        {
            //如果抽屉中有物体 或者 抽屉外正在使用的物体超上限了，则可以直接取出来用
            if (UIPoolDic[name].Count > 0 || UIPoolDic[name].isOverLimit)
            {
                obj = UIPoolDic[name].Pop();
            }
            //如果抽屉中没物体 并且 正在使用的物体也没超过上限，则需要创建
            else
            {
                //通过资源加载来实例化一个对象
                obj = GameObject.Instantiate(Resources.Load<GameObject>(name), UIPoolObj.transform);
                //更改对象名字
                obj.name = name;
                //记录到usedList中
                UIPoolDic[name].PushUsedList(obj);
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
        obj.transform.position = Vector2.one * 1000; //移到看不见的位置
        //将对象压入抽屉当中
        if (poolDic.ContainsKey(obj.name))
            poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 往缓存池中放入UI对象
    /// </summary>
    /// <param name="obj">放入的对象</param>
    public void PushUIObj(GameObject obj)
    {
        //将对象压入抽屉当中
        if (UIPoolDic.ContainsKey(obj.name))
            UIPoolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 清除柜子中的数据
    /// </summary>
    public void ClearPool()
    {
        foreach (PoolData pool in poolDic.Values)
        {
            while (pool.Count > 0)
                GameObject.Destroy(pool.Pop());
            GameObject.Destroy(pool.rootObj); //删除根物体
        }
        foreach (PoolData pool in UIPoolDic.Values)
        {
            while (pool.Count > 0)
                GameObject.Destroy(pool.Pop());
            GameObject.Destroy(pool.rootObj); //删除根物体
        }
        poolDic.Clear();
        UIPoolDic.Clear();
        //根对象置空
        if (poolObj != null)
        {
            GameObject.Destroy(poolObj);
            poolObj = null;
        }
        //UIPoolObj不用置空，是Don'tDestroy物体
        //if (UIPoolObj != null)
        //{
        //    GameObject.DestroyImmediate(UIPoolObj);
        //    UIPoolObj = null;
        //}
    }
}
