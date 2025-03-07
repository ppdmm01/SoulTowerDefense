using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷�UI������Թ�������ͼ��ק��
/// </summary>
public class ScrollNonUI : MonoBehaviour
{
    public float tweenBackDuration = 0.3f; //�ص�ʱ��
    public Ease tweenBackEase; //ָ���ص�����Ч��
    public bool freezeX; //����X��
    public FloatMinMax xConstraints = new FloatMinMax(); //x�᷶Χ
    public bool freezeY; //����Y��
    public FloatMinMax yConstraints = new FloatMinMax(); //y�᷶Χ
    private Vector2 offset;
    // distance from the center of this Game Object to the point where we clicked to start dragging 
    private Vector3 pointerDisplacement;
    private float zDisplacement; //��¼������Z������
    private bool dragging;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        zDisplacement = -mainCamera.transform.position.z + transform.position.z;
    }

    public void OnMouseDown()
    {
        pointerDisplacement = -transform.position + MouseInWorldCoords(); //�������ĵ�ָ�����������
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

        //�ö����������ƶ�
        Vector3 mousePos = MouseInWorldCoords();
        transform.position = new Vector3(
            freezeX ? transform.position.x : mousePos.x - pointerDisplacement.x, //����ƽ�������ĳ��ȼ������λ��
            freezeY ? transform.position.y : mousePos.y - pointerDisplacement.y,
            transform.position.z);
    }

    // ��ȡ��������������
    private Vector3 MouseInWorldCoords()
    {
        Vector3 screenMousePos = Input.mousePosition;
        //Debug.Log(screenMousePos);
        screenMousePos.z = zDisplacement;
        return mainCamera.ScreenToWorldPoint(screenMousePos);
    }

    /// <summary>
    /// ��λ�ûص������Ʒ�Χ��
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
