using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyManagerSO", menuName = "ScriptableObject/EnemyManagerSO")]
public class EnemyManagerSO : ScriptableObject
{
    [Header("µÐÈËÊý¾Ý")]
    public List<EnemySO> enemySOList;
}
