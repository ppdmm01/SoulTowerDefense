using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 触碰变大效果
/// </summary>
public class ScaleEff : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scale = 1.2f;
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one* scale, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.2f);
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
