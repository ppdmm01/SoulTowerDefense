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
    public Core core; //塔防核心(后面通过动态加载的形式)

    [SerializeField] private LayerMask towerLayer; //防御塔所在层
    private BaseTower nowTower; //当前检测的防御塔

    public List<BaseTower> towerList; //防御塔列表

    [Header("防御塔放置相关")]
    public bool isPlacing; //是否放置塔防中
    public BaseTower target; //正在放置的目标
    public List<GameObject> collisonTowerList; //记录在碰撞范围内的防御塔

    void Start()
    {
        towerList = new List<BaseTower>();
        collisonTowerList = new List<GameObject>();
        nowTower = null;

        //创建核心
        CreateCore();
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
        towerList.Add(tower);

        if (towerObj == null)
            Debug.LogError("防御塔不存在！");

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

        core.isUsed = true;
        this.core = core;
    }

    /// <summary>
    /// 放置防御塔
    /// </summary>
    public void PlaceTower()
    {
        if (CanPlace())
        {
            if (HasResources())
            {
                GameResManager.Instance.AddQiNum(-target.data.cost); //消耗资源
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
        else
        { 
            Debug.Log("位置不足，放置失败！");
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
            if (CanPlace() && HasResources())
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
    /// 是否可以放置防御塔
    /// </summary>
    public bool CanPlace()
    {
        return collisonTowerList.Count <= 0;
    }
    
    /// <summary>
    /// 是否有放置该防御塔的资源
    /// </summary>
    public bool HasResources()
    {
        return GameResManager.Instance.GetQiNum() >= target.data.cost;
    }
}
