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
    private BaseTower target; //���˹�����Ŀ��
    private List<BaseTower> towerList; //��Χ�ڵķ�����
    private bool isAttack; //�Ƿ����ڹ���
    private float attackTimer;

    //������Ч���
    private SpriteRenderer spriteRenderer;
    private Color originColor; //��¼ԭ������ɫ

    void Start()
    {
        nowHp = data.hp;
        attackTimer = 0;
        target = null;
        towerList = new List<BaseTower>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (target == null)
        {
            attackTimer -= Time.deltaTime;
            target = FindTarget();
        }

        if (target != null)
        {
            //����
            if (attackTimer >= data.interval)
            {
                attackTimer = 0;
                target.Wound(data.atk);
            }
        }
        else
        {
            //�ƶ�
            dir = (TowerManager.Instance.core.transform.position - transform.position).normalized;
            transform.position += dir * Time.deltaTime * data.moveSpeed;
        }
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

    private BaseTower FindTarget()
    {
        if (towerList.Count == 0) return null;
        return towerList[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            BaseTower tower = collision.GetComponent<BaseTower>();
            if (!tower.isUsed) return;
            if (!towerList.Contains(tower))
                towerList.Add(tower);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            BaseTower tower = collision.GetComponent<BaseTower>();
            if (!tower.isUsed) return;
            if (towerList.Contains(tower))
                towerList.Remove(tower);
        }
    }

    private void OnDisable()
    {
        spriteRenderer.color = originColor;
        towerList.Clear();
    }
}
