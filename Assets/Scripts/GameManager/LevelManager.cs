using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡管理器，管理关卡的开启和结束
/// </summary>
public class LevelManager : SingletonMono<LevelManager>
{
    public bool isInLevel; //是否在关卡中
    public LevelManagerSO data; //存储各个关卡

    private int nowWave; //当前波次
    private int totalWave; //总波次
    private float timer = 0; //计时器

    private int nowEnemyNum; //当前敌人数量

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<LevelManagerSO>("Data/LevelManagerSO");
            if (data == null)
                Debug.LogError("加载LevelManagerSO失败！");
        }
    }

    /// <summary>
    /// 开启一个关卡
    /// </summary>
    public void StartLevel(string sceneName,int levelNum = 1)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        ao.completed += (obj) =>
        {
            AudioManager.Instance.PlayBGM("BGM/FightMusic");
            //关卡加载完成，显示战斗面板，初始化资源，开始出怪等逻辑
            isInLevel = true;
            //创建防御塔按钮
            UIManager.Instance.ShowPanel<TowerPanel>().InitTowerBtn();
            //创建元气水晶
            TowerManager.Instance.CreateCore();
            //初始化资源
            GameResManager.Instance.ResetQiNum(); //先归零
            GameResManager.Instance.AddQiNum(100);
            //记录波次信息
            LevelSO levelData = data.levelSOList[levelNum - 1]; //获取关卡信息
            nowWave = 0;
            totalWave = levelData.waveInfos.Count;
            //初始化面板信息
            TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
            panel.UpdateWaveInfo(nowWave, totalWave);
            panel.UpdateEnemyNum(0);
            panel.UpdateQiNum(GameResManager.Instance.GetQiNum());
            //开始出怪
            StopAllCoroutines();
            StartCoroutine(SpawnLevelEnemies(levelData));
        };
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
        //显示胜利面板
        UIManager.Instance.ShowGameOverPanel(true);
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
        UIManager.Instance.GetPanel<TowerPanel>().UpdateEnemyNum(nowEnemyNum);
    }

    /// <summary>
    /// 减少怪物数量
    /// </summary>
    public void SubEnemyNum()
    {
        --nowEnemyNum;
        UIManager.Instance.GetPanel<TowerPanel>().UpdateEnemyNum(nowEnemyNum);
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
        StopAllCoroutines();
    }
}
