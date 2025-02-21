using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class BaseTower : MonoBehaviour
{
    [Header("基础数据")]
    public TowerSO data; //防御塔数据
    public int nowHp;

    [Header("发射器")]
    public Transform launcher; //发射器

    [Header("塔防图片精灵们")]
    public List<SpriteRenderer> renderers;

    [Header("范围检测")]
    public GameObject rangeObj; //显示范围
    public CircleCollider2D rangeTrigger; //范围检测器

    [Header("防御塔碰撞范围")]
    public CircleCollider2D towerCollider; //防御塔碰撞范围

    protected Transform target; //瞄准目标
    protected float attackTimer; //发射计时器
    protected float produceTimer; //生产计时器
    protected Animator ani; //动画
    public HealthBar hpBar; //血条

    public List<Transform> enemyList; //记录在范围内的敌人

    [HideInInspector] public bool isUsed; //是否启用

    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Update()
    {
        if (!isUsed) return;

        attackTimer += Time.deltaTime;
        produceTimer += Time.deltaTime; 

        if (data.isAttacker)
        {
            //寻找目标
            if (target == null && enemyList.Count > 0)
            {
                attackTimer -= Time.deltaTime;
                target = FindTarget();
            }

            //转向
            if (target != null)
            {
                Vector2 dir = target.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                launcher.rotation = Quaternion.Slerp(launcher.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * 10);
            }

            //攻击
            if (target != null && attackTimer > data.interval)
            {
                attackTimer = 0;
                Attack();
            }
        }

        //生产资源
        if (data.isProducer && produceTimer >= data.cooldown)
        {
            produceTimer = 0;
            Produce();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        enemyList = new List<Transform>();
        ani = GetComponent<Animator>();

        Material material = Resources.Load<Material>("Material/FlashMaterial");
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.material != material)
                renderer.material = material;
        }

        attackTimer = 0;
        produceTimer = 0;
        nowHp = data.hp;

        target = null;

        rangeTrigger.radius = data.range;
        rangeObj.transform.localScale = Vector3.one * (data.range * 2);
        rangeObj.SetActive(false);

        //创建血条
        CreateHpBar();
    }

    /// <summary>
    /// 攻击
    /// </summary>
    public virtual void Attack()
    {
        if (target == null) return;
        ani.Play("Attack"); //动画事件触发攻击
    }

    /// <summary>
    /// 生产资源
    /// </summary>
    public virtual void Produce()
    {
        FlashSmoothly(1f, Color.yellow, () =>
        {
            UIManager.Instance.ShowTxtPopup(data.output.ToString(), Color.yellow, transform.position);
            GameResManager.Instance.AddQiNum(data.output);
        });
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //检测敌人是否进入范围
        if (collision.CompareTag("Enemy") && !enemyList.Contains(collision.transform))
        {
            enemyList.Add(collision.transform);
        }

        //防御塔放置相关碰撞检测
        if (collision.CompareTag("Tower") && TowerManager.Instance.isPlacing && TowerManager.Instance.target == this)
        {
            TowerManager.Instance.collisonTowerList.Add(collision.gameObject);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        //检测敌人是否退出范围
        if (collision.CompareTag("Enemy") && enemyList.Contains(collision.transform))
        {
            if (target == collision.transform)
                target = null; //目标已消失，置空
            enemyList.Remove(collision.transform);
        }

        //防御塔放置相关碰撞检测
        if (collision.CompareTag("Tower") && TowerManager.Instance.isPlacing && TowerManager.Instance.target == this)
        {
            TowerManager.Instance.collisonTowerList.Remove(collision.gameObject);
        }
    }

    /// <summary>
    /// 寻找目标（离基地最近的）
    /// </summary>
    public virtual Transform FindTarget()
    {
        if (TowerManager.Instance.core == null) return null;

        Vector2 corePos = TowerManager.Instance.core.transform.position;
        Transform targetTrans = null;
        for (int i = enemyList.Count-1;i >=0;i--)
        {
            if (Vector2.Distance(corePos, enemyList[i].position) > 100) //如果距离非常远，则代表敌人被消灭（对象池销毁）
            {
                enemyList.RemoveAt(i);
                continue;
            }

            if (targetTrans == null)
            {
                targetTrans = enemyList[i];
            }
            else
            {
                if (Vector2.Distance(corePos, enemyList[i].position) < Vector2.Distance(corePos, targetTrans.position))
                {
                    targetTrans = enemyList[i];
                }
            }
        }
        return targetTrans;
    }

    #region 血条相关
    /// <summary>
    /// 创建血条
    /// </summary>
    public virtual void CreateHpBar()
    {
        //创建血条
        GameObject HpBarObj = UIManager.Instance.CreateUIObjByPoolMgr("UI/HealthBar/HealthBar");
        HealthBar hpBar = HpBarObj.GetComponent<HealthBar>();
        hpBar.Init(nowHp, data.hp, Color.green,true);
        this.hpBar = hpBar;
        HideHpBar();
    }

    /// <summary>
    /// 显示血条
    /// </summary>
    public void ShowHpBar()
    {
        if (hpBar != null)
            hpBar.ShowHpBar();
    }

    /// <summary>
    /// 直接隐藏血条
    /// </summary>
    public void HideHpBar()
    {
        if (hpBar != null)
            hpBar.HideHpBar();
    }

    /// <summary>
    /// 设置血条位置
    /// </summary>
    public void SetHpBarPos(Vector2 pos)
    {
        hpBar.SetPos(pos);
    }
    #endregion

    #region 攻击范围相关
    /// <summary>
    /// 显示范围
    /// </summary>
    public void ShowRange()
    {
        if (!rangeObj.activeSelf)
            rangeObj.SetActive(true);
    }

    /// <summary>
    /// 隐藏范围
    /// </summary>
    public void HideRange()
    {
        if (rangeObj.activeSelf)
            rangeObj.SetActive(false);
    }

    /// <summary>
    /// 设置范围颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetRangeColor(Color color)
    {
        SpriteRenderer renderer = rangeObj.GetComponent<SpriteRenderer>();
        if (renderer.color != color)
            renderer.color = color;
    }
    #endregion

    #region 受伤死亡相关
    /// <summary>
    /// 受伤
    /// </summary>
    public virtual void Wound(int dmg)
    {
        nowHp -= dmg;
        //更新血条
        ShowHpBar();
        hpBar.UpdateHp(nowHp,data.hp);
        //死亡
        if (nowHp < 0)
        {
            Dead();
        }
        //闪白
        Flash(0.1f,Color.white);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Dead()
    {
        //删除血条
        UIManager.Instance.DestroyUIObjByPoolMgr(hpBar.gameObject);
        hpBar = null;
        enemyList.Clear();
        Destroy(gameObject);
    }
    #endregion

    #region 闪烁效果相关
    /// <summary>
    /// 闪烁效果
    /// </summary>
    public void Flash(float time,Color color)
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
            StartCoroutine(FlashRoutine(time,color));
    }

    /// <summary>
    /// 平滑的闪烁效果
    /// </summary>
    public void FlashSmoothly(float time, Color color, UnityAction callback = null)
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
            StartCoroutine(FlashSmoothlyRoutine(time, color,callback));
    }

    private IEnumerator FlashRoutine(float time, Color color)
    {
        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            spriteRenderer.material.SetFloat("_FlashAmount",1);
            spriteRenderer.material.SetColor("_FlashColor", color);
        }
        yield return new WaitForSeconds(time);
        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            spriteRenderer.material.SetColor("_FlashColor", Color.white);
            spriteRenderer.material.SetFloat("_FlashAmount", 0);
        }
    }

    private IEnumerator FlashSmoothlyRoutine(float time, Color color, UnityAction callback)
    {
        float value = 0;
        float timer = 0;
        float halfTime = time / 2; //淡入淡出时间

        //淡入
        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            spriteRenderer.material.SetFloat("_FlashAmount", 0);
            spriteRenderer.material.SetColor("_FlashColor", color);
        }
        while (timer < halfTime)
        {
            value = Mathf.Lerp(0, 0.5f, timer/halfTime);
            foreach (SpriteRenderer spriteRenderer in renderers)
                spriteRenderer.material.SetFloat("_FlashAmount", value);
            timer += Time.deltaTime;
            yield return null;
        }

        //淡出
        while (timer < time)
        {
            value = Mathf.Lerp(0, 0.5f, (time - timer) / halfTime);
            foreach (SpriteRenderer spriteRenderer in renderers)
                spriteRenderer.material.SetFloat("_FlashAmount", value);
            timer += Time.deltaTime;
            yield return null;
        }
        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            spriteRenderer.material.SetFloat("_FlashAmount", 0);
            spriteRenderer.material.SetColor("_FlashColor", Color.white);
        }

        callback?.Invoke();
    }
    #endregion
}
