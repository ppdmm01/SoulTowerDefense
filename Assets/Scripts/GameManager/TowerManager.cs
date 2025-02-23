using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

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
    private BaseTower target; //��ǰ���õ�Ŀ�������
    private BaseTower nowTower; //��ǰ��ⷶΧ�ķ�����

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
        TowerRangeOperation();
        PlaceTowerOperation();
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="towerName">����������</param>
    public void CreateTower(string towerName)
    {
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
        TowerData towerData = new TowerData(GetTowerSO_ByName("Core"));
        core.Init(towerData);

        //ʹ��
        core.isUsed = true;
        //��¼
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
            target.ShowHpBar();
            target.towerCollider.isTrigger = false;
            target.isUsed = true;
            isPlacing = false;
            target.HideRange();
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

    /// <summary>
    /// ����������Χ�Ĳ���
    /// </summary>
    private void TowerRangeOperation()
    {
        if (isPlacing) return; //������ڷ��÷���������Ҫ��ʾ�����ķ�������Χ

        // �����λ�ô���Ļ����ת��Ϊ��������  
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �������Ƿ���ͣ�ڷ�����������  
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, 1 << LayerMask.GetMask("Tower"));
        if (hit.collider != null)
        {
            if (nowTower != hit.collider.GetComponent<BaseTower>())
            {
                if (nowTower != null)
                    nowTower.HideRange(); //��֮ǰ����Χ������
                nowTower = hit.collider.GetComponent<BaseTower>();
            }
            if (nowTower != null)
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
    /// �޸�ָ������������
    /// </summary>
    /// <param name="towerName">����������</param>
    /// <param name="activeEffect">����Ч��</param>
    public void SetTowerDataFromName(string towerName, ItemActiveEffect[] activeEffects)
    {
        if (!towerDatas.ContainsKey(towerName)) 
        {
            Debug.Log("δ�ҵ���Ϊ"+towerName+"�ķ�����");
            return;
        }

        TowerData data = towerDatas[towerName];
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
        foreach (TowerData data in towerDatas.Values)
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
