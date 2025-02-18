using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ч����
/// </summary>
public class Effect : MonoBehaviour
{
    private void OnEnable()
    {
        DestroyMe(2f);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// ɾ���Լ�
    /// </summary>
    /// <param name="time"></param>
    public void DestroyMe(float time = 0f)
    {
        StopAllCoroutines();
        StartCoroutine(ReallyDestroy(time));
    }

    private IEnumerator ReallyDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        PoolMgr.Instance.PushObj(gameObject);
    }
}
