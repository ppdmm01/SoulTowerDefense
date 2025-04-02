using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡管理器数据（管理所有关卡）
/// </summary>
[CreateAssetMenu(fileName = "LevelManagerSO", menuName = "ScriptableObjec/LevelManagerSO")]
public class LevelManagerSO : ScriptableObject
{
    public List<LevelSO> levelSOList;
}
