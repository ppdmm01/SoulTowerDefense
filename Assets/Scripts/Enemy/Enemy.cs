using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人脚本
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("敌人数据")]
    public EnemySO data;
    public int nowHp;

    private Vector3 dir; //敌人移动方向
    private BaseTower target; //敌人攻击的目标
    private List<BaseTower> towerList; //范围内的防御塔
    private float attackTimer;

    //闪白特效相关
    private SpriteRenderer spriteRenderer;
    private Color originColor; //记录原来的颜色

    public bool isDead; //是否死亡

    void Start()
    {
        isDead = false;
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
                //移动
                dir = (TowerManager.Instance.core.transform.position - transform.position).normalized;
                transform.position += dir * Time.deltaTime * data.moveSpeed;
            }
        }
    }

    /// <summary>
    /// 受伤
    /// </summary>
    public void Wound(int dmg)
    {
        nowHp -= dmg;
        //特效
        GameObject effObj = PoolMgr.Instance.GetObj("Effect/BloodEffect");
        effObj.transform.position = transform.position;
        //受击数字
        UIManager.Instance.ShowTxtPopup(dmg.ToString(),Color.red,transform.position);
        //判断死亡
        if (nowHp < 0)
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
        StopAllCoroutines();
        target = null;
        spriteRenderer.color = originColor;
        towerList.Clear();
    }

    private void OnEnable()
    {
        isDead = false;
    }
}
