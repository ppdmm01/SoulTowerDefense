using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �ڵ�״̬
/// </summary>
public enum NodeStates
{
    Locked, //��ס
    Visited, //�ѷ���
    Attainable //�ɴ�
}

/// <summary>
/// ��ͼ�ڵ㣨���壩
/// </summary>
public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public SpriteRenderer sr; //��ͼ�ڵ�ͼƬ
    public SpriteRenderer visitedCircle; //���ʹ���ԲȦͼƬ
    public Image visitedCircleImage; //���ʹ���ԲȦͼƬ��������������Ч����spriteRendererû��Filled������

    public Node Node { get; private set; }
    public NodeBlueprint Blueprint { get; private set; }

    private float initialScale; //��¼��ʼ�ߴ�
    private const float HoverScaleFactor = 1.2f; //�����ͣ������ϵ��

    private float mouseDownTime; //��갴�µ�ʱ��
    private const float MaxClickDuration = 0.5f; //������ļ��ʱ�䣨��갴�¶೤ʱ������Ϊ�����

    /// <summary>
    /// ����
    /// </summary>
    public void SetUp(Node node, NodeBlueprint blueprint)
    {
        Node = node;
        Blueprint = blueprint;
        if (sr != null) sr.sprite = blueprint.icon;
        if (node.nodeType == NodeType.Boss) transform.localScale *= 1.5f;
        if (sr != null) initialScale = sr.transform.localScale.x;

        if (visitedCircle != null)
        {
            visitedCircle.color = MapView.Instance.visitedColor;
            visitedCircle.gameObject.SetActive(false);
        }

        SetState(NodeStates.Locked);
    }

    /// <summary>
    /// ����ͼ��״̬
    /// </summary>
    public void SetState(NodeStates state)
    {
        if (visitedCircle != null) visitedCircle.gameObject.SetActive(false);

        switch (state)
        {
            case NodeStates.Locked:
                if (sr != null)
                {
                    sr.DOKill();
                    sr.color = MapView.Instance.lockedColor;
                }

                break;
            case NodeStates.Visited:
                if (sr != null)
                {
                    sr.DOKill();
                    sr.color = MapView.Instance.visitedColor;
                }

                if (visitedCircle != null) visitedCircle.gameObject.SetActive(true);
                break;
            case NodeStates.Attainable:
                // ͼƬ��δ������ɫ��������ɫ������˸
                if (sr != null)
                {
                    sr.color = MapView.Instance.lockedColor;
                    sr.DOKill();
                    sr.DOColor(MapView.Instance.visitedColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    //���Ч��
    public void OnPointerEnter(PointerEventData data)
    {
        if (sr != null)
        {
            sr.transform.DOKill();
            sr.transform.DOScale(initialScale * HoverScaleFactor, 0.3f);
        }
    }

    //�ָ�ԭ���ߴ�
    public void OnPointerExit(PointerEventData data)
    {
        if (sr != null)
        {
            sr.transform.DOKill();
            sr.transform.DOScale(initialScale, 0.3f);
        }
    }

    //��갴��
    public void OnPointerDown(PointerEventData data)
    {
        mouseDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (Time.time - mouseDownTime < MaxClickDuration)
            MapPlayerTracker.Instance.SelectNode(this); //����ڵ��¼�
    }

    //����ԲȦ����
    public void ShowSwirlAnimation()
    {
        if (visitedCircleImage == null)
            return;

        const float fillDuration = 0.2f;
        visitedCircleImage.fillAmount = 0;

        DOTween.To(() => visitedCircleImage.fillAmount, x => visitedCircleImage.fillAmount = x, 1f, fillDuration);
    }

    private void OnDestroy()
    {
        if (sr != null)
        {
            sr.transform.DOKill();
            sr.DOKill();
        }
    }
}
