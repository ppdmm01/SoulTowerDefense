using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �ؿ�������������ؿ��Ŀ����ͽ���
/// </summary>
public class LevelManager : SingletonMono<LevelManager>
{
    public bool isInLevel; //�Ƿ��ڹؿ���
    public LevelManagerSO data; //�洢�����ؿ�
    private LevelSO currentLevel; //��ǰ�ؿ�

    private int nowWave; //��ǰ����
    private int totalWave; //�ܲ���
    private float timer = 0; //��ʱ��

    private int nowEnemyNum; //��ǰ��������

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<LevelManagerSO>("Data/LevelManagerSO");
            if (data == null)
                Debug.LogError("����LevelManagerSOʧ�ܣ�");
        }
    }

    /// <summary>
    /// ����һ���ؿ�
    /// </summary>
    public void StartLevel(string sceneName,int levelNum = 1)
    {
        UIManager.Instance.LoadScene(sceneName, () =>
        {
            UIManager.Instance.HidePanel<BagPanel>();
            UIManager.Instance.HidePanel<PreFightPanel>();
        }, 
        () =>
        {
            AudioManager.Instance.PlayBGM("BGM/FightMusic");
            //�ؿ�������ɣ���ʾս����壬��ʼ����Դ����ʼ���ֵ��߼�
            isInLevel = true;
            //������������ť
            UIManager.Instance.ShowPanel<TowerPanel>().InitTowerBtn();
            //����Ԫ��ˮ��
            TowerManager.Instance.CreateCore();
            //��ʼ����Դ
            GameResManager.Instance.ResetSoulNum(); //�ȹ���
            GameResManager.Instance.AddSoulNum(100);
            //��¼������Ϣ
            LevelSO levelData = data.levelSOList[levelNum - 1]; //��ȡ�ؿ���Ϣ
            currentLevel = levelData;
            nowWave = 0;
            totalWave = levelData.waveInfos.Count;
            //��ʼ�������Ϣ
            TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
            panel.UpdateWaveInfo(nowWave, totalWave);
            panel.UpdateEnemyNum(0);
            panel.UpdateSoulNum(GameResManager.Instance.GetSoulNum());
            //��ʼ����
            StopAllCoroutines();
            StartCoroutine(SpawnLevelEnemies(levelData));
        }); 
    }

    /// <summary>
    /// ��ʼ�ؿ��ĳ���
    /// </summary>
    public IEnumerator SpawnLevelEnemies(LevelSO levelData)
    {
        timer = Defines.waitTime; //�غϿ�ʼǰ׼��ʱ��
        //�ȴ�ʱ��
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        ++nowWave;
        //����ʱ��
        while (nowWave <= totalWave)
        {
            nowEnemyNum = 0;
            float totalTime = SpawnWaveEnemies(levelData.GetWaveInfo(nowWave));
            UIManager.Instance.ShowTipInfo($"��{nowWave}��");
            UIManager.Instance.GetPanel<TowerPanel>().UpdateWaveInfo(nowWave, totalWave);
            timer = totalTime;
            while (timer > 0)
            {
                timer -= Time.deltaTime; //�ȴ�ȫ���������
                yield return null;
            }

            while (nowEnemyNum != 0)
            {
                yield return null; //ֻ��ȫ������ɱ�꣬���ܽ�����һ��
            }
            ++nowWave;
        }
        nowWave = totalWave;
        yield return new WaitForSeconds(1f);
        //��ʾʤ�����
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Fight)
            WinFight(); //ս��ʤ��
        else if (PlayerStateManager.Instance.CurrentState == PlayerState.Boss)
            WinGame(); //��Ϸʤ��
    }

    /// <summary>
    /// ��һ������
    /// </summary>
    /// <param name="waveInfo">������Ϣ</param>
    /// <returns>����ȫ����������Ҫ��ʱ��</returns>
    private float SpawnWaveEnemies(WaveInfo waveInfo)
    {
        float totalTime = 0;
        foreach (SpawnInfo info in waveInfo.spawnInfos)
        {
            totalTime = Mathf.Max(totalTime,info.GetTotalTime()); //��ȡ��ĳ���ʱ��
            AddEnemyNum(info.totalNum);
            EnemyManager.Instance.SpawnEnemies(info.enemyName, info.totalNum, info.spawnNum, info.frequency, info.delayTime);
        }
        return totalTime;
    }

    /// <summary>
    /// ������ǰ����
    /// </summary>
    public void SkipThisWave()
    {
        if (nowWave <= totalWave)
        {
            //����������е���
            EnemyManager.Instance.Clear();
            //����������0
            timer = 0;
            nowEnemyNum = 0; //��0ʱ�Զ�������һ��
            UIManager.Instance.GetPanel<TowerPanel>().UpdateEnemyNum(nowEnemyNum);
        }
    }

    /// <summary>
    /// ���ӹ�������
    /// </summary>
    private void AddEnemyNum(int num)
    {
        nowEnemyNum += num;
        UIManager.Instance.GetPanel<TowerPanel>().UpdateEnemyNum(nowEnemyNum);
    }

    /// <summary>
    /// ���ٹ�������
    /// </summary>
    public void SubEnemyNum()
    {
        --nowEnemyNum;
        UIManager.Instance.GetPanel<TowerPanel>().UpdateEnemyNum(nowEnemyNum);
    }

    /// <summary>
    /// ����ս��
    /// </summary>
    public void Clear()
    {
        isInLevel = false;
        nowWave = totalWave = 0;
        timer = 0;
        nowEnemyNum = 0;
        StopAllCoroutines();
    }

    public void WinFight()
    {
        //����ս��
        isInLevel = false;
        Clear();
        TowerManager.Instance.Clear();
        EnemyManager.Instance.Clear();
        PoolMgr.Instance.ClearPool(); //�л�����ǰ����������

        //��ʾ���
        UIManager.Instance.LoadScene("MapScene", () =>
        {
            UIManager.Instance.HidePanel<TowerPanel>();
            RewardPanel rewardPanel = UIManager.Instance.ShowPanel<RewardPanel>();
            rewardPanel.SetReward(currentLevel.rewardDatas);
        });
    }

    public void WinGame()
    {
        UIManager.Instance.HidePanel<TowerPanel>();
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }

    public void LoseGame()
    {
        UIManager.Instance.HidePanel<TowerPanel>();
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }
}
