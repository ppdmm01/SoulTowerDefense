using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡管理器，管理关卡的开启和结束
/// </summary>
public class LevelManager : SingletonMono<LevelManager>
{
    public bool isInLevel; //是否在关卡中
    public LevelManagerSO data; //存储各个关卡
    private LevelSO currentLevelData; //当前关卡

    private int nowWave; //当前波次
    private int totalWave; //总波次
    private float timer = 0; //计时器

    private int nowEnemyNum; //当前敌人数量

    private bool isQuit; //是否退出

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<LevelManagerSO>("Data/LevelManagerSO");
            if (data == null)
                Debug.LogError("加载LevelManagerSO失败！");
        }
        isQuit = false;
    }

    /// <summary>
    /// 开启一个关卡
    /// </summary>
    public void StartLevel(string sceneName,int mapLayer)
    {
        UIManager.Instance.LoadScene(sceneName, () =>
        {
            UIManager.Instance.HidePanel<BagPanel>();
            UIManager.Instance.HidePanel<PreFightPanel>();
        }, 
        () =>
        {
            AudioManager.Instance.PlayBGM("BGM/Fight");
            //关卡加载完成，显示战斗面板，初始化资源，开始出怪等逻辑
            isInLevel = true;
            //创建防御塔按钮
            UIManager.Instance.ShowPanel<TowerPanel>().InitTowerBtn();
            //创建元气水晶
            TowerManager.Instance.CreateCore();
            //初始化资源
            GameResManager.Instance.ResetSoulNum(); //先归零
            GameResManager.Instance.AddSoulNum(100 + mapLayer*5);
            //记录波次信息
            LevelSO levelData = GetLevelData(mapLayer); //获取关卡信息
            currentLevelData = levelData;
            nowWave = 0;
            totalWave = levelData.waveInfos.Count;
            //初始化面板信息
            TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
            panel.UpdateWaveInfo(nowWave, totalWave);
            panel.UpdateEnemyNum(0);
            panel.UpdateSoulNum(GameResManager.Instance.GetSoulNum());
            //开始出怪
            StopAllCoroutines();
            StartCoroutine(SpawnLevelEnemies(levelData));
        }); 
    }

    //获取一个关卡
    public LevelSO GetLevelData(int mapLayer)
    {
        int level = (mapLayer+1) / 2; //获取关卡等级
        List<LevelSO> list = data.levelSOList.Where(levelData => levelData.level == level).ToList(); //获取同等级的关卡
        LevelSO levelData = list.Random();
        Debug.Log("level:" + levelData.level + " name:" + levelData.name);
        return levelData;
    }

    /// <summary>
    /// 开始关卡的出怪
    /// </summary>
    public IEnumerator SpawnLevelEnemies(LevelSO levelData)
    {
        timer = Defines.waitTime; //回合开始前准备时间
        //等待时间
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        ++nowWave;
        //出怪时间
        while (nowWave <= totalWave)
        {
            nowEnemyNum = 0;
            float totalTime = SpawnWaveEnemies(levelData.GetWaveInfo(nowWave));
            UIManager.Instance.ShowTipInfo($"第{nowWave}波");
            UIManager.Instance.GetPanel<TowerPanel>().UpdateWaveInfo(nowWave, totalWave);
            timer = totalTime;
            while (timer > 0)
            {
                timer -= Time.deltaTime; //等待全部怪物出完
                yield return null;
            }

            while (nowEnemyNum != 0)
            {
                yield return null; //只有全部怪物杀完，才能进入下一波
            }
            ++nowWave;
        }
        nowWave = totalWave;
        yield return new WaitForSeconds(1f);
        //显示胜利面板
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Fight)
            WinFight(); //战斗胜利
        else if (PlayerStateManager.Instance.CurrentState == PlayerState.Boss)
            WinGame(); //游戏胜利
    }

    /// <summary>
    /// 出一波敌人
    /// </summary>
    /// <param name="waveInfo">波数信息</param>
    /// <returns>生成全部敌人所需要的时间</returns>
    private float SpawnWaveEnemies(WaveInfo waveInfo)
    {
        float totalTime = 0;
        foreach (SpawnInfo info in waveInfo.spawnInfos)
        {
            totalTime = Mathf.Max(totalTime,info.GetTotalTime()); //获取最长的出怪时间
            AddEnemyNum(info.totalNum);
            EnemyManager.Instance.SpawnEnemies(info.enemyName, info.totalNum, info.spawnNum, info.frequency, info.delayTime);
        }
        return totalTime;
    }

    /// <summary>
    /// 跳过当前波次
    /// </summary>
    public void SkipThisWave()
    {
        if (nowWave <= totalWave)
        {
            //清除场上所有敌人
            EnemyManager.Instance.Clear();
            //敌人数量清0
            timer = 0;
            nowEnemyNum = 0; //清0时自动进入下一波
            UIManager.Instance.GetPanel<TowerPanel>().UpdateEnemyNum(nowEnemyNum);
        }
    }

    /// <summary>
    /// 增加怪物数量
    /// </summary>
    private void AddEnemyNum(int num)
    {
        nowEnemyNum += num;
        UIManager.Instance.GetPanel<TowerPanel>()?.UpdateEnemyNum(nowEnemyNum);
    }

    /// <summary>
    /// 减少怪物数量
    /// </summary>
    public void SubEnemyNum()
    {
        --nowEnemyNum;
        UIManager.Instance.GetPanel<TowerPanel>()?.UpdateEnemyNum(nowEnemyNum);
    }

    /// <summary>
    /// 清理战场
    /// </summary>
    public void Clear()
    {
        isInLevel = false;
        nowWave = totalWave = 0;
        timer = 0;
        nowEnemyNum = 0;
        TowerManager.Instance.Clear();
        EnemyManager.Instance.Clear();
        PoolMgr.Instance.ClearPool(); //切换场景前先清除对象池
        StopAllCoroutines();
    }

    public void WinFight()
    {
        //清理战场
        Clear();

        //显示面板
        UIManager.Instance.LoadScene("MapScene", () =>
        {
            UIManager.Instance.HidePanel<TowerPanel>();
            RewardPanel rewardPanel = UIManager.Instance.ShowPanel<RewardPanel>();
            rewardPanel.SetReward("战利品",currentLevelData.rewardDatas);
            AudioManager.Instance.PlayBGM("BGM/Music2");
        });
    }

    public void WinGame()
    {
        UIManager.Instance.HidePanel<TowerPanel>();
        GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();
        panel.SetTitle(true);
        GameResManager.Instance.ResetCoreHp();
        GameResManager.Instance.ResetTaixuNum();
    }

    public void LoseGame()
    {
        //if (PlayerStateManager.Instance.CurrentState == PlayerState.Menu) return; //如果是菜单退出导致的，则忽视
        UIManager.Instance.HidePanel<TowerPanel>();
        GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();
        panel.SetTitle(false);
        GameResManager.Instance.ResetCoreHp();
        GameResManager.Instance.ResetTaixuNum();
    }

    private void OnApplicationQuit()
    {
        //战斗途中退出，则退回到上一个节点
        isQuit = true;
        if (isInLevel)
        {
            if (PlayerStateManager.Instance.CurrentState == PlayerState.Fight ||
            PlayerStateManager.Instance.CurrentState == PlayerState.Boss ||
            PlayerStateManager.Instance.CurrentState == PlayerState.Menu)
            {
                Map currentMap = GameDataManager.Instance.mapData;
                if (currentMap != null && currentMap.path.Count >= 1)
                    currentMap.path.RemoveAt(currentMap.path.Count - 1);
                GameDataManager.Instance.SaveMapData(currentMap);
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log(isInLevel);
        //战斗途中退出，则退回到上一个节点（直接退出游戏时会触发OnApplicationQuit和该方法，所以避免重复触发）
        if (isInLevel && !isQuit)
        {
            if (PlayerStateManager.Instance.CurrentState == PlayerState.Fight ||
            PlayerStateManager.Instance.CurrentState == PlayerState.Boss ||
            PlayerStateManager.Instance.CurrentState == PlayerState.Menu)
            {
                Debug.Log("退回节点");
                Map currentMap = GameDataManager.Instance.mapData;
                if (currentMap != null && currentMap.path.Count >= 1)
                    currentMap.path.RemoveAt(currentMap.path.Count - 1);
                GameDataManager.Instance.SaveMapData(currentMap);
            }
        }
    }
}
