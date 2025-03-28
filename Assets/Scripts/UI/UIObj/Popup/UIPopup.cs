using DG.Tweening;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// �ı�����Ч��
/// </summary>
public class UIPopup : MonoBehaviour
{
    private TextMeshProUGUI digitalText;
    private Sequence seq;
    [SerializeField] private float moveHeight = 30f; //�ƶ��߶�  
    [SerializeField] private float duration = 0.8f; //����ʱ��
    [SerializeField] private float xOffset = 5f; //ˮƽƫ��

    public void Init(string txt,Color color,float size, Vector2 worldPos)
    {
        digitalText = GetComponent<TextMeshProUGUI>();

        Camera mainCamera = Camera.main;

        //��������ת��Ļ����  
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        transform.position = screenPos + Vector2.right*Random.value*xOffset;
        transform.localScale = Vector3.one;

        // ������ֵ�ͳ�ʼ״̬  
        digitalText.text = txt;
        digitalText.fontSize = size;
        digitalText.color = color;
        digitalText.alpha = 1f;

        DOAnimate(screenPos);
    }

    public void InitUI(string txt, Color color, float size, Vector2 uiPos)
    {
        digitalText = GetComponent<TextMeshProUGUI>();

        //��������ת��Ļ����  
        transform.position = uiPos + Vector2.right * Random.value * xOffset;
        transform.localScale = Vector3.one;

        // ������ֵ�ͳ�ʼ״̬  
        digitalText.text = txt;
        digitalText.fontSize = size;
        digitalText.color = color;
        digitalText.alpha = 1f;

        DOAnimate(uiPos);
    }

    public void DOAnimate(Vector2 screenPos)
    {
        //��������  
        seq = DOTween.Sequence();
        //��ֱ�ƶ�
        seq.Append(transform.DOMoveY(screenPos.y + moveHeight, 0.4f).SetEase(Ease.OutQuad));
        //����Ч��  
        seq.Join(digitalText.DOFade(0, duration * 0.2f).SetDelay(duration * 0.5f));
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

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
