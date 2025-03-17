using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������ǵ�����
/// </summary>
public class StarPoint : MonoBehaviour
{
    [Header("��������")]
    public GameObject starShow;
    public GameObject starHide;

    public Image showIcon;
    public Image hideIcon;

    [Header("����ͼƬ")]
    public Sprite starShowIcon;
    public Sprite starHideIcon;
    public Sprite fireShowIcon;
    public Sprite fireHideIcon;

    public void SetStarActive(bool active,DetectionPoint.PointType type)
    {
        switch (type)
        {
            case DetectionPoint.PointType.Star:
                showIcon.sprite = starShowIcon;
                hideIcon.sprite = starHideIcon;
                break;
            case DetectionPoint.PointType.Fire:
                showIcon.sprite = fireShowIcon;
                hideIcon.sprite = fireHideIcon;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (active)
        {
            hideIcon.gameObject.SetActive(false);
            showIcon.gameObject.SetActive(true);
        }
        else
        {
            hideIcon.gameObject.SetActive(true);
            showIcon.gameObject.SetActive(false);
        }
    }
}
