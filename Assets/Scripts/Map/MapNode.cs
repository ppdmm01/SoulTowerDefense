using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 节点状态
/// </summary>
public enum NodeStates
{
    Locked, //锁住
    Visited, //已访问
    Attainable //可达
}

/// <summary>
/// 地图节点（物体）
/// </summary>
public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public SpriteRenderer sr; //地图节点图片
    public SpriteRenderer visitedCircle; //访问过的圆圈图片
    public Image visitedCircleImage; //访问过的圆圈图片（用于制作动画效果，spriteRenderer没有Filled参数）

    public Node Node { get; private set; }
    public NodeBlueprint Blueprint { get; private set; }

    private float initialScale; //记录初始尺寸
    private const float HoverScaleFactor = 1.2f; //鼠标悬停的缩放系数

    private float mouseDownTime; //鼠标按下的时间
    private const float MaxClickDuration = 0.5f; //鼠标点击的间隔时间（鼠标按下多长时间内视为点击）

    /// <summary>
    /// 设置
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
    /// 设置图标状态
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
                // 图片从未访问颜色到访问颜色来回闪烁
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

    //变大效果
    public void OnPointerEnter(PointerEventData data)
    {
        if (sr != null)
        {
            sr.transform.DOKill();
            sr.transform.DOScale(initialScale * HoverScaleFactor, 0.3f);
        }
    }

    //恢复原来尺寸
    public void OnPointerExit(PointerEventData data)
    {
        if (sr != null)
        {
            sr.transform.DOKill();
            sr.transform.DOScale(initialScale, 0.3f);
        }
    }

    //鼠标按下
    public void OnPointerDown(PointerEventData data)
    {
        mouseDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (Time.time - mouseDownTime < MaxClickDuration)
            MapPlayerTracker.Instance.SelectNode(this); //点击节点事件
    }

    //播放圆圈动画
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
