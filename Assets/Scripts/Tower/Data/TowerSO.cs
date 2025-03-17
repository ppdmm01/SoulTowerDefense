using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 防御塔数据
/// </summary>
[CreateAssetMenu(fileName = "TowerSO",menuName = "ScriptableObject/TowerSO")]
public class TowerSO : ScriptableObject
{
    [Header("防御塔描述")]
    public string towerName; //防御塔名称
    public string towerChineseName; //防御塔中文名称
    [TextArea]
    public string description; //描述
    public Sprite towerIcon; //防御塔图片

    [Header("基础属性")]
    public int hp; //血量
    public int cost; //花费
    public bool canBeAttack; //是否能被攻击

    [Header("攻击相关")]
    public bool isAttacker; //是否是攻击者
    public int damage; //伤害
    public float range; //攻击范围
    public float interval; //攻击间隔

    [Header("生产资源相关")]
    public bool isProducer; //是否生产资源
    public int output; //每次产量
    public float cooldown; //生产冷却时间

    [Header("Buff数据")]
    public List<BuffData> buffDatas;

    [Header("标签")]
    public List<ItemTag> itemTags; //用于与物品产生联动

    /// <summary>
    /// 获取拷贝后的buff数据
    /// </summary>
    /// <returns></returns>
    public List<BuffData> GetBuffDatas()
    {
        List<BuffData> list = new List<BuffData>();
        for (int i = 0; i < buffDatas.Count; i++)
        {
            list.Add(new BuffData(buffDatas[i]));
        }
        return list;
    }

    /// <summary>
    /// 获取Buff数据
    /// </summary>
    public BuffData GetBuffData(BuffType type)
    {
        if (buffDatas.Any(data => data.buffType == type))
            return buffDatas.FirstOrDefault(data => data.buffType == type);
        else
            return null;
    }
}
