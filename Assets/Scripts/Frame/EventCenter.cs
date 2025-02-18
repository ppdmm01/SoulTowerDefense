using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;
using UnityEngine.Events;

/// <summary>
/// �¼���Ϣ����
/// </summary>
public abstract class EventInfoBase { }

/// <summary>
/// �¼���Ϣ�ࣨ�вΣ�
/// </summary>
/// <typeparam name="T">��������</typeparam>
public class EventInfo<T> : EventInfoBase
{
    //�۲����ǵĺ�����Ϣ
    public UnityAction<T> actions;

    //��ʼ�����캯��
    public EventInfo(UnityAction<T> action)
    {
        this.actions += action;
    }

}

/// <summary>
/// �¼���Ϣ�ࣨ�޲Σ�
/// </summary>
public class EventInfo : EventInfoBase
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        this.actions += action;
    }
}

/// <summary>
/// �¼�����ģ��
/// </summary>
public class EventCenter : Singleton<EventCenter>
{
    private EventCenter() { }

    //���ڼ�¼��Ӧ�¼� ������ ��Ӧ�߼�
    private Dictionary<EventType, EventInfoBase> eventDic = new Dictionary<EventType, EventInfoBase>();

    /// <summary>
    /// ������Ӧ�¼����вΣ�
    /// </summary>
    /// <param name="eventType">�¼���</param>
    /// <param name="info">���ݵĲ���</param>
    public void EventTrigger<T>(EventType eventType, T info)
    {
        //���ڹ�ע�ߣ���ȥ֪ͨ��ע�ߴ�������߼�
        if (eventDic.ContainsKey(eventType))
        {
            //ִ�ж�Ӧ�߼�
            (eventDic[eventType] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    /// <summary>
    /// ������Ӧ�¼����޲Σ�
    /// </summary>
    /// <param name="eventType">�¼���</param>
    public void EventTrigger(EventType eventType)
    {
        if (eventDic.ContainsKey(eventType))
        {
            //ִ�ж�Ӧ�߼�
            (eventDic[eventType] as EventInfo).actions?.Invoke();
        }
    }

    /// <summary>
    /// ����¼������ߣ��вΣ�
    /// </summary>
    /// <param name="eventType">�¼���</param>
    /// <param name="action">��ӵ�ί��</param>
    public void AddEventListener<T>(EventType eventType, UnityAction<T> action)
    {
        //����Ѿ����ڹ�ע���¼�����ֱ����ӹ�ע�ߵ�ί�м���,������´���һ��
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(eventType, new EventInfo<T>(action));
        }
    }

    /// <summary>
    /// ����¼������ߣ��޲Σ�
    /// </summary>
    /// <param name="eventType">�¼���</param>
    /// <param name="action">��ӵ�ί��</param>
    public void AddEventListener(EventType eventType, UnityAction action)
    {
        //����Ѿ����ڹ�ע���¼�����ֱ����ӹ�ע�ߵ�ί�м���,������´���һ��
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(eventType, new EventInfo(action));
        }
    }

    /// <summary>
    /// �Ƴ��¼������ߣ��вΣ�
    /// </summary>
    /// <param name="eventType">�¼���</param>
    /// <param name="action">�Ƴ���ί��</param>
    public void RemoveEventListener<T>(EventType eventType, UnityAction<T> action)
    {
        //������ڹ�ע���¼������ܽ���ע���Ƴ�
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo<T>).actions -= action;
    }

    /// <summary>
    /// �Ƴ��¼������ߣ��޲Σ�
    /// </summary>
    /// <param name="eventType">�¼���</param>
    /// <param name="action">�Ƴ���ί��</param>
    public void RemoveEventListener(EventType eventType, UnityAction action)
    {
        //������ڹ�ע���¼������ܽ���ע���Ƴ�
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo).actions -= action;
    }

    /// <summary>
    /// ��������¼�
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// ���أ����ָ�����¼�
    /// </summary>
    /// <param name="eventType">�¼���</param>
    public void Clear(EventType eventType)
    {
        if (eventDic.ContainsKey(eventType))
            eventDic.Remove(eventType);
    }
}
