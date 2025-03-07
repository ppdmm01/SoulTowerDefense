using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 让非UI物体可以滚动（地图拖拽）
/// </summary>
public class ScrollNonUI : MonoBehaviour
{
    public float tweenBackDuration = 0.3f; //回弹时间
    public Ease tweenBackEase; //指定回弹缓动效果
    public bool freezeX; //限制X轴
    public FloatMinMax xConstraints = new FloatMinMax(); //x轴范围
    public bool freezeY; //限制Y轴
    public FloatMinMax yConstraints = new FloatMinMax(); //y轴范围
    private Vector2 offset;
    // distance from the center of this Game Object to the point where we clicked to start dragging 
    private Vector3 pointerDisplacement;
    private float zDisplacement; //记录对象在Z轴的深度
    private bool dragging;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        zDisplacement = -mainCamera.transform.position.z + transform.position.z;
    }

    public void OnMouseDown()
    {
        pointerDisplacement = -transform.position + MouseInWorldCoords(); //对象中心点指向鼠标点的向量
        transform.DOKill();
        dragging = true;
    }

    public void OnMouseUp()
    {
        dragging = false;
        TweenBack();
    }

    private void Update()
    {
        if (!dragging) return;

        //让对象跟随鼠标移动
        Vector3 mousePos = MouseInWorldCoords();
        transform.position = new Vector3(
            freezeX ? transform.position.x : mousePos.x - pointerDisplacement.x, //鼠标点平移向量的长度即对象的位置
            freezeY ? transform.position.y : mousePos.y - pointerDisplacement.y,
            transform.position.z);
    }

    // 获取鼠标在世界的坐标
    private Vector3 MouseInWorldCoords()
    {
        Vector3 screenMousePos = Input.mousePosition;
        //Debug.Log(screenMousePos);
        screenMousePos.z = zDisplacement;
        return mainCamera.ScreenToWorldPoint(screenMousePos);
    }

    /// <summary>
    /// 将位置回弹到限制范围内
    /// </summary>
    private void TweenBack()
    {
        if (freezeY)
        {
            if (transform.localPosition.x >= xConstraints.min && transform.localPosition.x <= xConstraints.max)
                return;

            float targetX = transform.localPosition.x < xConstraints.min ? xConstraints.min : xConstraints.max;
            transform.DOLocalMoveX(targetX, tweenBackDuration).SetEase(tweenBackEase);
        }
        else if (freezeX)
        {
            if (transform.localPosition.y >= yConstraints.min && transform.localPosition.y <= yConstraints.max)
                return;

            float targetY = transform.localPosition.y < yConstraints.min ? yConstraints.min : yConstraints.max;
            transform.DOLocalMoveY(targetY, tweenBackDuration).SetEase(tweenBackEase);
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
