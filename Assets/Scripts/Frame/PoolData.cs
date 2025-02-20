using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// �������ݣ������е����ݣ�
/// </summary>
public class PoolData
{
    //�洢�����еĶ���
    protected Stack<GameObject> dataStack = new Stack<GameObject>();
    //�洢����ʹ���еĶ��󣨼�������Ķ���
    protected List<GameObject> usedList = new List<GameObject>();
    //���������
    protected GameObject rootObj;
    //�����������
    protected int maxNum;

    public PoolData() { }

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
            maxNum = 1000;
            Debug.Log("��Ϊ�������PoolObj�ű����������ö���������ޣ���Ĭ������Ϊ1000");
            return;
        }
        else
        {
            //��¼����
            maxNum = poolObj.maxNum;
        }
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
    public virtual GameObject Pop()
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
        //ɾ�������������ж���
        DOTween.Kill(obj.transform);
        //����������ֹ���
        if (PoolMgr.isOpenLayout)
        {
            //ȡ��ʱ�Ͽ����ӹ�ϵ
            obj.transform.SetParent(rootObj.transform.parent);
        }
        return obj;
    }

    /// <summary>
    /// ������Żص�������
    /// </summary>
    /// <param name="obj">�Żص�����</param>
    public virtual void Push(GameObject obj)
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
