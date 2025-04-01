using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 敌人脚本
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("敌人数据")]
    public EnemySO data;
    public int nowHp;
    public float speedMultiplier = 1f; //移动倍率
    public float damageMultiplier = 1f; //受伤倍率

    private Vector3 dir; //敌人移动方向
    private BaseTower target; //敌人攻击的目标
    private List<BaseTower> towerList; //范围内的防御塔
    private float attackTimer;

    //闪白特效相关
    public SpriteRenderer spriteRenderer;
    private Color nowColor; //记录当前的颜色
    public Color originColor; //初始颜色
    public Color slowColor; //减速时的颜色
    public Color stunColor; //眩晕时的颜色
    public Color burnColor; //灼烧时的颜色

    public CircleCollider2D attackRange; //攻击范围

    public Animator ani;

    //标记图标
    public GameObject markIcon;

    public bool isDead; //是否死亡

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
            //攻击
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
                //移动
                dir = (TowerManager.Instance.core.transform.position - transform.position).normalized;
                transform.position += dir * Time.deltaTime * data.moveSpeed * speedMultiplier;

                //判断左右翻转
                if (dir.x >= 0) spriteRenderer.flipX = true;
                else spriteRenderer.flipX = false;
            }
        }

        //标记
        if (GetSelfBuffs().Contains(BuffType.Mark))
            markIcon.gameObject.SetActive(true);
        else
            markIcon.gameObject.SetActive(false);

        if (speedMultiplier == 0)
            nowColor = stunColor; //眩晕色
        else if (speedMultiplier < 1)
            nowColor = slowColor; //减速色
        else if (GetSelfBuffs().Contains(BuffType.Burn))
            nowColor = burnColor; //火焰
        else
            nowColor = originColor; //正常色
        spriteRenderer.color = nowColor;

        ani.speed = speedMultiplier;
    }

    /// <summary>
    /// 受伤
    /// </summary>
    public void Wound(int dmg,Color txtColor)
    {
        int nowDmg = Mathf.RoundToInt(dmg * damageMultiplier);
        nowHp -= nowDmg;
        //受击数字
        UIManager.Instance.ShowTxtPopup(nowDmg.ToString(), txtColor,36, transform.position);
        //判断死亡
        if (nowHp <= 0)
        {
            Dead();
            return;
        }
        //闪白
        Flash(0.1f);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void Dead()
    {
        if (isDead) return;
        isDead = true;
        target = null;
        Vector3 soulPos = transform.position;
        GameResManager.Instance.CreateOneSoul(data.soulNum, soulPos);
        EffectManager.Instance.PlayEffect("SmokeEffect", transform.position);
        LevelManager.Instance.SubEnemyNum(); //怪物数量-1
        PoolMgr.Instance.PushObj(gameObject);

    }

    /// <summary>
    /// 闪白（目前用颜色，后面换成Shader）
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

    //检测防御塔
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

        //清理身上的所有buff
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

    //获取身上的buff
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
