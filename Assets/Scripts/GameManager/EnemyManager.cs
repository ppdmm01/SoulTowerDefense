using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ���������
/// </summary>
public class EnemyManager : SingletonMono<EnemyManager>
{
    [Header("ƫ�ƾ���")]
    [Tooltip("���ɵ�����Ļ�߽�ľ���")]
    public float spawnOffset = 2f;
    private float left, right, top, bottom; //���������ĸ�����ı߽�

    private EnemyManagerSO data; //�洢���е�������
    private List<GameObject> enemies; //�洢����

    protected override void Awake()
    {
        base.Awake();
        enemies = new List<GameObject>();
        if (data == null)
        {
            data = Resources.Load<EnemyManagerSO>("Data/EnemyManagerSO");
            if (data == null)
                Debug.LogError("����EnemyManagerSOʧ�ܣ�");
        }
    }

    private void Start()
    {
        float verticalSize = Camera.main.orthographicSize; //��ֱ�����С
        float horizontalSize = verticalSize * Camera.main.aspect; //ˮƽ�����С
        //����߽�ֵ  
        left = Camera.main.transform.position.x - horizontalSize - spawnOffset;
        right = Camera.main.transform.position.x + horizontalSize + spawnOffset;
        bottom = Camera.main.transform.position.y - verticalSize - spawnOffset;
        top = Camera.main.transform.position.y + verticalSize + spawnOffset;
    }

    /// <summary>
    /// ����һ�ε���
    /// </summary>
    /// <param name="enemyName">��������</param>
    /// <param name="totalNum">��������</param>
    /// <param name="spawnNum">һ�����ɵĵ���</param>
    /// <param name="frequency">����Ƶ�ʣ��������һ�Σ�</param>
    /// <param name="delayTime">��ʱ��òų���</param>
    public void SpawnEnemies(string enemyName,int totalNum,int spawnNum,float frequency, float delayTime)
    {
        StartCoroutine(SpawnEnemiesRoutine(enemyName,totalNum, spawnNum, frequency, delayTime));
    }

    private IEnumerator SpawnEnemiesRoutine(string enemyName, int totalNum, int spawnNum, float frequency,float delayTime)
    {
        yield return new WaitForSeconds(delayTime); //��ʱ

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

    /// <summary>
    /// ����ǰ���е���
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
    /// ����ս���ϵĵ���
    /// </summary>
    public void Clear()
    {
        KillAllEnemies();
        StopAllCoroutines(); //ֹͣ��������
    }
}
