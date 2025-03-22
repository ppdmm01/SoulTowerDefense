using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO",menuName = "ScriptableObject/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("基础属性")]
    public int hp; //血量
    public float moveSpeed; //移动速度
    public int atk; //攻击力
    public float interval; //攻击间隔
    public int soulNum; //死亡后获得得资源
}
