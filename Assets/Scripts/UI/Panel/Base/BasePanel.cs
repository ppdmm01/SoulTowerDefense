using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    //当前面板上的CanvasGroup组件
    private CanvasGroup canvasGroup;
    //是否显示
    protected bool isShow;
    //隐藏面板后执行的事件
    private UnityAction hideCallBack;
    //显隐速度
    public float alphaSpeed = 5f;

    protected void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
    }

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Update()
    {
        if (canvasGroup.alpha < 1 && isShow)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }

        if (canvasGroup.alpha > 0 && !isShow)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //执行事件
                hideCallBack?.Invoke();
            }
        }
    }

    /// <summary>
    /// 初始化方法
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// 显示自己的方法
    /// </summary>
    public virtual void ShowMe()
    {
        canvasGroup.alpha = 0;
        isShow = true;
    }

    /// <summary>
    /// 隐藏自己的方法
    /// </summary>
    /// <param name="action">隐藏完毕后做什么</param>
    public virtual void HideMe(UnityAction action)
    {
        canvasGroup.alpha = 1;
        isShow = false;

        this.hideCallBack = action;
    }
}
