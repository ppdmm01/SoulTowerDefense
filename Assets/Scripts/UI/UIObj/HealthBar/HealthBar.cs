using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image hpImg; //血条
    public Image effImg; //缓冲条

    public float buffTime = 0.5f; //血条白条缓动时间

    public float showTime = 2; //血条显示时间
    private float timer;
    private bool isAutoHide; //是否自动隐藏

    private Coroutine updateCoroutine;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="nowHp">当前血量</param>
    /// <param name="maxHp">最大血量</param>
    /// <param name="color">颜色</param>
    /// <param name="worldPos">目标世界坐标位置</param>
    /// <param name="isAutoHide">是否自动隐藏</param>
    public void Init(int nowHp, int maxHp, Color color,bool isAutoHide = false)
    {
        hpImg.fillAmount = (float)nowHp / (float)maxHp;
        SetColor(color);
        this.isAutoHide = isAutoHide;
        timer = 0;
    }

    private void Update()
    {
        if (!isAutoHide) return;
        //隔一段时间自动隐藏血条
        if (gameObject.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer > showTime)
            {
                timer = 0;
                HideHpBar();
            }
        }
        else
        {
            timer = 0;
        }
    }

    /// <summary>
    /// 更新血量
    /// </summary>
    /// <param name="nowHp">当前血量</param>
    /// <param name="maxHp">最大血量</param>
    public void UpdateHp(int nowHp, int maxHp)
    {
        hpImg.fillAmount = (float)nowHp / (float)maxHp;
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        updateCoroutine = StartCoroutine(UpdateHpEffect());
    }

    private IEnumerator UpdateHpEffect()
    {
        float effectLength = effImg.fillAmount - hpImg.fillAmount; //白条需要改变的长度
        float timer = 0f; //每一段减少的时间
        while (timer < buffTime && effectLength != 0)
        {
            timer += Time.deltaTime;
            effImg.fillAmount = Mathf.Lerp(hpImg.fillAmount + effectLength, hpImg.fillAmount, timer / buffTime);
            yield return null;
        }
        effImg.fillAmount = hpImg.fillAmount;
    }

    public void SetPos(Vector2 worldPos)
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }

    public void SetColor(Color color)
    {
        hpImg.color = color;
    }

    //显示血条
    public void ShowHpBar()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        timer = 0; //重新计时
    }

    //隐藏血条
    public void HideHpBar()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    //private IEnumerator DestroyMe(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    PoolMgr.Instance.PushObj(gameObject);
    //}
}
