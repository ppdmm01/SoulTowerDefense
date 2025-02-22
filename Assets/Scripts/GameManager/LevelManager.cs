using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �ؿ�������������ؿ��Ŀ����ͽ���
/// </summary>
public class LevelManager : SingletonMono<LevelManager>
{
    public bool isInLevel; //�Ƿ��ڹؿ���
    //TODO:�����洢�����ؿ�

    /// <summary>
    /// ����һ���ؿ�
    /// </summary>
    public void StartLevel(string sceneName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        ao.completed += (obj) =>
        {
            //�ؿ�������ɣ���ʾս����壬��ʼ����Դ����ʼ���ֵ��߼�
            isInLevel = true;
            UIManager.Instance.ShowPanel<TowerPanel>().InitTowerBtn();
            TowerManager.Instance.CreateCore();
        };
    }
}
