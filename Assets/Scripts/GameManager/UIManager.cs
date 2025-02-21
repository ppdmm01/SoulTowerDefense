using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    //�����ϵ�Canvas
    public Transform canvasTrans;

    //�洢��������
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    
    //�洢UI���������
    private List<GameObject> UIObjList = new List<GameObject>();

    private UIManager()
    {
        //����Canvas
        GameObject canvasObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvasObj.transform;
        //��������ɾ��
        GameObject.DontDestroyOnLoad(canvasObj);
    }

    /// <summary>
    /// ��ʾ���
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <returns>��Ӧ���ű�</returns>
    public T ShowPanel<T>() where T : BasePanel
    {
        //�������
        string panelName = typeof(T).Name;
        //����Ѿ������ˣ���ֱ�ӷ������
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //����������
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
        //���ø�����
        panelObj.transform.SetParent(canvasTrans, false);
        //��ȡ���ű�
        T panel = panelObj.GetComponent<T>();
        //ִ����ʾ����
        panel.ShowMe();
        //��ӽ�������
        panelDic.Add(panelName, panel);
        //����������
        return panel;
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <param name="isFade">�Ƿ񵭳���Ĭ��Ϊtrue,������</param>
    /// <param name="callback">�ص�����</param>
    public void HidePanel<T>(bool isFade = true, UnityAction callback = null) where T : BasePanel
    {
        //�������
        string panelName = typeof(T).Name;

        if (panelDic.ContainsKey(panelName))
        {
            //��ȡ���
            T panel = panelDic[panelName] as T;
            if (isFade)
            {
                panel.HideMe(() =>
                {
                    callback?.Invoke();
                    //�Ƴ��Լ�
                    GameObject.Destroy(panel.gameObject);
                    //�Ƴ������е�����
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                callback?.Invoke();
                //ֱ���Ƴ��Լ�
                GameObject.Destroy(panel.gameObject);
                //�Ƴ������е�����
                panelDic.Remove(panelName);
            }
        }
    }

    /// <summary>
    /// �õ����
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <returns>��Ӧ���</returns>
    public T GetPanel<T>() where T : BasePanel
    {
        //�������
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }

    /// <summary>
    /// ͨ������ش���UI����
    /// </summary>
    /// <param name="resName">��Դ·����</param>
    /// <returns>����</returns>
    public GameObject CreateUIObjByPoolMgr(string resName)
    {
        GameObject UIObj = PoolMgr.Instance.GetUIObj(resName); //�Ӷ�����л�ȡ����
        UIObjList.Add(UIObj);
        return UIObj;
    }

    /// <summary>
    /// ����UI����
    /// </summary>
    /// <param name="resName">��Դ·����</param>
    /// <returns>����</returns>
    public GameObject CreateUIObj(string resName,Transform parent)
    {
        GameObject UIObj = GameObject.Instantiate(Resources.Load<GameObject>(resName),parent); //�Ӷ�����л�ȡ����
        UIObj.transform.SetAsLastSibling();
        UIObjList.Add(UIObj);
        return UIObj;
    }

    /// <summary>
    /// ͨ�������ɾ��UI����
    /// </summary>
    /// <param name="UIObj">Ҫɾ����UI����</param>
    public void DestroyUIObjByPoolMgr(GameObject UIObj)
    {
        if (UIObjList.Contains(UIObj))
        {
            UIObjList.Remove(UIObj);
            PoolMgr.Instance.PushUIObj(UIObj);
        }
    }

    /// <summary>
    /// ͨ�������ɾ��UI����
    /// </summary>
    /// <param name="UIObj">Ҫɾ����UI����</param>
    public void DestroyUIObj(GameObject UIObj)
    {
        if (UIObjList.Contains(UIObj))
        {
            UIObjList.Remove(UIObj);
            GameObject.Destroy(UIObj);
        }
    }

    /// <summary>
    /// ��ʾ�����ı�
    /// </summary>
    /// <param name="txt">�ı�</param>
    /// <param name="color">��ɫ</param>
    /// <param name="pos">λ��</param>
    public void ShowTxtPopup(string txt,Color color, Vector2 pos)
    {
        GameObject obj = CreateUIObjByPoolMgr("UI/Popup/TxtPopup");
        UIPopup txtPopup = obj.GetComponent<UIPopup>();
        txtPopup.Init(txt, color, pos);
    }

    /// <summary>
    /// ��������UI���壨������ʱʹ�ã�
    /// </summary>
    public void ClearAllUIObj()
    {
        foreach (GameObject UIObj in UIObjList)
        {
            GameObject.Destroy(UIObj);
        }
        UIObjList.Clear();
    }
}
