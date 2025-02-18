using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���˽ű�
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("��������")]
    public EnemySO data;
    private int nowHp;

    private Vector3 dir; //�����ƶ�����

    //������Ч���
    private SpriteRenderer spriteRenderer;
    private Color originColor; //��¼ԭ������ɫ

    void Start()
    {
        nowHp = data.hp;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
    }

    void Update()
    {
        dir = (TowerManager.Instance.core.transform.position - transform.position).normalized;
        transform.position += dir * Time.deltaTime * data.moveSpeed;
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Wound(int dmg)
    {
        nowHp -= dmg;
        //��Ч
        GameObject effObj = PoolMgr.Instance.GetObj("Effect/BloodEffect");
        effObj.transform.position = transform.position;
        //�ܻ�����
        UIManager.Instance.ShowTxtPopup(dmg.ToString(),Color.red,transform.position);
        //�ж�����
        if (nowHp < 0)
        {
            Dead();
        }
        //����
        Flash(0.1f);
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Dead()
    {
        PoolMgr.Instance.PushObj(gameObject);
    }

    /// <summary>
    /// ���ף�Ŀǰ����ɫ�����滻��Shader��
    /// </summary>
    public void Flash(float time)
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
            StartCoroutine(FlashRoutine(time));
    }

    private IEnumerator FlashRoutine(float time)
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(time);
        spriteRenderer.color = originColor;
    }

    private void OnDisable()
    {
        spriteRenderer.color = originColor;
    }
}
