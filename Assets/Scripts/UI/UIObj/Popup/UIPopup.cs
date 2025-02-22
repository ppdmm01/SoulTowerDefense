using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// �ı�����Ч��
/// </summary>
public class UIPopup : MonoBehaviour
{
    private TextMeshProUGUI digitalText;
    private Sequence seq;
    [SerializeField] private float moveHeight = 50f; //�ƶ��߶�  
    [SerializeField] private float duration = 0.8f; //����ʱ��
    [SerializeField] private float maxOffset = 30f; //ˮƽƫ��

    public void Init(string txt,Color color, Vector2 pos)
    {
        digitalText = GetComponent<TextMeshProUGUI>();

        Camera mainCamera = Camera.main;

        //��������ת��Ļ����  
        Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);
        transform.position = screenPos;
        transform.localScale = Vector3.one;

        // ������ֵ�ͳ�ʼ״̬  
        digitalText.text = txt;
        digitalText.color = color;
        digitalText.alpha = 1f;

        //���ˮƽƫ��  
        float offsetX = Random.Range(-maxOffset, maxOffset);
        
        //��������  
        seq = DOTween.Sequence();
        //��ֱ�ƶ�
        seq.Append(transform.DOMoveY(screenPos.y + moveHeight, duration).SetEase(Ease.OutQuad));
        //ˮƽƫ��  
        seq.Join(transform.DOMoveX(screenPos.x + offsetX, duration));
        //����Ч��  
        seq.Join(digitalText.DOFade(0, duration * 0.5f).SetDelay(duration * 0.5f));
        //���Ŷ���  
        seq.Join(transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 2));
        //������ɺ�����  
        seq.OnComplete(() => 
        {
            UIManager.Instance.DestroyUIObjByPoolMgr(gameObject);
        });

    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}
