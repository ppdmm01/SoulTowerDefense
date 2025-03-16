using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class BaseTower : MonoBehaviour
{
    [Header("��������")]
    //public TowerSO data; //����������
    [HideInInspector] public TowerData data; //��̬�ķ���������
    protected int nowHp;

    [Header("������")]
    public Transform launcher; //������

    [Header("����ͼƬ������")]
    public List<SpriteRenderer> renderers;

    [Header("��Χ���")]
    public GameObject rangeObj; //��ʾ��Χ
    public CircleCollider2D rangeTrigger; //��Χ�����

    [Header("��������ײ��Χ")]
    public CircleCollider2D towerCollider; //��������ײ��Χ

    [Header("Buffʩ����")]
    public BuffApplier buffApplier; //����������ʩ�ӵ�buff

    protected Transform target; //��׼Ŀ��
    protected float attackTimer; //�����ʱ��
    protected float produceTimer; //������ʱ��
    protected Animator ani; //����
    public HealthBar hpBar; //Ѫ��

    public List<Transform> enemyList; //��¼�ڷ�Χ�ڵĵ���

    [HideInInspector] public bool isUsed; //�Ƿ�����
    protected bool isDead; //�Ƿ�����

    protected virtual void Update()
    {
        if (!isUsed) return;

        attackTimer += Time.deltaTime;
        produceTimer += Time.deltaTime; 

        if (data.isAttacker)
        {
            //Ѱ��Ŀ��
            if (target == null && enemyList.Count > 0)
            {
                attackTimer -= Time.deltaTime;
                target = FindTarget();
            }

            //ת��
            if (target != null)
            {
                Vector2 dir = target.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                launcher.rotation = Quaternion.Slerp(launcher.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * 10);
            }

            //����
            if (target != null && attackTimer > data.interval)
            {
                attackTimer = 0;
                Attack();
            }
        }

        //������Դ
        if (data.isProducer && produceTimer >= data.cooldown)
        {
            produceTimer = 0;
            Produce();
        }

        //����Ѫ��λ��
        if (hpBar != null && hpBar.gameObject.activeSelf)
            SetHpBarPos(transform.position + Vector3.up);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public virtual void Init(TowerData data)
    {
        this.data = data;
        isDead = false;

        enemyList = new List<Transform>();
        ani = GetComponent<Animator>();

        //��ȡ��������buff
        buffApplier = new BuffApplier(data.buffDatas);

        if (data.isAttacker)
        {
            float atkSpeed = TowerManager.Instance.GetTowerSO_ByName(data.towerName).interval / data.interval; //���㹥���ٶ������˶���
            ani.SetFloat("AttackSpeed", atkSpeed); //���ù��������ٶ�
        }

        //���ײ���
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

        //��������Χ
        rangeTrigger.radius = data.range;
        rangeObj.transform.localScale = Vector3.one * (data.range * 2);
        rangeObj.SetActive(false);

        //����Ѫ��
        CreateHpBar();
    }

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Attack()
    {
        if (target == null) return;
        ani.Play("Attack"); //�����¼���������
    }

    /// <summary>
    /// ������Դ
    /// </summary>
    public virtual void Produce()
    {
        FlashSmoothly(1f, Color.yellow, () =>
        {
            UIManager.Instance.ShowTxtPopup(data.output.ToString(), Color.white, transform.position);
            GameResManager.Instance.AddQiNum(data.output);
        });
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //�������Ƿ���뷶Χ
        if (collision.CompareTag("Enemy") && !enemyList.Contains(collision.transform))
        {
            enemyList.Add(collision.transform);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        //�������Ƿ��˳���Χ
        if (collision.CompareTag("Enemy") && enemyList.Contains(collision.transform))
        {
            if (target == collision.transform)
                target = null; //Ŀ������ʧ���ÿ�
            enemyList.Remove(collision.transform);
        }
    }

    /// <summary>
    /// Ѱ��Ŀ�꣨���������ģ�
    /// </summary>
    public virtual Transform FindTarget()
    {
        if (TowerManager.Instance.core == null) return null;

        Vector2 corePos = TowerManager.Instance.core.transform.position;
        Transform targetTrans = null;
        for (int i = enemyList.Count-1;i >=0;i--)
        {
            if (Vector2.Distance(corePos, enemyList[i].position) > 100) //�������ǳ�Զ���������˱����𣨶�������٣�
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

    #region Ѫ�����
    /// <summary>
    /// ����Ѫ��
    /// </summary>
    public virtual void CreateHpBar()
    {
        //����Ѫ��
        GameObject HpBarObj = UIManager.Instance.CreateUIObjByPoolMgr("UI/UIObj/HealthBar");
        HealthBar hpBar = HpBarObj.GetComponent<HealthBar>();
        hpBar.Init(nowHp, data.hp, Color.green,true);
        this.hpBar = hpBar;
        HideHpBar();
    }

    /// <summary>
    /// ��ʾѪ��
    /// </summary>
    public void ShowHpBar()
    {
        if (hpBar != null)
            hpBar.ShowHpBar();
    }

    /// <summary>
    /// ֱ������Ѫ��
    /// </summary>
    public void HideHpBar()
    {
        if (hpBar != null)
            hpBar.HideHpBar();
    }

    /// <summary>
    /// ����Ѫ��
    /// </summary>
    public void UpdateHpBar()
    {
        if (hpBar != null)
            hpBar.UpdateHp(nowHp, data.hp);
    }

    /// <summary>
    /// ����Ѫ��λ��
    /// </summary>
    public virtual void SetHpBarPos(Vector2 pos)
    {
        hpBar.SetPos(pos);
    }
    #endregion

    #region ������Χ���
    /// <summary>
    /// ��ʾ��Χ
    /// </summary>
    public void ShowRange()
    {
        if (!rangeObj.activeSelf)
            rangeObj.SetActive(true);
    }

    /// <summary>
    /// ���ط�Χ
    /// </summary>
    public void HideRange()
    {
        if (rangeObj.activeSelf)
            rangeObj.SetActive(false);
    }

    /// <summary>
    /// ���÷�Χ��ɫ
    /// </summary>
    /// <param name="color"></param>
    public void SetRangeColor(Color color)
    {
        SpriteRenderer renderer = rangeObj.GetComponent<SpriteRenderer>();
        if (renderer.color != color)
            renderer.color = color;
    }
    #endregion

    #region �����������
    /// <summary>
    /// ����
    /// </summary>
    public virtual void Wound(int dmg)
    {
        nowHp -= dmg;
        //����Ѫ��
        ShowHpBar();
        UpdateHpBar();
        //����
        if (nowHp <= 0)
        {
            Dead();
            return;
        }
        //����
        Flash(0.1f,Color.white);
    }

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Dead()
    {
        if (isDead) return;
        isDead = true;
        //ɾ��Ѫ��
        if (hpBar != null)
            UIManager.Instance.DestroyUIObjByPoolMgr(hpBar.gameObject);
        hpBar = null;
        enemyList.Clear();
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        //����������������ļ�¼
        if (TowerManager.Instance.gameTowerList.Contains(this))
            TowerManager.Instance.gameTowerList.Remove(this);
    }

    private void OnEnable()
    {
        isDead = false;
    }
    #endregion

    #region ��˸Ч�����
    /// <summary>
    /// ��˸Ч��
    /// </summary>
    public void Flash(float time,Color color)
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
            StartCoroutine(FlashRoutine(time,color));
    }

    /// <summary>
    /// ƽ������˸Ч��
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
        float halfTime = time / 2; //���뵭��ʱ��

        //����
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

        //����
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
