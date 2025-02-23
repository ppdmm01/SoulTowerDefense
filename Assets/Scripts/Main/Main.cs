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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (LevelManager.Instance.isInLevel)
            {
                LevelManager.Instance.SkipThisWave();
            }
        }
    }
}
