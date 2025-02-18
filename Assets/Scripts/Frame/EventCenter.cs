using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;
using UnityEngine.Events;

/// <summary>
/// 事件信息基类
/// </summary>
public abstract class EventInfoBase { }

/// <summary>
/// 事件信息类（有参）
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
public class EventInfo<T> : EventInfoBase
{
    //观察者们的函数信息
    public UnityAction<T> actions;

    //初始化构造函数
    public EventInfo(UnityAction<T> action)
    {
        this.actions += action;
    }

}

/// <summary>
/// 事件信息类（无参）
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
/// 事件中心模块
/// </summary>
public class EventCenter : Singleton<EventCenter>
{
    private EventCenter() { }

    //用于记录对应事件 关联的 对应逻辑
    private Dictionary<EventType, EventInfoBase> eventDic = new Dictionary<EventType, EventInfoBase>();

    /// <summary>
    /// 触发对应事件（有参）
    /// </summary>
    /// <param name="eventType">事件名</param>
    /// <param name="info">传递的参数</param>
    public void EventTrigger<T>(EventType eventType, T info)
    {
        //存在关注者，才去通知关注者处理相关逻辑
        if (eventDic.ContainsKey(eventType))
        {
            //执行对应逻辑
            (eventDic[eventType] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    /// <summary>
    /// 触发对应事件（无参）
    /// </summary>
    /// <param name="eventType">事件名</param>
    public void EventTrigger(EventType eventType)
    {
        if (eventDic.ContainsKey(eventType))
        {
            //执行对应逻辑
            (eventDic[eventType] as EventInfo).actions?.Invoke();
        }
    }

    /// <summary>
    /// 添加事件监听者（有参）
    /// </summary>
    /// <param name="eventType">事件名</param>
    /// <param name="action">添加的委托</param>
    public void AddEventListener<T>(EventType eventType, UnityAction<T> action)
    {
        //如果已经存在关注的事件，则直接添加关注者的委托即可,否则就新创建一个
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
    /// 添加事件监听者（无参）
    /// </summary>
    /// <param name="eventType">事件名</param>
    /// <param name="action">添加的委托</param>
    public void AddEventListener(EventType eventType, UnityAction action)
    {
        //如果已经存在关注的事件，则直接添加关注者的委托即可,否则就新创建一个
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
    /// 移除事件监听者（有参）
    /// </summary>
    /// <param name="eventType">事件名</param>
    /// <param name="action">移除的委托</param>
    public void RemoveEventListener<T>(EventType eventType, UnityAction<T> action)
    {
        //如果存在关注的事件，才能将关注者移除
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo<T>).actions -= action;
    }

    /// <summary>
    /// 移除事件监听者（无参）
    /// </summary>
    /// <param name="eventType">事件名</param>
    /// <param name="action">移除的委托</param>
    public void RemoveEventListener(EventType eventType, UnityAction action)
    {
        //如果存在关注的事件，才能将关注者移除
        if (eventDic.ContainsKey(eventType))
            (eventDic[eventType] as EventInfo).actions -= action;
    }

    /// <summary>
    /// 清除所有事件
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// 重载：清除指定的事件
    /// </summary>
    /// <param name="eventType">事件名</param>
    public void Clear(EventType eventType)
    {
        if (eventDic.ContainsKey(eventType))
            eventDic.Remove(eventType);
    }
}
