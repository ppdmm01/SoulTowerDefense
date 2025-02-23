using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public Button closeBtn;
    public Button cancelBtn;
    public Button sureBtn;
    public TextMeshProUGUI infoTxt;

    private UnityAction sureCallback;
    private UnityAction cancelCallback;

    public override void Init()
    {
        closeBtn.onClick.AddListener(CancelDo);
        cancelBtn.onClick.AddListener(CancelDo);
        sureBtn.onClick.AddListener(SureDo);
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="callback">确认按钮按下时的回调</param>
    public void SetInfo(string info,UnityAction callback)
    {
        infoTxt.text = info;
        this.sureCallback += callback;
    }

    /// <summary>
    /// 设置确认按钮的回调函数
    /// </summary>
    public void AddSureBtnCallBack(UnityAction callback)
    {
        this.sureCallback += callback;
    }

    /// <summary>
    /// 设置取消按钮的回调函数
    /// </summary>
    public void AddCancelBtnCallBack(UnityAction callback)
    {
        this.cancelCallback += callback;
    }

    private void SureDo()
    {
        sureCallback?.Invoke();
        UIManager.Instance.HidePanel<TipPanel>();
    }

    private void CancelDo()
    {
        cancelCallback?.Invoke();
        UIManager.Instance?.HidePanel<TipPanel>();
    }
}
