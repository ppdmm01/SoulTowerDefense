using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPanel : BasePanel
{
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI InfoTxt;
    public RectTransform panelTrans;

    public override void Init()
    {
        
    }

    protected override void Update()
    {
        base.Update();
        panelTrans.position = Input.mousePosition;
        panelTrans.position -= new Vector3(panelTrans.sizeDelta.x/4,0,0);
    }

    public void SetInfo(string name,string info)
    {
        nameTxt.text = name;
        InfoTxt.text = info;
    }
}
