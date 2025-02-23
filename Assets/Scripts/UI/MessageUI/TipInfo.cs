using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ��ʾ��Ϣ
/// </summary>
public class TipInfo : MonoBehaviour
{
    public RectTransform rectTransform; //��ʾ��Ϣ��RectTransform
    public TextMeshProUGUI infoTxt; //�ı���Ϣ
    private CanvasGroup canvasGroup;
    private float fadeDuration; //���뵭��ʱ��
    private float displayDuration; //չʾʱ��
    private float height; //�����߶�
    private Sequence seq; //�����б�

    /// <summary>
    /// ������ʾ��Ϣ
    /// </summary>
    /// <param name="info">��Ϣ</param>
    /// <param name="fadeDuration">���뵭��ʱ��</param>
    /// <param name="displayDuration">չʾʱ��</param>
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
        // ���뵭���ƶ�Ч��  
        seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1,fadeDuration));
        seq.Join(rectTransform.DOAnchorPosY(height,fadeDuration));
        seq.AppendInterval(displayDuration);
        seq.Append(canvasGroup.DOFade(0, fadeDuration));
        seq.OnComplete(() =>
        {
            //�����Լ�
            UIManager.Instance.DestroyUIObj(this.gameObject);
        });
    }
}
