using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 怪物管理器
/// </summary>
public class EnemyManager : SingletonMono<EnemyManager>
{
    [Header("偏移距离")]
    [Tooltip("生成点离屏幕边界的距离")]
    public float spawnOffset = 2f;
    private float left, right, top, bottom; //上下左右四个方向的边界

    private EnemyManagerSO data; //存储所有敌人数据
    private List<GameObject> enemies; //存储敌人

    protected override void Awake()
    {
        base.Awake();
        enemies = new List<GameObject>();
        if (data == null)
        {
            data = Resources.Load<EnemyManagerSO>("Data/EnemyManagerSO");
            if (data == null)
                Debug.LogError("加载EnemyManagerSO失败！");
        }
    }

    private void Start()
    {
        float verticalSize = Camera.main.orthographicSize; //垂直方向大小
        float horizontalSize = verticalSize * Camera.main.aspect; //水平方向大小
        //计算边界值  
        left = Camera.main.transform.position.x - horizontalSize - spawnOffset;
        right = Camera.main.transform.position.x + horizontalSize + spawnOffset;
        bottom = Camera.main.transform.position.y - verticalSize - spawnOffset;
        top = Camera.main.transform.position.y + verticalSize + spawnOffset;
    }

    /// <summary>
    /// 生成一次敌人
    /// </summary>
    /// <param name="enemyName">敌人名字</param>
    /// <param name="totalNum">生成数量</param>
    /// <param name="spawnNum">一次生成的敌人</param>
    /// <param name="frequency">生成频率（多久生成一次）</param>
    /// <param name="delayTime">延时多久才出怪</param>
    public void SpawnEnemies(string enemyName,int totalNum,int spawnNum,float frequency, float delayTime)
    {
        StartCoroutine(SpawnEnemiesRoutine(enemyName,totalNum, spawnNum, frequency, delayTime));
    }

    private IEnumerator SpawnEnemiesRoutine(string enemyName, int totalNum, int spawnNum, float frequency,float delayTime)
    {
        yield return new WaitForSeconds(delayTime); //延时

        GameObject enemyObj = Resources.Load<GameObject>("Enemy/"+enemyName);
        float timer = 0;
        int nowNum = totalNum;
        while (nowNum > 0)
        {
            timer += Time.deltaTime;
            if (timer >= frequency)
            {
                timer = 0;
                if (nowNum < spawnNum)
                {
                    CreateEnemies(enemyName, nowNum);
                    nowNum = 0;
                }
                else
                {
                    CreateEnemies(enemyName, spawnNum);
                    nowNum -= spawnNum;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 创建敌人
    /// </summary>
    /// <param name="prefab">敌人预制体</param>
    /// <param name="num">生成数量</param>
    private void CreateEnemies(string enemyName, int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject enemy = PoolMgr.Instance.GetObj("Enemy/" + enemyName);
            enemy.transform.position = RandomSpawnPoint();
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// 随机获得生成点
    /// </summary>
    /// <returns></returns>
    private Vector2 RandomSpawnPoint()
    {
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0:
                return new Vector2(left, Random.Range(bottom, top)); //左
            case 1:
                return new Vector2(right, Random.Range(bottom, top)); //右  
            case 2:
                return new Vector2(Random.Range(left, right), top);   //上 
            default:
                return new Vector2(Random.Range(left, right), bottom); //下 

        }
    }

    /// <summary>
    /// 清理当前所有敌人
    /// </summary>
    private void KillAllEnemies()
    {
        foreach (GameObject enemyObj in enemies)
        {
            enemyObj.GetComponent<Enemy>().Dead();
        }
        enemies.Clear();
    }

    /// <summary>
    /// 清理战场上的敌人
    /// </summary>
    public void Clear()
    {
        KillAllEnemies();
        StopAllCoroutines(); //停止创建敌人
    }
}
