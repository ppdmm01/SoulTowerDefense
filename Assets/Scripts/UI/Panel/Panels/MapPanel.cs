using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPanel : BasePanel
{
    public override void Init()
    {
        //���¶�����
        TopColumnPanel panel = UIManager.Instance.ShowPanel<TopColumnPanel>();
        panel.transform.SetAsLastSibling();
        if (panel != null)
        {
            panel.ShowBtn(TopColumnBtnType.Book,TopColumnBtnType.Crystal,TopColumnBtnType.Menu);
        }
        panel.SetTitle("��ͼ");

        PlayerStateManager.Instance.ChangeState(PlayerState.Map);
    }
}
