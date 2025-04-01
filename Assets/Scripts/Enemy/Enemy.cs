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
    public SpriteRenderer spriteRenderer;
    private Color nowColor; //��¼��ǰ����ɫ
    public Color originColor; //��ʼ��ɫ
    public Color slowColor; //����ʱ����ɫ
    public Color stunColor; //ѣ��ʱ����ɫ
    public Color burnColor; //����ʱ����ɫ

    public CircleCollider2D attackRange; //������Χ

    public Animator ani;

    //���ͼ��
    public GameObject markIcon;

    public bool isDead; //�Ƿ�����

    void Start()
    {
        isDead = false;
        nowHp = data.hp;
        attackTimer = 0;
        target = null;
        towerList = new List<BaseTower>();

        Material material = Resources.Load<Material>("Material/FlashMaterial");
        if (spriteRenderer.material != material)
            spriteRenderer.material = material;

        nowColor = originColor;

        markIcon.gameObject.SetActive(false);
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
                target.Wound(data.atk,this);
            }
        }
        else
        {
            if (TowerManager.Instance.core != null)
            {
                //�ƶ�
                dir = (TowerManager.Instance.core.transform.position - transform.position).normalized;
                transform.position += dir * Time.deltaTime * data.moveSpeed * speedMultiplier;

                //�ж����ҷ�ת
                if (dir.x >= 0) spriteRenderer.flipX = true;
                else spriteRenderer.flipX = false;
            }
        }

        //���
        if (GetSelfBuffs().Contains(BuffType.Mark))
            markIcon.gameObject.SetActive(true);
        else
            markIcon.gameObject.SetActive(false);

        if (speedMultiplier == 0)
            nowColor = stunColor; //ѣ��ɫ
        else if (speedMultiplier < 1)
            nowColor = slowColor; //����ɫ
        else if (GetSelfBuffs().Contains(BuffType.Burn))
            nowColor = burnColor; //����
        else
            nowColor = originColor; //����ɫ
        spriteRenderer.color = nowColor;

        ani.speed = speedMultiplier;
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Wound(int dmg,Color txtColor)
    {
        int nowDmg = Mathf.RoundToInt(dmg * damageMultiplier);
        nowHp -= nowDmg;
        //�ܻ�����
        UIManager.Instance.ShowTxtPopup(nowDmg.ToString(), txtColor,36, transform.position);
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
        Vector3 soulPos = transform.position;
        GameResManager.Instance.CreateOneSoul(data.soulNum, soulPos);
        EffectManager.Instance.PlayEffect("SmokeEffect", transform.position);
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
        spriteRenderer.material.SetFloat("_FlashAmount", 1);
        spriteRenderer.material.SetColor("_FlashColor", Color.white);
        yield return new WaitForSeconds(time);
        spriteRenderer.material.SetColor("_FlashColor", nowColor);
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
    }

    private BaseTower FindTarget()
    {
        if (towerList.Count == 0) return null;
        foreach (BaseTower target in towerList)
        {
            if (target.isUsed) return target;
        }
        return null;
    }

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

        nowColor = originColor;
        spriteRenderer.color = nowColor;
        spriteRenderer.material.SetColor("_FlashColor", nowColor);
        spriteRenderer.material.SetFloat("_FlashAmount", 0);

        markIcon.gameObject.SetActive(false);
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

    //��ȡ���ϵ�buff
    public List<BuffType> GetSelfBuffs()
    {
        Buff[] buffDatas = GetComponents<Buff>();
        List<BuffType> buffs = new List<BuffType>();
        foreach (Buff buff in buffDatas)
        {
            if (!buffs.Contains(buff.data.buffType) && buff.data.buffType != BuffType.None)
                buffs.Add(buff.data.buffType);
        }
        return buffs;
    }
}
