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

    private List<BaseTower> gameTowerList; //记录场上的防御塔

    [Header("防御塔放置相关")]
    public bool isPlacing; //是否放置塔防中
    public BaseTower target; //正在放置的目标
    public List<GameObject> collisonTowerList; //记录在碰撞范围内的防御塔
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
        gameTowerList = new List<BaseTower>();
        collisonTowerList = new List<GameObject>();
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
    /// <param name="towerName"></param>
    public void CreateTower(string towerName)
    {
        GameObject towerObj = Instantiate(Resources.Load<GameObject>("Tower/" + towerName));
        BaseTower tower = towerObj.GetComponent<BaseTower>();
        gameTowerList.Add(tower);

        if (towerObj == null)
            Debug.LogError("防御塔不存在！");

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
        Core core = coreObj.GetComponent<Core>();
        if (coreObj == null)
            Debug.LogError("核心不存在！");

        //使用
        core.isUsed = true;
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
            target.SetHpBarPos(target.transform.position+Vector3.up);
            target.ShowHpBar();
            target.towerCollider.isTrigger = false;
            target.isUsed = true;
            isPlacing = false;
            target.HideRange();
            target = null;
        }
        else
        {
            Debug.Log("资源不足，放置失败！");
            //CancelPlaceTower();
        }

        collisonTowerList.Clear(); //清空数据，方便下一次放置塔防时调用
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
    /// 清理所有防御塔
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

    public TowerSO GetTowerSOByName(string towerName)
    {
        if (!towerDataDic.ContainsKey(towerName)) return null;
        return towerDataDic[towerName];
    }
}
