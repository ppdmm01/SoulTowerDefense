using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��Ч����
/// </summary>
public class Effect : MonoBehaviour
{
    public float DestroyTime = 3f;
    private UnityAction callback;
    private void OnEnable()
    {
        DestroyMe(DestroyTime);
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one;
        StopAllCoroutines();
        callback = null;
    }

    public void AddCallBack(UnityAction callback)
    {
        this.callback = callback;
    }

    /// <summary>
    /// ɾ���Լ�
    /// </summary>
    /// <param name="time"></param>
    protected virtual void DestroyMe(float time = 0f)
    {
        StopAllCoroutines();
        StartCoroutine(ReallyDestroy(time));
    }

    protected virtual IEnumerator ReallyDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
        callback = null;
        PoolMgr.Instance.PushObj(gameObject);
    }

    /// <summary>
    /// ����ɾ���Լ�
    /// </summary>
    public virtual void DestroyMeImmediate()
    {
        StopAllCoroutines();
        callback?.Invoke();
        callback = null;
        PoolMgr.Instance.PushObj(gameObject);
        EffectManager.Instance.RemoveEffect(this);
    }
}
