using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    public Button sureBtn;
    public override void Init()
    {
        sureBtn.onClick.AddListener(() =>
        {
            //�ص����˵�
            UIManager.Instance.LoadScene("BeginScene", () =>
            {
                UIManager.Instance.HidePanel<GameOverPanel>();
                UIManager.Instance.ShowPanel<BeginPanel>();
            });
            //����浵
            GameDataManager.Instance.ClearGameData();
        });
    }
}
