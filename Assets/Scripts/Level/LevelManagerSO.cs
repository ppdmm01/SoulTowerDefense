using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ؿ����������ݣ��������йؿ���
/// </summary>
[CreateAssetMenu(fileName = "LevelManagerSO", menuName = "ScriptableObject/LevelManagerSO")]
public class LevelManagerSO : ScriptableObject
{
    public List<LevelSO> levelSOList;
}
