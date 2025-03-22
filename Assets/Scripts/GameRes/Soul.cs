using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ����Դ���������ռ�
/// </summary>
public class Soul : MonoBehaviour,IPointerEnterHandler
{
    private Sequence seq;
    private int num; //��ö�����Դ
    public float surviveTime; //���ʱ��
    private float timer;

    private bool isCollect; //�Ƿ��ռ���

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

    //���
    public void OnPointerEnter(PointerEventData eventData)
    {
        Collect(); //�ռ����
    }

    private void Collect()
    {
        if (isCollect) return;
        isCollect = true;

        transform.DOKill();
        //��������ԴUI
        Vector3 pos = Vector3.zero;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
        {
            pos = panel.soulUITrans.position;
            pos = Camera.main.ScreenToWorldPoint(pos);
            //��������  
            seq = DOTween.Sequence();
            //��ֱ�ƶ�
            seq.Append(transform.DOMove(pos, 0.6f));
            //������ɺ�����  
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
