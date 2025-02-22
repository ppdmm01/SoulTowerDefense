using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// ��������������������������á���Ϣ�鿴���洢�ȣ�
/// </summary>
public class TowerManager : SingletonMono<TowerManager>
{
    [HideInInspector] public Core core; //��������

    [Header("���������ڲ�")]
    [SerializeField] private LayerMask towerLayer; //���������ڲ�

    private TowerManagerSO data; //�洢��������������
    private Dictionary<string, TowerSO> towerDataDic; //�洢��������������

    public Dictionary<string,TowerData> towers; //��¼Ŀǰѡ��ķ�������������

    private List<BaseTower> gameTowerList; //��¼���ϵķ�����

    [Header("�������������")]
    public bool isPlacing; //�Ƿ����������
    public BaseTower target; //���ڷ��õ�Ŀ��
    public List<GameObject> collisonTowerList; //��¼����ײ��Χ�ڵķ�����
    private BaseTower nowTower; //��ǰ���ķ�����

    protected override void Awake()
    {
        base.Awake();
        if (data == null)
        {
            data = Resources.Load<TowerManagerSO>("Data/TowerManagerSO");
            if (data == null)
                Debug.LogError("����TowerManagerSOʧ�ܣ�");
        }

        towerDataDic = new Dictionary<string, TowerSO>();
        foreach (TowerSO towerSO in data.towerSOList)
        {
            towerDataDic.Add(towerSO.towerName, towerSO);
        }
    }

    void Start()
    {
        towers = new Dictionary<string, TowerData>();
        gameTowerList = new List<BaseTower>();
        collisonTowerList = new List<GameObject>();
        nowTower = null;
    }

    void Update()
    {
        TowerRangeOperation();
        PlaceTowerOperation();
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="towerName"></param>
    public void CreateTower(string towerName)
    {
        GameObject towerObj = Instantiate(Resources.Load<GameObject>("Tower/" + towerName));
        BaseTower tower = towerObj.GetComponent<BaseTower>();
        gameTowerList.Add(tower);

        if (towerObj == null)
            Debug.LogError("�����������ڣ�");

        tower.towerCollider.isTrigger = true;
        tower.isUsed = false;
        isPlacing = true;
        target = tower;
    }

    /// <summary>
    /// �������غ���
    /// </summary>
    public void CreateCore()
    {
        GameObject coreObj = Instantiate(Resources.Load<GameObject>("Tower/Core"),Vector2.zero,Quaternion.identity);
        Core core = coreObj.GetComponent<Core>();
        if (coreObj == null)
            Debug.LogError("���Ĳ����ڣ�");

        //ʹ��
        core.isUsed = true;
        this.core = core;
    }

    /// <summary>
    /// ���÷�����
    /// </summary>
    public void PlaceTower()
    {
        if (HasResources())
        {
            GameResManager.Instance.AddQiNum(-target.data.cost); //������Դ

            //ʹ��
            target.SetHpBarPos(target.transform.position+Vector3.up);
            target.ShowHpBar();
            target.towerCollider.isTrigger = false;
            target.isUsed = true;
            isPlacing = false;
            target.HideRange();
            target = null;
        }
        else
        {
            Debug.Log("��Դ���㣬����ʧ�ܣ�");
            //CancelPlaceTower();
        }

        collisonTowerList.Clear(); //������ݣ�������һ�η�������ʱ����
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

    /// <summary>
    /// ����������Χ�Ĳ���
    /// </summary>
    private void TowerRangeOperation()
    {
        if (isPlacing) return; //������ڷ��÷���������Ҫ��ʾ�����ķ�������Χ

        // �����λ�ô���Ļ����ת��Ϊ��������  
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �������Ƿ���ͣ�ڷ�����������  
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, towerLayer);
        if (hit.collider != null)
        {
            if (nowTower != hit.collider.GetComponent<BaseTower>())
            {
                if (nowTower != null)
                    nowTower.HideRange(); //��֮ǰ����Χ������
                nowTower = hit.collider.GetComponent<BaseTower>();
            }
            nowTower.ShowRange();
        }
        else
        {
            if (nowTower != null)
            {
                nowTower.HideRange();
                nowTower = null;
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
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.transform.position = mousePos;
            //��ʾ��������Χ
            target.ShowRange();
            if (HasResources())
                target.SetRangeColor(Defines.validRangeColor);
            else
                target.SetRangeColor(Defines.invalidRangeColor);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("ȡ������");
                CancelPlaceTower();
            }
        }
    }
    
    /// <summary>
    /// �Ƿ��з��ø÷���������Դ
    /// </summary>
    public bool HasResources()
    {
        return GameResManager.Instance.GetQiNum() >= target.data.cost;
    }

    /// <summary>
    /// �������з�����
    /// </summary>
    public void ClearAllTower()
    {
        foreach (BaseTower tower in gameTowerList)
        {
            Destroy(tower.gameObject);
        }
        gameTowerList.Clear();
    }

    /// <summary>
    /// ���Ҫ�õ��ķ�����
    /// </summary>
    public void AddTower(string towerName,TowerData data)
    {
        if (towers.ContainsKey(towerName)) return;
        towers.Add(towerName,data);
    }

    /// <summary>
    /// �Ƴ�Ҫ�õ�������
    /// </summary>
    /// <param name="towerName"></param>
    public void RemoveTower(string towerName)
    {
        if (towers.ContainsKey(towerName))
            towers.Remove(towerName);
    }

    /// <summary>
    /// ͨ�����ֻ�ȡ��������������
    /// </summary>
    /// <param name="towerName">����������</param>
    /// <returns>��������������</returns>
    public TowerSO GetTowerSO_ByName(string towerName)
    {
        if (!towerDataDic.ContainsKey(towerName)) return null;
        return towerDataDic[towerName];
    }

    /// <summary>
    /// �޸�ָ������������
    /// </summary>
    /// <param name="towerName">����������</param>
    /// <param name="activeEffect">����Ч��</param>
    public void SetTowerDataFromName(string towerName, ItemActiveEffect[] activeEffects)
    {
        if (!towers.ContainsKey(towerName)) 
        {
            Debug.Log("δ�ҵ���Ϊ"+towerName+"�ķ�����");
            return;
        }

        TowerData data = towers[towerName];
        foreach (ItemActiveEffect activeEffect in activeEffects)
        {
            switch (activeEffect.effectType)
            {
                case ItemActiveEffect.EffectType.Hp:
                    data.hp += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Cost:
                    data.cost += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Damage:
                    data.damage += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Range:
                    data.range += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Interval:
                    data.interval += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Output:
                    data.output += (int)activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Cooldown:
                    data.cooldown += activeEffect.value;
                    break;
            }
        }
    }

    /// <summary>
    /// ͨ����ǩͳһ�޸ķ���������
    /// </summary>
    /// <param name="tags">��ǩ</param>
    /// <param name="activeEffects">����Ч��</param>
    public void SetTowerDataFromTag(ItemTag[] tags, ItemActiveEffect[] activeEffects)
    {
        bool flag = true; //����Ƿ������ǩ����
        foreach (TowerData data in towers.Values)
        {
            flag = true;
            //ֻ�����б�ǩ�������
            foreach (ItemTag tag in tags)
                if (!data.itemTags.Contains(tag)) 
                    flag = false;

            if (flag)
                SetTowerDataFromName(data.towerName, activeEffects);
        }
    }
}
