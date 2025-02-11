using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 单例模式基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : class
{
    private static T instance;
    //用于加锁的对象
    protected static readonly object lockObj = new object();
    //属性的方式
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        //获取类的Type
                        Type type = typeof(T);
                        //通过反射获取私有的无参构造函数
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                        //调用构造函数
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("无法找到对应的无参构造函数");
                    }
                }
            }
            return instance;
        }
    }
}

