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
    /// ������Ϣ
    /// </summary>
    /// <param name="info">��Ϣ</param>
    /// <param name="callback">ȷ�ϰ�ť����ʱ�Ļص�</param>
    public void SetInfo(string info,UnityAction callback)
    {
        infoTxt.text = info;
        this.sureCallback += callback;
    }

    /// <summary>
    /// ����ȷ�ϰ�ť�Ļص�����
    /// </summary>
    public void AddSureBtnCallBack(UnityAction callback)
    {
        this.sureCallback += callback;
    }

    /// <summary>
    /// ����ȡ����ť�Ļص�����
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
