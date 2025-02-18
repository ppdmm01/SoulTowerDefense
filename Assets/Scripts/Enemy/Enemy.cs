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
    private int nowHp;

    private Vector3 dir; //敌人移动方向

    //闪白特效相关
    private SpriteRenderer spriteRenderer;
    private Color originColor; //记录原来的颜色

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
        }
        //闪白
        Flash(0.1f);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void Dead()
    {
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

    private void OnDisable()
    {
        spriteRenderer.color = originColor;
    }
}
