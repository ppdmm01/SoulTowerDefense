using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff���ã����ڸ�ħ��
/// </summary>
[CreateAssetMenu(fileName = "BuffConfigSO", menuName = "ScriptableObject/BuffConfigSO")]
public class BuffConfigSO : ScriptableObject
{
    public List<BuffData> BuffDataList;
}
