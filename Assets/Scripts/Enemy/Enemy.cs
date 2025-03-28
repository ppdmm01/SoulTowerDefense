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
    private Color originColor; //初始颜色
    private Color slowColor; //减速时的颜色
    private Color stunColor; //眩晕时的颜色

    public CircleCollider2D attackRange; //攻击范围

    //眩晕图标
    public GameObject stunIcon;
    //减速图标
    public GameObject slowIcon;

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

        originColor = spriteRenderer.color;
        slowColor = Color.blue;
        stunColor = Color.gray;

        nowColor = originColor;

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
            //攻击
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
                    nowColor = stunColor; //眩晕色
                else if (speedMultiplier < 1)
                    nowColor = slowColor; //减速色
                else
                    nowColor = originColor; //正常色

                if (speedMultiplier == 0)
                {
                    slowIcon.SetActive(false);
                    stunIcon.SetActive(true); //眩晕图标
                }
                else if (speedMultiplier < 1)
                {
                    slowIcon.SetActive(true); //减速图标
                    stunIcon.SetActive(false);
                }
                else
                {
                    slowIcon.SetActive(false);
                    stunIcon.SetActive(false);
                }

                //移动
                dir = (TowerManager.Instance.core.transform.position - transform.position).normalized;
                transform.position += dir * Time.deltaTime * data.moveSpeed * speedMultiplier;
            }
        }
    }

    /// <summary>
    /// 受伤
    /// </summary>
    public void Wound(int dmg,Color txtColor)
    {
        nowHp -= Mathf.RoundToInt(dmg * damageMultiplier);
        //受击数字
        UIManager.Instance.ShowTxtPopup(dmg.ToString(), txtColor,36, transform.position);
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
        spriteRenderer.material.SetFloat("_FlashAmount", 0);
    }

    private BaseTower FindTarget()
    {
        if (towerList.Count == 0) return null;
        return towerList[0];
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

        slowIcon.SetActive(false);
        stunIcon.SetActive(false);
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
}
