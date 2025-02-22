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
    //TODO:变量存储各个关卡

    /// <summary>
    /// 开启一个关卡
    /// </summary>
    public void StartLevel(string sceneName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        ao.completed += (obj) =>
        {
            //关卡加载完成，显示战斗面板，初始化资源，开始出怪等逻辑
            isInLevel = true;
            UIManager.Instance.ShowPanel<TowerPanel>().InitTowerBtn();
            TowerManager.Instance.CreateCore();
        };
    }
}
