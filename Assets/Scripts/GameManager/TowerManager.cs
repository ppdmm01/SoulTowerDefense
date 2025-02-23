using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 防御塔管理器（管理防御塔放置、信息查看、存储等）
/// </summary>
public class TowerManager : SingletonMono<TowerManager>
{
    [HideInInspector] public Core core; //塔防核心

    [Header("防御塔所在层")]
    [SerializeField] private LayerMask towerLayer; //防御塔所在层

    private TowerManagerSO data; //存储防御塔配置数据
    private Dictionary<string, TowerSO> towerDataDic; //存储防御塔配置数据

    public Dictionary<string,TowerData> towers; //记录目前选择的防御塔及其数据
    public Dictionary<string,TowerData> oldTowerDatas; //记录上一次变化前的数据（用于计算属性变化）

    public List<BaseTower> gameTowerList; //记录场上的防御塔

    [Header("防御塔放置相关")]
    public bool isPlacing; //是否放置塔防中
    public BaseTower target; //正在放置的目标
    //public List<GameObject> collsionTowerList; //记录在碰撞范围内的防御塔
    private BaseTower nowTower; //当前检测的防御塔

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<TowerManagerSO>("Data/TowerManagerSO");
            if (data == null)
                Debug.LogError("加载TowerManagerSO失败！");
        }

        towerDataDic = new Dictionary<string, TowerSO>();
        foreach (TowerSO towerSO in data.towerSOList)
        {
            towerDataDic.Add(towerSO.towerName, towerSO);
        }
    }

    void Start()
    {
        towers = new Dictionary<string, TowerData>();
        oldTowerDatas = new Dictionary<string, TowerData>();

        gameTowerList = new List<BaseTower>();
        //collsionTowerList = new List<GameObject>();
        nowTower = null;
    }

    void Update()
    {
        TowerRangeOperation();
        PlaceTowerOperation();
    }

    /// <summary>
    /// 创建防御塔
    /// </summary>
    /// <param name="towerName">防御塔名称</param>
    public void CreateTower(string towerName)
    {
        GameObject towerObj = Instantiate(Resources.Load<GameObject>("Tower/" + towerName));
        if (towerObj == null)
            Debug.LogError("防御塔不存在！");
        //初始化数据
        BaseTower tower = towerObj.GetComponent<BaseTower>();
        tower.Init(towers[towerName]);
        //放置中，未使用
        tower.towerCollider.isTrigger = true;
        tower.isUsed = false;
        isPlacing = true;
        target = tower;
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

    /// <summary>
    /// 放置防御塔
    /// </summary>
    public void PlaceTower()
    {
        if (HasResources())
        {
            GameResManager.Instance.AddQiNum(-target.data.cost); //消耗资源

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
            Debug.Log("资源不足，放置失败！");
            //CancelPlaceTower();
        }

        //collsionTowerList.Clear(); //清空数据，方便下一次放置塔防时调用
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

    /// <summary>
    /// 检测防御塔范围的操作
    /// </summary>
    private void TowerRangeOperation()
    {
        if (isPlacing) return; //如果正在放置防御塔，则不要显示其他的防御塔范围

        // 将鼠标位置从屏幕坐标转换为世界坐标  
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 检测鼠标是否悬停在防御塔本体上  
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, towerLayer);
        if (hit.collider != null)
        {
            if (nowTower != hit.collider.GetComponent<BaseTower>())
            {
                if (nowTower != null)
                    nowTower.HideRange(); //把之前塔范围隐藏了
                nowTower = hit.collider.GetComponent<BaseTower>();
            }
            nowTower.ShowRange();
        }
        else
        {
            if (nowTower != null)
            {
                nowTower.HideRange();
                nowTower = null;
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
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.transform.position = mousePos;
            //显示防御塔范围
            target.ShowRange();
            if (HasResources())
                target.SetRangeColor(Defines.validRangeColor);
            else
                target.SetRangeColor(Defines.invalidRangeColor);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("取消放置");
                CancelPlaceTower();
            }
        }
    }
    
    /// <summary>
    /// 是否有放置该防御塔的资源
    /// </summary>
    public bool HasResources()
    {
        return GameResManager.Instance.GetQiNum() >= target.data.cost;
    }

    /// <summary>
    /// 清理场上所有防御塔
    /// </summary>
    public void ClearAllTower()
    {
        foreach (BaseTower tower in gameTowerList)
        {
            Destroy(tower.gameObject);
        }
        gameTowerList.Clear();
    }

    /// <summary>
    /// 添加要用到的防御塔
    /// </summary>
    public void AddTower(string towerName,TowerData data)
    {
        if (towers.ContainsKey(towerName)) return;
        towers.Add(towerName,data);
    }

    /// <summary>
    /// 移除要用到防御塔
    /// </summary>
    /// <param name="towerName"></param>
    public void RemoveTower(string towerName)
    {
        if (towers.ContainsKey(towerName))
            towers.Remove(towerName);
    }

    /// <summary>
    /// 通过名字获取防御塔配置数据
    /// </summary>
    /// <param name="towerName">防御塔名字</param>
    /// <returns>防御塔配置数据</returns>
    public TowerSO GetTowerSO_ByName(string towerName)
    {
        if (!towerDataDic.ContainsKey(towerName)) return null;
        return towerDataDic[towerName];
    }

    /// <summary>
    /// 修改指定防御塔属性
    /// </summary>
    /// <param name="towerName">防御塔名字</param>
    /// <param name="activeEffect">激活效果</param>
    public void SetTowerDataFromName(string towerName, ItemActiveEffect[] activeEffects)
    {
        if (!towers.ContainsKey(towerName)) 
        {
            Debug.Log("未找到名为"+towerName+"的防御塔");
            return;
        }

        TowerData data = towers[towerName];
        foreach (ItemActiveEffect activeEffect in activeEffects)
        {
            switch (activeEffect.effectType)
            {
                case ItemActiveEffect.EffectType.Hp:
                    data.hp += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Cost:
                    data.cost += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Damage:
                    data.damage += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Range:
                    data.range += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Interval:
                    data.interval += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Output:
                    data.output += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Cooldown:
                    data.cooldown += activeEffect.value;
                    break;
            }
        }
    }

    /// <summary>
    /// 通过标签统一修改防御塔属性
    /// </summary>
    /// <param name="tags">标签</param>
    /// <param name="activeEffects">激活效果</param>
    public void SetTowerDataFromTag(ItemTag[] tags, ItemActiveEffect[] activeEffects)
    {
        bool flag = true; //标记是否满足标签条件
        foreach (TowerData data in towers.Values)
        {
            flag = true;
            //只有所有标签满足才行
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
        foreach (string towerName in towers.Keys)
        {
            TowerData oldData = new TowerData(towers[towerName]);
            oldTowerDatas.Add(towerName, oldData);
        }
    }
}
