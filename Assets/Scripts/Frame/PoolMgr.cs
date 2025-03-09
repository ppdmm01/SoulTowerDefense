using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����(����)��ģ�������
/// </summary>
public class PoolMgr : Singleton<PoolMgr>
{
    private PoolMgr() { }

    //�����������г�������������
    Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //�洢UI��صĶ����
    Dictionary<string, PoolData> UIPoolDic = new Dictionary<string, PoolData>();

    //���ӣ�����ع�������������
    private GameObject poolObj;
    //UI���Ӹ�����
    private GameObject UIPoolObj;
    //�Ƿ������ֹ���
    public static bool isOpenLayout = true;

    /// <summary>
    /// �ö����ķ���
    /// </summary>
    /// <param name="name">�������������󣩵�����</param>
    /// <param name="maxNum">��������������Ĭ��50��</param>
    /// <returns>�ӻ������ȡ���Ķ���</returns>
    public GameObject GetObj(string name)
    {
        //������Ӹ�����Ϊ�գ��򴴽�������
        if (poolObj == null)
            poolObj = new GameObject("Pool");

        GameObject obj;
        //���û�г��룬��̬����һ�����󲢼�¼�������ĳ�����
        if (!poolDic.ContainsKey(name))
        {
            //ͨ����Դ������ʵ����һ������
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name), poolObj.transform);
            //���Ķ�������
            obj.name = name;
            //��������
            poolDic.Add(obj.name, new PoolData(poolObj, name, obj));
        }
        //����г���
        else
        {
            //��������������� ���� ����������ʹ�õ����峬�����ˣ������ֱ��ȡ������
            if (poolDic[name].Count > 0 || poolDic[name].isOverLimit)
            {
                obj = poolDic[name].Pop();
            }
            //���������û���� ���� ����ʹ�õ�����Ҳû�������ޣ�����Ҫ����
            else
            {
                //ͨ����Դ������ʵ����һ������
                obj = GameObject.Instantiate(Resources.Load<GameObject>(name), poolObj.transform);
                //���Ķ�������
                obj.name = name;
                //��¼��usedList��
                poolDic[name].PushUsedList(obj);
            }
        }
        //���ض���
        return obj;
    }

    public GameObject GetUIObj(string name)
    {
        //������Ӹ�����Ϊ�գ��򴴽�������
        if (UIPoolObj == null)
            UIPoolObj = UIManager.Instance.topCanvasTrans.gameObject;

        GameObject obj;
        //���û�г��룬��̬����һ�����󲢼�¼�������ĳ�����
        if (!UIPoolDic.ContainsKey(name))
        {
            //ͨ����Դ������ʵ����һ������
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name), UIPoolObj.transform);
            //���Ķ�������
            obj.name = name;
            //��������
            UIPoolDic.Add(obj.name, new PoolData(UIPoolObj, name, obj));
        }
        //����г���
        else
        {
            //��������������� ���� ����������ʹ�õ����峬�����ˣ������ֱ��ȡ������
            if (UIPoolDic[name].Count > 0 || UIPoolDic[name].isOverLimit)
            {
                obj = UIPoolDic[name].Pop();
            }
            //���������û���� ���� ����ʹ�õ�����Ҳû�������ޣ�����Ҫ����
            else
            {
                //ͨ����Դ������ʵ����һ������
                obj = GameObject.Instantiate(Resources.Load<GameObject>(name), UIPoolObj.transform);
                //���Ķ�������
                obj.name = name;
                //��¼��usedList��
                UIPoolDic[name].PushUsedList(obj);
            }
        }
        //���ض���
        return obj;
    }

    /// <summary>
    /// ��������з������
    /// </summary>
    /// <param name="obj">����Ķ���</param>
    public void PushObj(GameObject obj)
    {
        obj.transform.position = Vector2.one * 1000; //�Ƶ���������λ��
        //������ѹ����뵱��
        if (poolDic.ContainsKey(obj.name))
            poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// ��������з���UI����
    /// </summary>
    /// <param name="obj">����Ķ���</param>
    public void PushUIObj(GameObject obj)
    {
        //������ѹ����뵱��
        if (UIPoolDic.ContainsKey(obj.name))
            UIPoolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// ��������е�����
    /// </summary>
    public void ClearPool()
    {
        foreach (PoolData pool in poolDic.Values)
        {
            while (pool.Count > 0)
                GameObject.Destroy(pool.Pop());
            GameObject.Destroy(pool.rootObj); //ɾ��������
        }
        foreach (PoolData pool in UIPoolDic.Values)
        {
            while (pool.Count > 0)
                GameObject.Destroy(pool.Pop());
            GameObject.Destroy(pool.rootObj); //ɾ��������
        }
        poolDic.Clear();
        UIPoolDic.Clear();
        //�������ÿ�
        if (poolObj != null)
        {
            GameObject.Destroy(poolObj);
            poolObj = null;
        }
        //UIPoolObj�����ÿգ���Don'tDestroy����
        //if (UIPoolObj != null)
        //{
        //    GameObject.DestroyImmediate(UIPoolObj);
        //    UIPoolObj = null;
        //}
    }
}
