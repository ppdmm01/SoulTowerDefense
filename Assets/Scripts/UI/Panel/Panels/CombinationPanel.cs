using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CombinationPanel : BasePanel
{
    public Button closeBtn;
    public ScrollRect sr; //放置组合信息的容器
    private List<GameObject> combinationInfoList = new List<GameObject>();
    
    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<CombinationPanel>();
        });
    }

    /// <summary>
    /// 更新组合信息
    /// </summary>
    public void UpdateCombinationInfo()
    {
        ClearCombinationInfos();
        foreach (CombinationSO combination in CombinationManager.Instance.GetNowCombinations())  
        {
            GameObject combinationObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/CombinationInfo"));
            combinationObj.transform.SetParent(sr.content, false);
            CombinationInfo info = combinationObj.GetComponent<CombinationInfo>();
            info.SetInfo(combination.combinationName,combination.activeAttribute);
            combinationInfoList.Add(combinationObj);
        }
    }

    /// <summary>
    /// 清理组合信息
    /// </summary>
    public void ClearCombinationInfos()
    {
        foreach (GameObject combination in combinationInfoList)
        {
            Destroy(combination);
        }
        combinationInfoList.Clear();
    }

    private void OnDestroy()
    {
        foreach (GameObject combinationObj in combinationInfoList)
            Destroy(combinationObj);
        combinationInfoList.Clear();
    }
}
