using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�����
/// </summary>
public class Main : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.ShowPanel<BagPanel>();
    }
}
