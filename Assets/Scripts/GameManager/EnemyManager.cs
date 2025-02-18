using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物管理器
/// </summary>
public class EnemyManager : SingletonMono<EnemyManager>
{
    [Header("偏移距离")]
    [Tooltip("生成点离屏幕边界的距离")]
    public float spawnOffset = 2f;

    private List<GameObject> enemies; //存储敌人

    private float left,right,top,bottom; //上下左右四个方向的边界

    private void Start()
    {
        enemies = new List<GameObject>();

        float verticalSize = Camera.main.orthographicSize; //垂直方向大小
        float horizontalSize = verticalSize * Camera.main.aspect; //水平方向大小
        //计算边界值  
        left = Camera.main.transform.position.x - horizontalSize - spawnOffset;
        right = Camera.main.transform.position.x + horizontalSize + spawnOffset;
        bottom = Camera.main.transform.position.y - verticalSize - spawnOffset;
        top = Camera.main.transform.position.y + verticalSize + spawnOffset;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnWaveEnemies("Enemy1", 20, 5, 0.5f);
        }
    }

    /// <summary>
    /// 生成一波敌人
    /// </summary>
    /// <param name="enemyName">敌人名字</param>
    /// <param name="totalNum">生成数量</param>
    /// <param name="spawnNum">一次生成的敌人</param>
    /// <param name="frequency">生成频率（多久生成一次）</param>
    public void SpawnWaveEnemies(string enemyName,int totalNum,int spawnNum,float frequency)
    {
        StartCoroutine(ReallySpawnWaveEnemies(enemyName,totalNum, spawnNum, frequency));
    }

    private IEnumerator ReallySpawnWaveEnemies(string enemyName, int totalNum, int spawnNum, float frequency)
    {
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
            //Vector2 pos = RandomSpawnPoint();
            //GameObject enemy = Instantiate(Resources.Load<GameObject>("Enemy/"+enemyName), pos, Quaternion.identity);
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
}
