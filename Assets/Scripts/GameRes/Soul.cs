using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 气资源，点击后会收集
/// </summary>
public class Soul : MonoBehaviour,IPointerEnterHandler
{
    private Sequence seq;
    private int num; //获得多少资源
    public float surviveTime; //存活时间
    private float timer;

    private bool isCollect; //是否收集了

    public void Init(int num)
    {
        this.num = num;
        isCollect = false;
    }

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= surviveTime)
        {
            Collect();
        }
    }

    //鼠标
    public void OnPointerEnter(PointerEventData eventData)
    {
        Collect(); //收集灵魂
    }

    private void Collect()
    {
        if (isCollect) return;
        isCollect = true;

        transform.DOKill();
        //飞向气资源UI
        Vector3 pos = Vector3.zero;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
        {
            pos = panel.soulUITrans.position;
            pos = Camera.main.ScreenToWorldPoint(pos);
            //动画序列  
            seq = DOTween.Sequence();
            //垂直移动
            seq.Append(transform.DOMove(pos, 0.6f));
            //动画完成后销毁  
            seq.OnComplete(() =>
            {
                if (this == null || gameObject == null) return;
                GameResManager.Instance.AddSoulNum(num);
                PoolMgr.Instance.PushObj(gameObject);
            });
        }
    }

    private void OnDestroy()
    {
        seq?.Kill();
        transform.DOKill();
        timer = 0;
        isCollect = false;
    }

    private void OnDisable()
    {
        seq?.Kill();
        transform.DOKill();
        timer = 0;
        isCollect = false;  
    }
}
