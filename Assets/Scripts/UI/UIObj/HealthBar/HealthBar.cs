using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image hpImg; //Ѫ��
    public Image effImg; //������

    public float buffTime = 0.5f; //Ѫ����������ʱ��

    public float showTime = 2; //Ѫ����ʾʱ��
    private float timer;
    private bool isAutoHide; //�Ƿ��Զ�����

    private Coroutine updateCoroutine;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="nowHp">��ǰѪ��</param>
    /// <param name="maxHp">���Ѫ��</param>
    /// <param name="color">��ɫ</param>
    /// <param name="worldPos">Ŀ����������λ��</param>
    /// <param name="isAutoHide">�Ƿ��Զ�����</param>
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
        //��һ��ʱ���Զ�����Ѫ��
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
    /// ����Ѫ��
    /// </summary>
    /// <param name="nowHp">��ǰѪ��</param>
    /// <param name="maxHp">���Ѫ��</param>
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
        float effectLength = effImg.fillAmount - hpImg.fillAmount; //������Ҫ�ı�ĳ���
        float timer = 0f; //ÿһ�μ��ٵ�ʱ��
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

    //��ʾѪ��
    public void ShowHpBar()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        timer = 0; //���¼�ʱ
    }

    //����Ѫ��
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
