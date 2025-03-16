using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : BasePanel
{
    public TextMeshProUGUI qiNumText; //“气”资源数量文本
    public TextMeshProUGUI waveNumText; //波数
    public TextMeshProUGUI nowEnemyNumText; //剩余敌人数量
    public Transform towerBtnContent; //放置按钮的位置
    private List<CreateTowerBtn> towerBtnList = new List<CreateTowerBtn>(); //存储放置防御塔的按钮
    public override void Init()
    {

    }

    /// <summary>
    /// 初始化防御塔放置按钮
    /// </summary>
    public void InitTowerBtn()
    {
        Debug.Log(TowerManager.Instance.towerDatas.Count);
        foreach (TowerData towerData in TowerManager.Instance.towerDatas.Values)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("UI/UIObj/CreateTowerBtn"));
            obj.transform.SetParent(towerBtnContent, false);
            CreateTowerBtn btn = obj.GetComponent<CreateTowerBtn>();
            btn.SetTower(towerData);
            towerBtnList.Add(btn);
        }
    }

    /// <summary>
    /// 摧毁所有放置防御塔按钮
    /// </summary>
    public void DestroyAllTowerBtn()
    {
        foreach (CreateTowerBtn btn in towerBtnList)
            Destroy(btn);
        towerBtnList.Clear();
    }

    /// <summary>
    /// 更新气资源数量
    /// </summary>
    /// <param name="num"></param>
    public void UpdateQiNum(int num)
    {
        qiNumText.text = num.ToString();    
    }

    /// <summary>
    /// 更新波数
    /// </summary>
    /// <param name="waveNum"></param>
    public void UpdateWaveInfo(int waveNum,int totalWaveNum)
    {
        waveNumText.text = $"第{waveNum}/{totalWaveNum}波";
    }

    /// <summary>
    /// 更新敌人数量
    /// </summary>
    /// <param name="num"></param>
    public void UpdateEnemyNum(int num)
    {
        nowEnemyNumText.text = num.ToString();
    }
}
