using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : BasePanel
{
    public TextMeshProUGUI title; //标题
    public Button closeBtn;
    public Transform btnContainer; //装奖励按钮的容器

    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<RewardPanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
        });
    }

    //设置奖励
    public void SetReward(string title,List<RewardData> rewards)
    {
        this.title.text = title;
        foreach (RewardData reward in rewards)
        {
            GameObject rewardObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/RewardBtn"));
            rewardObj.transform.SetParent(btnContainer, false);
            RewardBtn btn = rewardObj.GetComponent<RewardBtn>();
            btn.SetBtn(reward);
        }
    }
}
