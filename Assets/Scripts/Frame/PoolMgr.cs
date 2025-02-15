using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������ݣ������е����ݣ�
/// </summary>
public class PoolData
{
    //�洢�����еĶ���
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //�洢����ʹ���еĶ��󣨼�������Ķ���
    private List<GameObject> usedList = new List<GameObject>();
    //���������
    private GameObject rootObj;
    //�����������
    private int maxNum;

    /// <summary>
    /// ��ʼ�����캯�����������벢�Ž������У�ͬʱ��������ӽ�ʹ���б���
    /// </summary>
    /// <param name="root">����أ����ӣ�������</param>
    /// <param name="name">������</param>
    /// <param name="usedObj">ʹ�õĶ���</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        //����������ֹ���
        if (PoolMgr.isOpenLayout)
        {
            //�������������
            rootObj = new GameObject(name);
            //��������ڹ��Ӹ������£��������ӹ�ϵ
            rootObj.transform.SetParent(root.transform);
        }
        //��ʹ�õĶ�����ӽ��б��У���������ʱ�϶��ᴴ��һ������
        PushUsedList(usedObj);

        //��ȡ�����������
        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            Debug.LogError("��Ϊ�������PoolObj�ű����������ö����������");
            return;
        }
        //��¼����
        maxNum = poolObj.maxNum;
    }

    //�����ж�������
    public int Count => dataStack.Count;

    //ʹ���еĶ��������
    public int UsedCount => usedList.Count;

    //�����Ƿ񳬹��������
    public bool isOverLimit => usedList.Count >= maxNum;

    /// <summary>
    /// ���������е����ݶ���
    /// </summary>
    /// <returns>��Ӧ�����ݶ���</returns>
    public GameObject Pop()
    {
        //��ȡ����
        GameObject obj;
        if (Count > 0)
        {
            //��������ж����ֱ��ȡ
            obj = dataStack.Pop();
            //��ӽ�usedList��
            usedList.Add(obj);
        }
        else
        {
            //�������usedList��ֱ����ʹ����õ���Դ����
            //��0����������ʹ��ʱ����õ�
            obj = usedList[0];
            //��ʹ�õ�����ŵ����һ������,��ʾ���µ����壨���Ƴ�����ӽ�ȥ��
            usedList.RemoveAt(0);
            usedList.Add(obj);
        }
        //�����󼤻�
        obj.SetActive(true);
        //����������ֹ���
        if (PoolMgr.isOpenLayout)
        {
            //ȡ��ʱ�Ͽ����ӹ�ϵ
            obj.transform.SetParent(null);
        }
        return obj;
    }

    /// <summary>
    /// ������Żص�������
    /// </summary>
    /// <param name="obj">�Żص�����</param>
    public void Push(GameObject obj)
    {
        //��Ӷ��󵽳�����
        dataStack.Push(obj);
        //������ʧ��
        obj.SetActive(false);
        //����������ֹ���
        if (PoolMgr.isOpenLayout)
        {
            //�ŵ������������
            obj.transform.SetParent(rootObj.transform);
        }
        //�������usedList���Ƴ�
        usedList.Remove(obj);
    }

    /// <summary>
    /// ��ʹ���еĶ����б�����Ӷ���
    /// </summary>
    /// <param name="obj">��ӵĶ���</param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/// <summary>
/// ����(����)��ģ�������
/// </summary>
public class PoolMgr : Singleton<PoolMgr>
{
    private PoolMgr() { }

    //�����������г�������������
    Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    //���ӣ�����ع�������������
    private GameObject poolObj;
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
        //������Ӹ�����Ϊ�ղ��ҿ������ֹ��ܣ��򴴽�������
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;
        //���û�г��룬��̬����һ�����󲢼�¼�������ĳ�����
        if (!poolDic.ContainsKey(name))
        {
            //ͨ����Դ������ʵ����һ������
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
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
                obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
                //���Ķ�������
                obj.name = name;
                //��¼��usedList��
                poolDic[name].PushUsedList(obj);
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
        //������ѹ����뵱��
        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// ��������е�����
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        //�������ÿ�
        poolObj = null;
    }
}
