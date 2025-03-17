using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 存储防御塔最终数据（配置数据+动态修改的数据）
/// </summary>
public class TowerData
{
    public TowerSO originData; //原始数据

    [Header("防御塔描述")]
    public string towerName;
    public string towerChineseName;
    public string description;
    public Sprite towerIcon;

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

    [Header("Buff相关")]
    public List<BuffData> buffDatas; //Buff数据

    [Header("标签")]
    public List<ItemTag> itemTags; //用于与物品产生联动

    [Header("属性倍率计算")]
    public float damageMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public float intervalMultiplier = 1f;

    public TowerData(TowerSO towerSO)
    {
        Init(towerSO);
    }

    public TowerData(TowerData other)
    {
        originData = other.originData;
        towerName = other.towerName;
        towerChineseName = other.towerChineseName;
        description = other.description;
        towerIcon = other.towerIcon;
        hp = other.hp;
        cost = other.cost;
        canBeAttack = other.canBeAttack;
        isAttacker = other.isAttacker;
        damage = other.damage;
        range = other.range;
        interval = other.interval;
        output = other.output;
        cooldown = other.cooldown;
        buffDatas = other.GetBuffDatas();
        itemTags = other.itemTags;

        damageMultiplier = other.damageMultiplier;
        rangeMultiplier = other.rangeMultiplier;
        intervalMultiplier = other.intervalMultiplier;
    }

    /// <summary>
    /// 初始化用
    /// </summary>
    /// <param name="towerSO"></param>
    public void Init(TowerSO towerSO)
    {
        originData = towerSO;
        towerName = towerSO.towerName;
        towerChineseName = towerSO.towerChineseName;
        description = towerSO.description;
        towerIcon = towerSO.towerIcon;
        isAttacker = towerSO.isAttacker;
        hp = towerSO.hp;
        cost = towerSO.cost;
        canBeAttack = towerSO.canBeAttack;
        damage = towerSO.damage;
        range = towerSO.range;
        interval = towerSO.interval;
        isProducer = towerSO.isProducer;
        output = towerSO.output;
        cooldown = towerSO.cooldown;
        buffDatas = towerSO.GetBuffDatas();
        itemTags = towerSO.itemTags;

        damageMultiplier = 1f;
        rangeMultiplier = 1f;
        intervalMultiplier = 1f;
    }

    /// <summary>
    /// 获取拷贝后的buff数据
    /// </summary>
    /// <returns></returns>
    public List<BuffData> GetBuffDatas()
    {
        List<BuffData> list = new List<BuffData>();
        for(int i = 0; i < buffDatas.Count; i++)
        {
            list.Add(new BuffData(buffDatas[i]));
        }
        return list;
    }

    /// <summary>
    /// 更新属性
    /// </summary>
    public void UpdateAttribute()
    {
        damage = (int)(originData.damage * damageMultiplier);
        range = originData.range * rangeMultiplier;
        interval = originData.interval * intervalMultiplier;
        foreach (BuffData data in buffDatas)
        {
            data.damage = (int)(originData.GetBuffData(data.buffType).damage * data.damageMultiplier); //最终伤害：配置数据 * 当前倍率
        }
    }

    /// <summary>
    /// 获取buff数据
    /// </summary>
    public BuffData GetBuffData(BuffType type)
    {
        if (buffDatas.Any(data => data.buffType == type))
            return buffDatas.FirstOrDefault(data => data.buffType == type);
        else
            return null;
    }
}
