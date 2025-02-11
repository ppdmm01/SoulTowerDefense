using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ģʽ���ࣨ�̳�MonoBehaviour��
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
        //��֤������ֻ��Ψһһ���ű�
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
        //��������ɾ������֤����Ϸ�������������ж�����
        DontDestroyOnLoad(this.gameObject);
    }
}
