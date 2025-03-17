using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ���˽ű�
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("��������")]
    public EnemySO data;
    public int nowHp;
    public float speedMultiplier = 1f; //�ƶ�����
    public float damageMultiplier = 1f; //���˱���

    private Vector3 dir; //�����ƶ�����
    private BaseTower target; //���˹�����Ŀ��
    private List<BaseTower> towerList; //��Χ�ڵķ�����
    private float attackTimer;

    //������Ч���
    private SpriteRenderer spriteRenderer;
    private Color oldColor; //��¼ԭ������ɫ
    private Color originColor; //��ʼ��ɫ
    private Color slowColor; //����ʱ����ɫ
    private Color stunColor; //ѣ��ʱ����ɫ

    public CircleCollider2D attackRange; //������Χ

    //ѣ��ͼ��
    public GameObject stunIcon;
    //����ͼ��
    public GameObject slowIcon;

    public bool isDead; //�Ƿ�����

    void Start()
    {
        isDead = false;
        nowHp = data.hp;
        attackTimer = 0;
        target = null;
        towerList = new List<BaseTower>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        oldColor = spriteRenderer.color;
        originColor = spriteRenderer.color;
        slowColor = Color.blue;
        stunColor = Color.gray;

        stunIcon.SetActive(false);
        slowIcon.SetActive(false);
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        CheckTower();

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
            if (TowerManager.Instance.core != null)
            {
                if (speedMultiplier == 0)
                    oldColor = stunColor; //ѣ��ɫ
                else if (speedMultiplier < 1)
                    oldColor = slowColor; //����ɫ
                else
                    oldColor = originColor; //����ɫ

                if (speedMultiplier == 0)
                {
                    slowIcon.SetActive(false);
                    stunIcon.SetActive(true); //ѣ��ͼ��
                }
                else if (speedMultiplier < 1)
                {
                    slowIcon.SetActive(true); //����ͼ��
                    stunIcon.SetActive(false);
                }
                else
                {
                    slowIcon.SetActive(false);
                    stunIcon.SetActive(false);
                }

                //�ƶ�
                dir = (TowerManager.Instance.core.transform.position - transform.position).normalized;
                transform.position += dir * Time.deltaTime * data.moveSpeed * speedMultiplier;
            }
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Wound(int dmg,Color txtColor)
    {
        nowHp -= (int)(dmg * damageMultiplier);
        //�ܻ�����
        UIManager.Instance.ShowTxtPopup(dmg.ToString(), txtColor, transform.position);
        //�ж�����
        if (nowHp <= 0)
        {
            Dead();
            return;
        }
        //����
        Flash(0.1f);
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Dead()
    {
        if (isDead) return;
        isDead = true;
        target = null;
        LevelManager.Instance.SubEnemyNum(); //��������-1
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
        spriteRenderer.color = oldColor;
    }

    private BaseTower FindTarget()
    {
        if (towerList.Count == 0) return null;
        return towerList[0];
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Tower"))
    //    {
    //        BaseTower tower = collision.GetComponent<BaseTower>();
    //        if (tower.data.canBeAttack)
    //        {
    //            if (!tower.isUsed) return;
    //            if (!towerList.Contains(tower))
    //                towerList.Add(tower);
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Tower"))
    //    {
    //        BaseTower tower = collision.GetComponent<BaseTower>();
    //        if (!tower.isUsed) return;
    //        if (towerList.Contains(tower))
    //            towerList.Remove(tower);
    //    }
    //}

    //��������
    public void CheckTower()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange.radius,
            1 << LayerMask.NameToLayer("Tower"));
        towerList.Clear();
        foreach (Collider2D collider in colliders)
        {
            BaseTower tower = collider.GetComponent<BaseTower>();
            if (tower != null && tower.data.canBeAttack)
                towerList.Add(collider.GetComponent<BaseTower>());
        }
        if (!towerList.Any(tower => tower == target))
        {
            target = null;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        target = null;
        oldColor = originColor;
        spriteRenderer.color = oldColor;
        slowIcon.SetActive(false);
        stunIcon.SetActive(false);
        towerList.Clear();

        //�������ϵ�����buff
        Buff[] buffs = GetComponents<Buff>();
        foreach (Buff buff in buffs)
            Destroy(buff);
    }

    private void OnEnable()
    {
        isDead = false;
        nowHp = data.hp;
        attackTimer = 0;
        target = null;
    }
}
