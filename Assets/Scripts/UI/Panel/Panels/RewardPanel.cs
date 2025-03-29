using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : BasePanel
{
    public Button closeBtn;
    public Transform btnContainer; //◊∞Ω±¿¯∞¥≈•µƒ»›∆˜

    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<RewardPanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
        });
    }

    //…Ë÷√Ω±¿¯
    public void SetReward(List<RewardData> rewards)
    {
        foreach (RewardData reward in rewards)
        {
            GameObject rewardObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/RewardBtn"));
            rewardObj.transform.SetParent(btnContainer, false);
            RewardBtn btn = rewardObj.GetComponent<RewardBtn>();
            btn.SetBtn(reward);
        }
    }
}
