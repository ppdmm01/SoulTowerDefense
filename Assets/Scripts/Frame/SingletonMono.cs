using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式基类（继承MonoBehaviour）
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        //保证场景中只有唯一一个脚本
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
        //过场景不删除，保证在游戏整个生命周期中都存在
        DontDestroyOnLoad(this.gameObject);
    }
}
