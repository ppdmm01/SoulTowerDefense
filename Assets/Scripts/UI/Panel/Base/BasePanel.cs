using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    //��ǰ����ϵ�CanvasGroup���
    private CanvasGroup canvasGroup;
    //�Ƿ���ʾ
    protected bool isShow;
    //��������ִ�е��¼�
    private UnityAction hideCallBack;
    //�����ٶ�
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
                //ִ���¼�
                hideCallBack?.Invoke();
            }
        }
    }

    /// <summary>
    /// ��ʼ������
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// ��ʾ�Լ��ķ���
    /// </summary>
    public virtual void ShowMe()
    {
        canvasGroup.alpha = 0;
        isShow = true;
    }

    /// <summary>
    /// �����Լ��ķ���
    /// </summary>
    /// <param name="action">������Ϻ���ʲô</param>
    public virtual void HideMe(UnityAction action)
    {
        canvasGroup.alpha = 1;
        isShow = false;

        this.hideCallBack = action;
    }
}
