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
    public Core core; //��������(����ͨ����̬���ص���ʽ)

    [SerializeField] private LayerMask towerLayer; //���������ڲ�
    private BaseTower nowTower; //��ǰ���ķ�����

    public List<BaseTower> towerList; //�������б�

    [Header("�������������")]
    public bool isPlacing; //�Ƿ����������
    public BaseTower target; //���ڷ��õ�Ŀ��
    public List<GameObject> collisonTowerList; //��¼����ײ��Χ�ڵķ�����

    void Start()
    {
        towerList = new List<BaseTower>();
        collisonTowerList = new List<GameObject>();
        nowTower = null;

        //��������
        CreateCore();
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
        towerList.Add(tower);

        if (towerObj == null)
            Debug.LogError("�����������ڣ�");

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

        core.isUsed = true;
        this.core = core;
    }

    /// <summary>
    /// ���÷�����
    /// </summary>
    public void PlaceTower()
    {
        if (CanPlace())
        {
            if (HasResources())
            {
                GameResManager.Instance.AddQiNum(-target.data.cost); //������Դ
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
        else
        { 
            Debug.Log("λ�ò��㣬����ʧ�ܣ�");
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
            if (CanPlace() && HasResources())
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
    /// �Ƿ���Է��÷�����
    /// </summary>
    public bool CanPlace()
    {
        return collisonTowerList.Count <= 0;
    }
    
    /// <summary>
    /// �Ƿ��з��ø÷���������Դ
    /// </summary>
    public bool HasResources()
    {
        return GameResManager.Instance.GetQiNum() >= target.data.cost;
    }
}
