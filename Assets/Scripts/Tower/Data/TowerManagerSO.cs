using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerManagerSO",menuName = "ScriptableObject/TowerManagerSO")]
public class TowerManagerSO : ScriptableObject
{
    [Header("·ÀÓùËşÊı¾İ")]
    public List<TowerSO> towerSOList;
}
