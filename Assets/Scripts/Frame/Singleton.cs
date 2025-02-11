using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ����ģʽ����
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : class
{
    private static T instance;
    //���ڼ����Ķ���
    protected static readonly object lockObj = new object();
    //���Եķ�ʽ
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
                        //��ȡ���Type
                        Type type = typeof(T);
                        //ͨ�������ȡ˽�е��޲ι��캯��
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                        //���ù��캯��
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("�޷��ҵ���Ӧ���޲ι��캯��");
                    }
                }
            }
            return instance;
        }
    }
}

