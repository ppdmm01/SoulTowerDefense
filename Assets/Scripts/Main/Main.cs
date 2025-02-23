using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏主入口
/// </summary>
public class Main : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.ShowPanel<BagPanel>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (LevelManager.Instance.isInLevel)
            {
                LevelManager.Instance.SkipThisWave();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); //退出游戏
        }
    }
}
