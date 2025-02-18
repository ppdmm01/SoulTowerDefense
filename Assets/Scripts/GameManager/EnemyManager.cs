using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������
/// </summary>
public class EnemyManager : SingletonMono<EnemyManager>
{
    [Header("ƫ�ƾ���")]
    [Tooltip("���ɵ�����Ļ�߽�ľ���")]
    public float spawnOffset = 2f;

    private List<GameObject> enemies; //�洢����

    private float left,right,top,bottom; //���������ĸ�����ı߽�

    private void Start()
    {
        enemies = new List<GameObject>();

        float verticalSize = Camera.main.orthographicSize; //��ֱ�����С
        float horizontalSize = verticalSize * Camera.main.aspect; //ˮƽ�����С
        //����߽�ֵ  
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
    /// ����һ������
    /// </summary>
    /// <param name="enemyName">��������</param>
    /// <param name="totalNum">��������</param>
    /// <param name="spawnNum">һ�����ɵĵ���</param>
    /// <param name="frequency">����Ƶ�ʣ��������һ�Σ�</param>
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
    /// ��������
    /// </summary>
    /// <param name="prefab">����Ԥ����</param>
    /// <param name="num">��������</param>
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
    /// ���������ɵ�
    /// </summary>
    /// <returns></returns>
    private Vector2 RandomSpawnPoint()
    {
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0:
                return new Vector2(left, Random.Range(bottom, top)); //��
            case 1:
                return new Vector2(right, Random.Range(bottom, top)); //��  
            case 2:
                return new Vector2(Random.Range(left, right), top);   //�� 
            default:
                return new Vector2(Random.Range(left, right), bottom); //�� 

        }
    }
}
