using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class MyCanvas : MonoBehaviour
{
    public float width;
    public float height;
    public float nowAspect;
    public float aspect;
    public RectTransform[] rect; //黑边

    void Start()
    {
        aspect = 1920f / 1080f;
        rect = GetComponentsInChildren<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        width = Screen.width;
        height = Screen.height;//获取屏幕宽高信息
        nowAspect = width / height;
        if (nowAspect >= aspect)
        {
            LeftAndRight();
        }
        if (nowAspect < aspect)
        {
            UpAndDown();
        }
    }
    public void LeftAndRight()
    {
        rect[2].anchorMax = new Vector2(0, 0.5f);//锚点
        rect[2].anchorMin = new Vector2(0, 0.5f);//锚点
        rect[2].anchoredPosition = new Vector2(0, 0);//位置
        rect[2].pivot = new Vector2(0f, 0.5f);//轴心
        rect[2].sizeDelta = new Vector2(((nowAspect * rect[0].sizeDelta.y) - rect[0].sizeDelta.y * aspect) / 2, rect[0].sizeDelta.y);

        rect[1].anchorMax = new Vector2(1, 0.5f);
        rect[1].anchorMin = new Vector2(1, 0.5f);
        rect[1].anchoredPosition = new Vector2(0, 0);
        rect[1].pivot = new Vector2(1f, 0.5f);
        rect[1].sizeDelta = new Vector2(((nowAspect * rect[0].sizeDelta.y) - rect[0].sizeDelta.y * aspect) / 2, rect[0].sizeDelta.y);
    }
    public void UpAndDown()
    {
        rect[2].anchorMax = new Vector2(0.5f, 1f);//锚点
        rect[2].anchorMin = new Vector2(0.5f, 1f);//锚点
        rect[2].anchoredPosition = new Vector2(0, 0);//位置
        rect[2].pivot = new Vector2(0.5f, 1f);//轴心
        rect[2].sizeDelta = new Vector2(rect[0].sizeDelta.x, ((rect[0].sizeDelta.x / nowAspect) - rect[0].sizeDelta.x / aspect) / 2);

        rect[1].anchorMax = new Vector2(0.5f, 0);
        rect[1].anchorMin = new Vector2(0.5f, 0);
        rect[1].anchoredPosition = new Vector2(0, 0);
        rect[1].pivot = new Vector2(0.5f, 0);
        rect[1].sizeDelta = new Vector2(rect[0].sizeDelta.x, ((rect[0].sizeDelta.x / nowAspect) - rect[0].sizeDelta.x / aspect) / 2);
    }
}