using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private LevelSO currentLevelData; //��ǰ�ؿ�

    private int nowWave; //��ǰ����
    private int totalWave; //�ܲ���
    private float timer = 0; //��ʱ��

    private int nowEnemyNum; //��ǰ��������

    private bool isQuit; //�Ƿ��˳�

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<LevelManagerSO>("Data/LevelManagerSO");
            if (data == null)
                Debug.LogError("����LevelManagerSOʧ�ܣ�");
        }
        isQuit = false;
    }

    /// <summary>
    /// ����һ���ؿ�
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
            //�ؿ�������ɣ���ʾս����壬��ʼ����Դ����ʼ���ֵ��߼�
            isInLevel = true;
            //������������ť
            UIManager.Instance.ShowPanel<TowerPanel>().InitTowerBtn();
            //����Ԫ��ˮ��
            TowerManager.Instance.CreateCore();
            //��ʼ����Դ
            GameResManager.Instance.ResetSoulNum(); //�ȹ���
            GameResManager.Instance.AddSoulNum(100 + mapLayer*5);
            //��¼������Ϣ
            LevelSO levelData = GetLevelData(mapLayer); //��ȡ�ؿ���Ϣ
            currentLevelData = levelData;
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

    //��ȡһ���ؿ�
    public LevelSO GetLevelData(int mapLayer)
    {
        int level = (mapLayer+1) / 2; //��ȡ�ؿ��ȼ�
        List<LevelSO> list = data.levelSOList.Where(levelData => levelData.level == level).ToList(); //��ȡͬ�ȼ��Ĺؿ�
        LevelSO levelData = list.Random();
        Debug.Log("level:" + levelData.level + " name:" + levelData.name);
        return levelData;
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
        UIManager.Instance.GetPanel<TowerPanel>()?.UpdateEnemyNum(nowEnemyNum);
    }

    /// <summary>
    /// ���ٹ�������
    /// </summary>
    public void SubEnemyNum()
    {
        --nowEnemyNum;
        UIManager.Instance.GetPanel<TowerPanel>()?.UpdateEnemyNum(nowEnemyNum);
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
        TowerManager.Instance.Clear();
        EnemyManager.Instance.Clear();
        PoolMgr.Instance.ClearPool(); //�л�����ǰ����������
        StopAllCoroutines();
    }

    public void WinFight()
    {
        //����ս��
        Clear();

        //��ʾ���
        UIManager.Instance.LoadScene("MapScene", () =>
        {
            UIManager.Instance.HidePanel<TowerPanel>();
            RewardPanel rewardPanel = UIManager.Instance.ShowPanel<RewardPanel>();
            rewardPanel.SetReward("ս��Ʒ",currentLevelData.rewardDatas);
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
        //if (PlayerStateManager.Instance.CurrentState == PlayerState.Menu) return; //����ǲ˵��˳����µģ������
        UIManager.Instance.HidePanel<TowerPanel>();
        GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();
        panel.SetTitle(false);
        GameResManager.Instance.ResetCoreHp();
        GameResManager.Instance.ResetTaixuNum();
    }

    private void OnApplicationQuit()
    {
        //ս��;���˳������˻ص���һ���ڵ�
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
        //ս��;���˳������˻ص���һ���ڵ㣨ֱ���˳���Ϸʱ�ᴥ��OnApplicationQuit�͸÷��������Ա����ظ�������
        if (isInLevel && !isQuit)
        {
            if (PlayerStateManager.Instance.CurrentState == PlayerState.Fight ||
            PlayerStateManager.Instance.CurrentState == PlayerState.Boss ||
            PlayerStateManager.Instance.CurrentState == PlayerState.Menu)
            {
                Debug.Log("�˻ؽڵ�");
                Map currentMap = GameDataManager.Instance.mapData;
                if (currentMap != null && currentMap.path.Count >= 1)
                    currentMap.path.RemoveAt(currentMap.path.Count - 1);
                GameDataManager.Instance.SaveMapData(currentMap);
            }
        }
    }
}
