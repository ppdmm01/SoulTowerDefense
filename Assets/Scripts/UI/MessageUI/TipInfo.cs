using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 提示信息
/// </summary>
public class TipInfo : MonoBehaviour
{
    public RectTransform rectTransform; //提示信息的RectTransform
    public TextMeshProUGUI infoTxt; //文本信息
    private CanvasGroup canvasGroup;
    private float fadeDuration; //淡入淡出时间
    private float displayDuration; //展示时间
    private float height; //上升高度
    private Sequence seq; //动画列表

    /// <summary>
    /// 设置提示信息
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="fadeDuration">淡入淡出时间</param>
    /// <param name="displayDuration">展示时间</param>
    public void Init(string info,float fadeDuration = 0.2f,float displayDuration = 2f,float height = 100f)
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        infoTxt.text = info;
        this.fadeDuration = fadeDuration;
        this.displayDuration = displayDuration;
        this.height = height;
        ShowTip();
    }

    private void ShowTip()
    {
        DOTween.Kill(canvasGroup);
        DOTween.Kill(rectTransform);

        canvasGroup.alpha = 0;
        rectTransform.anchoredPosition = Vector2.zero;
        // 淡入淡出移动效果  
        seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1,fadeDuration));
        seq.Join(rectTransform.DOAnchorPosY(height,fadeDuration));
        seq.AppendInterval(displayDuration);
        seq.Append(canvasGroup.DOFade(0, fadeDuration));
        seq.OnComplete(() =>
        {
            //销毁自己
            UIManager.Instance.DestroyUIObj(this.gameObject);
        });
    }
}
