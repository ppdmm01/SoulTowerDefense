using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 文本弹跳效果
/// </summary>
public class UIPopup : MonoBehaviour
{
    private TextMeshProUGUI digitalText;
    private Sequence seq;
    [SerializeField] private float moveHeight = 30f; //移动高度  
    [SerializeField] private float duration = 0.8f; //持续时间
    [SerializeField] private float xOffset = 5f; //水平偏移

    public void Init(string txt,Color color, Vector2 pos)
    {
        digitalText = GetComponent<TextMeshProUGUI>();

        Camera mainCamera = Camera.main;

        //世界坐标转屏幕坐标  
        Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);
        transform.position = screenPos + Vector2.right*Random.value*xOffset;
        transform.localScale = Vector3.one;

        // 设置数值和初始状态  
        digitalText.text = txt;
        digitalText.color = color;
        digitalText.alpha = 1f;
        
        //动画序列  
        seq = DOTween.Sequence();
        //垂直移动
        seq.Append(transform.DOMoveY(screenPos.y + moveHeight, 0.4f).SetEase(Ease.OutQuad));
        //渐隐效果  
        seq.Join(digitalText.DOFade(0, duration * 0.2f).SetDelay(duration * 0.5f));
        //缩放动画  
        seq.Join(transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 2));
        //动画完成后销毁  
        seq.OnComplete(() => 
        {
            UIManager.Instance.DestroyUIObjByPoolMgr(gameObject);
        });

    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
