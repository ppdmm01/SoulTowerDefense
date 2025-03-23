using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 防御塔管理器（管理防御塔放置、信息查看、存储等）
/// </summary>
public class TowerManager : SingletonMono<TowerManager>
{
    //【防御塔配置数据，清理时不用清除】
    private TowerManagerSO data; //存储防御塔配置数据
    private Dictionary<string, TowerSO> towerSODic; //存储防御塔配置数据

    //记录动态变化的防御塔数据
    public Dictionary<string,TowerData> towerDatas; //记录目前选择的防御塔及其数据
    public Dictionary<string,TowerData> oldTowerDatas; //记录上一次变化的数据（用于计算属性变化）

    //记录场上的塔
    public Core core; //记录基地核心
    public List<BaseTower> gameTowerList; //记录场上的防御塔

    //操作相关
    private bool isPlacing; //是否放置塔防中
    public bool isOpenPanel; //是否正在打开防御塔操作面板
    private BaseTower target; //当前放置的目标防御塔
    private BaseTower nowTower; //当前检测范围的防御塔

    private float operationOffsetTime = 0.1f; //操作间隔时间(防止误触连续点击)
    private float timer; //计时器

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<TowerManagerSO>("Data/TowerManagerSO");
            if (data == null)
                Debug.LogError("加载TowerManagerSO失败！");
        }

        towerSODic = new Dictionary<string, TowerSO>();
        foreach (TowerSO towerSO in data.towerSOList)
        {
            towerSODic.Add(towerSO.towerName, towerSO);
        }
    }

    void Start()
    {
        towerDatas = new Dictionary<string, TowerData>();
        oldTowerDatas = new Dictionary<string, TowerData>();

        gameTowerList = new List<BaseTower>();
        nowTower = null;
    }

    void Update()
    {
        TowerRangeOperation(); //查看防御塔范围操作
        SellTowerOperation(); //售卖防御塔操作
        PlaceTowerOperation(); //放置防御塔操作（放在最后，避免放置时的bool变化影响前面操作）
    }

    /// <summary>
    /// 创建防御塔
    /// </summary>
    /// <param name="towerName">防御塔名称</param>
    public void CreateTower(string towerName)
    {
        if (isPlacing) return; //如果正在放置，则不能再弄了

        GameObject towerObj = Instantiate(Resources.Load<GameObject>("Tower/" + towerName));
        if (towerObj == null)
            Debug.LogError("防御塔不存在！");
        //初始化数据
        BaseTower tower = towerObj.GetComponent<BaseTower>();
        tower.Init(towerDatas[towerName]);
        //放置中，未使用
        tower.towerCollider.isTrigger = true;
        tower.isUsed = false;
        isPlacing = true;
        target = tower;

        timer = 0;
    }

    /// <summary>
    /// 创建基地核心
    /// </summary>
    public void CreateCore()
    {
        GameObject coreObj = Instantiate(Resources.Load<GameObject>("Tower/Core"),Vector2.zero,Quaternion.identity);
        if (coreObj == null)
            Debug.LogError("核心不存在！");
        //初始化数据
        Core core = coreObj.GetComponent<Core>();
        TowerData towerData = new TowerData(GetTowerSO_ByName("Core"));
        core.Init(towerData);

        //使用
        core.isUsed = true;
        //记录
        this.core = core;
    }

    #region 操作相关
    /// <summary>
    /// 检测防御塔范围的操作
    /// </summary>
    private void TowerRangeOperation()
    {
        if (isPlacing || isOpenPanel) return; //如果正在放置防御塔或者操作面板已打开，则跳过

        // 将鼠标位置从屏幕坐标转换为世界坐标  
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 检测鼠标是否悬停在防御塔本体上  
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tower"));
        if (hit.collider != null)
        {
            if (nowTower != hit.collider.GetComponent<BaseTower>()) //如果和之前的防御塔不一样
            {
                if (nowTower != null)
                    nowTower.HideRange(); //把之前塔范围隐藏了
                nowTower = hit.collider.GetComponent<BaseTower>();
            }
            if (nowTower != null)
                nowTower.ShowRange();
        }
        else if (nowTower != null)
        {
            nowTower.HideRange();
            nowTower = null;
        }
    }

    /// <summary>
    /// 卖塔操作（返还50%）
    /// </summary>
    private void SellTowerOperation()
    {
        if (isPlacing) return;

        //打开面板操作
        if (nowTower != null && core != nowTower) //防御塔存在并且不是核心
        {
            if (Input.GetMouseButtonDown(0))
            {
                nowTower.HideRange();
                //同时也要显示血条
                nowTower.ShowHpBar();
                //显示卖塔界面
                TowerOperationPanel panel = UIManager.Instance.ShowPanel<TowerOperationPanel>();
                panel.SetInfo(nowTower);
                isOpenPanel = true;
            }
        }
    }

    /// <summary>
    /// 放置防御塔的操作
    /// </summary>
    private void PlaceTowerOperation()
    {
        if (isPlacing)
        {
            timer += Time.deltaTime;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.transform.position = mousePos;
            //显示防御塔范围
            target.ShowRange();
            if (HasResources())
                target.SetRangeColor(Defines.validRangeColor);
            else
                target.SetRangeColor(Defines.invalidRangeColor);

            if (Input.GetMouseButtonDown(0) && timer >= operationOffsetTime)
            {
                PlaceTower();
            }
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlaceTower();
            }
        }
    }


    /// <summary>
    /// 放置防御塔
    /// </summary>
    public void PlaceTower()
    {
        if (HasResources())
        {
            GameResManager.Instance.AddSoulNum(-target.data.cost); //消耗资源

            //使用
            target.ShowHpBar();
            target.towerCollider.isTrigger = false;
            target.isUsed = true;
            isPlacing = false;
            target.HideRange();
            //记录
            gameTowerList.Add(target);
            target = null;
        }
        else
        {
            UIManager.Instance.ShowTipInfo("资源不足，放置失败！");
        }
    }

    /// <summary>
    /// 取消放置防御塔
    /// </summary>
    public void CancelPlaceTower()
    {
        isPlacing = false;
        if (target != null)
            Destroy(target.gameObject);
        target = null;
    }
    #endregion

    /// <summary>
    /// 是否有放置该防御塔的资源
    /// </summary>
    public bool HasResources()
    {
        return GameResManager.Instance.GetSoulNum() >= target.data.cost;
    }

    /// <summary>
    /// 添加要用到的防御塔
    /// </summary>
    public void AddTowerData(string towerName,TowerData data)
    {
        if (towerDatas.ContainsKey(towerName)) return;
        towerDatas.Add(towerName,data);
    }

    /// <summary>
    /// 移除要用到防御塔
    /// </summary>
    /// <param name="towerName"></param>
    public void RemoveTowerData(string towerName)
    {
        if (towerDatas.ContainsKey(towerName))
            towerDatas.Remove(towerName);
    }

    /// <summary>
    /// 通过名字获取防御塔配置数据
    /// </summary>
    /// <param name="towerName">防御塔名字</param>
    /// <returns>防御塔配置数据</returns>
    public TowerSO GetTowerSO_ByName(string towerName)
    {
        if (!towerSODic.ContainsKey(towerName)) return null;
        return towerSODic[towerName];
    }

    /// <summary>
    /// 修改指定防御塔属性
    /// </summary>
    /// <param name="towerName">防御塔名字</param>
    /// <param name="activeEffect">激活效果</param>
    public void SetTowerDataFromName(string towerName, ItemActiveEffect[] activeEffects)
    {
        if (!towerDatas.ContainsKey(towerName)) 
        {
            Debug.Log("未找到名为"+towerName+"的防御塔");
            return;
        }

        TowerData data = towerDatas[towerName];
        BuffData buffData;
        foreach (ItemActiveEffect activeEffect in activeEffects)
        {
            switch (activeEffect.effectType)
            {
                case ItemActiveEffect.EffectType.Hp:
                    data.hp += Mathf.RoundToInt(activeEffect.value);
                    break;
                case ItemActiveEffect.EffectType.Cost:
                    data.cost += Mathf.RoundToInt(activeEffect.value);
                    break;
                case ItemActiveEffect.EffectType.Output:
                    data.output += Mathf.RoundToInt(activeEffect.value);
                    break;
                case ItemActiveEffect.EffectType.Cooldown:
                    data.cooldown += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.DamageMultiplier:
                    data.damageMultiplier += activeEffect.value;
                    data.UpdateAttribute();
                    break;
                case ItemActiveEffect.EffectType.RangeMultiplier:
                    data.rangeMultiplier += activeEffect.value;
                    data.UpdateAttribute();
                    break;
                case ItemActiveEffect.EffectType.IntervalMultiplier:
                    data.intervalMultiplier += activeEffect.value;
                    data.UpdateAttribute();
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_Duration:
                    buffData = data.GetBuffData(BuffType.Burn);
                    if (buffData != null)
                        buffData.duration += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_Damage:
                    buffData = data.GetBuffData(BuffType.Burn);
                    if (buffData != null)
                        buffData.damageMultiplier += activeEffect.value;
                    data.UpdateAttribute();
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_TriggerChance:
                    buffData = data.GetBuffData(BuffType.Burn);
                    if (buffData != null)
                        buffData.triggerChance += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.SlowBuff_Duration:
                    buffData = data.GetBuffData(BuffType.Slow);
                    if (buffData != null)
                        buffData.duration += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.SlowBuff_TriggerChance:
                    buffData = data.GetBuffData(BuffType.Slow);
                    if (buffData != null)
                        buffData.triggerChance += activeEffect.value;
                    break;
            }
        }
    }

    /// <summary>
    /// 通过标签统一修改防御塔属性
    /// </summary>
    /// <param name="tags">标签</param>
    /// <param name="activeEffects">激活效果</param>
    public void SetTowerDataFromTag(ItemTag[] tags,ItemActiveEffect[] activeEffects)
    {
        bool flag = true; //标记是否满足标签条件
        foreach (TowerData data in towerDatas.Values)
        {
            flag = true;
            //只有所有标签满足并且检测点类型对应才行
            foreach (ItemTag tag in tags)
                if (!data.itemTags.Contains(tag)) 
                    flag = false;

            if (flag)
                SetTowerDataFromName(data.towerName, activeEffects);
        }
    }

    /// <summary>
    /// 记录老数据
    /// </summary>
    public void RecordOldData()
    {
        //清理老数据
        oldTowerDatas.Clear();
        foreach (string towerName in towerDatas.Keys)
        {
            TowerData oldData = new TowerData(towerDatas[towerName]);
            oldTowerDatas.Add(towerName, oldData);
        }
    }

    /// <summary>
    /// 清理战场
    /// </summary>
    public void Clear()
    {
        Debug.Log("清理战场");
        //清除动态数据
        towerDatas.Clear();
        oldTowerDatas.Clear();
        //清除核心
        if (core != null)
            core.Dead();
        core = null;
        //清除场上的防御塔
        foreach (BaseTower tower in gameTowerList.ToList())
            tower.Dead();
    }
}
