using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��������������������������á���Ϣ�鿴���洢�ȣ�
/// </summary>
public class TowerManager : SingletonMono<TowerManager>
{
    //���������������ݣ�����ʱ���������
    private TowerManagerSO data; //�洢��������������
    private Dictionary<string, TowerSO> towerSODic; //�洢��������������

    //��¼��̬�仯�ķ���������
    public Dictionary<string,TowerData> towerDatas; //��¼Ŀǰѡ��ķ�������������
    public Dictionary<string,TowerData> oldTowerDatas; //��¼��һ�α仯�����ݣ����ڼ������Ա仯��

    //��¼���ϵ���
    public Core core; //��¼���غ���
    public List<BaseTower> gameTowerList; //��¼���ϵķ�����

    //�������
    private bool isPlacing; //�Ƿ����������
    public bool isOpenPanel; //�Ƿ����ڴ򿪷������������
    private BaseTower target; //��ǰ���õ�Ŀ�������
    private BaseTower nowTower; //��ǰ��ⷶΧ�ķ�����

    private float operationOffsetTime = 0.1f; //�������ʱ��(��ֹ���������)
    private float timer; //��ʱ��

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<TowerManagerSO>("Data/TowerManagerSO");
            if (data == null)
                Debug.LogError("����TowerManagerSOʧ�ܣ�");
        }

        towerSODic = new Dictionary<string, TowerSO>();
        foreach (TowerSO towerSO in data.towerSOList)
        {
            towerSODic.Add(towerSO.towerName, towerSO);
        }
    }

    void Start()
    {
        towerDatas = new Dictionary<string, TowerData>();
        oldTowerDatas = new Dictionary<string, TowerData>();

        gameTowerList = new List<BaseTower>();
        nowTower = null;
    }

    void Update()
    {
        TowerRangeOperation(); //�鿴��������Χ����
        SellTowerOperation(); //��������������
        PlaceTowerOperation(); //���÷�����������������󣬱������ʱ��bool�仯Ӱ��ǰ�������
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="towerName">����������</param>
    public void CreateTower(string towerName)
    {
        if (isPlacing) return; //������ڷ��ã�������Ū��

        GameObject towerObj = Instantiate(Resources.Load<GameObject>("Tower/" + towerName));
        if (towerObj == null)
            Debug.LogError("�����������ڣ�");
        //��ʼ������
        BaseTower tower = towerObj.GetComponent<BaseTower>();
        tower.Init(towerDatas[towerName]);
        //�����У�δʹ��
        tower.towerCollider.isTrigger = true;
        tower.isUsed = false;
        isPlacing = true;
        target = tower;

        timer = 0;
    }

    /// <summary>
    /// �������غ���
    /// </summary>
    public void CreateCore()
    {
        GameObject coreObj = Instantiate(Resources.Load<GameObject>("Tower/Core"),Vector2.zero,Quaternion.identity);
        if (coreObj == null)
            Debug.LogError("���Ĳ����ڣ�");
        //��ʼ������
        Core core = coreObj.GetComponent<Core>();
        TowerData towerData = new TowerData(GetTowerSO_ByName("Core"),null);
        core.Init(towerData);

        //ʹ��
        core.isUsed = true;
        //��¼
        this.core = core;
    }

    #region �������
    /// <summary>
    /// ����������Χ�Ĳ���
    /// </summary>
    private void TowerRangeOperation()
    {
        if (isPlacing || isOpenPanel) return; //������ڷ��÷��������߲�������Ѵ򿪣�������

        // �����λ�ô���Ļ����ת��Ϊ��������  
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �������Ƿ���ͣ�ڷ�����������  
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tower"));
        if (hit.collider != null)
        {
            if (nowTower != hit.collider.GetComponent<BaseTower>()) //�����֮ǰ�ķ�������һ��
            {
                if (nowTower != null)
                    nowTower.HideRange(); //��֮ǰ����Χ������
                nowTower = hit.collider.GetComponent<BaseTower>();
            }
            if (nowTower != null)
                nowTower.ShowRange();
        }
        else if (nowTower != null)
        {
            nowTower.HideRange();
            nowTower = null;
        }
    }

    /// <summary>
    /// ��������������50%��
    /// </summary>
    private void SellTowerOperation()
    {
        if (isPlacing) return;

        //��������
        if (nowTower != null && core != nowTower) //���������ڲ��Ҳ��Ǻ���
        {
            if (Input.GetMouseButtonDown(0))
            {
                nowTower.HideRange();
                //ͬʱҲҪ��ʾѪ��
                nowTower.ShowHpBar();
                //��ʾ��������
                TowerOperationPanel panel = UIManager.Instance.ShowPanel<TowerOperationPanel>();
                panel.SetInfo(nowTower);
                isOpenPanel = true;
            }
        }
    }

    /// <summary>
    /// ���÷������Ĳ���
    /// </summary>
    private void PlaceTowerOperation()
    {
        if (isPlacing)
        {
            timer += Time.deltaTime;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.transform.position = mousePos;
            //��ʾ��������Χ
            target.ShowRange();
            if (HasResources())
                target.SetRangeColor(Defines.validRangeColor);
            else
                target.SetRangeColor(Defines.invalidRangeColor);

            if (Input.GetMouseButtonDown(0) && timer >= operationOffsetTime)
            {
                PlaceTower();
            }
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlaceTower();
            }
        }
    }


    /// <summary>
    /// ���÷�����
    /// </summary>
    public void PlaceTower()
    {
        if (HasResources())
        {
            GameResManager.Instance.AddSoulNum(-target.data.cost); //������Դ

            //ʹ��
            target.ShowHpBar();
            target.towerCollider.isTrigger = false;
            target.isUsed = true;
            isPlacing = false;
            target.HideRange();
            //�ƿ���Χ�ĵ���
            Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, 1.5f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    Vector2 forceDir = (collider.transform.position - target.transform.position).normalized;
                    collider.GetComponent<Rigidbody2D>().AddForce(forceDir*0.01f);
                }
            }
            //��¼
            gameTowerList.Add(target);
            target = null;
        }
        else
        {
            UIManager.Instance.ShowTipInfo("��Դ���㣬����ʧ�ܣ�");
        }
    }

    /// <summary>
    /// ȡ�����÷�����
    /// </summary>
    public void CancelPlaceTower()
    {
        isPlacing = false;
        if (target != null)
            Destroy(target.gameObject);
        target = null;
    }
    #endregion

    /// <summary>
    /// �Ƿ��з��ø÷���������Դ
    /// </summary>
    public bool HasResources()
    {
        return GameResManager.Instance.GetSoulNum() >= target.data.cost;
    }

    /// <summary>
    /// ���Ҫ�õ��ķ�����
    /// </summary>
    public void AddTowerData(string towerName,TowerData data)
    {
        if (towerDatas.ContainsKey(towerName)) return;
        towerDatas.Add(towerName,data);
    }

    /// <summary>
    /// �Ƴ�Ҫ�õ�������
    /// </summary>
    /// <param name="towerName"></param>
    public void RemoveTowerData(string towerName)
    {
        if (towerDatas.ContainsKey(towerName))
            towerDatas.Remove(towerName);
    }

    /// <summary>
    /// ͨ�����ֻ�ȡ��������������
    /// </summary>
    /// <param name="towerName">����������</param>
    /// <returns>��������������</returns>
    public TowerSO GetTowerSO_ByName(string towerName)
    {
        if (!towerSODic.ContainsKey(towerName)) return null;
        return towerSODic[towerName];
    }

    /// <summary>
    /// ��¼������
    /// </summary>
    public void RecordOldData()
    {
        //����������
        oldTowerDatas.Clear();
        foreach (string towerName in towerDatas.Keys)
        {
            TowerData oldData = new TowerData(towerDatas[towerName]);
            oldTowerDatas.Add(towerName, oldData);
        }
    }

    /// <summary>
    /// ����ս��
    /// </summary>
    public void Clear()
    {
        Debug.Log("����ս��");
        //�����̬����
        towerDatas.Clear();
        oldTowerDatas.Clear();
        //�������
        if (core != null)
            core.Dead();
        core = null;
        //������ϵķ�����
        foreach (BaseTower tower in gameTowerList.ToList())
            tower.Dead();
    }
}
