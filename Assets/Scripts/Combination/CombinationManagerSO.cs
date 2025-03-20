using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombinationManagerSO", menuName = "ScriptableObject/CombinationManagerSO")]
public class CombinationManagerSO : ScriptableObject
{
    public List<CombinationSO> combinationList; //组合列表
}
