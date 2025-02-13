using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    //�����ϵ�Canvas
    private Transform canvasTrans;

    //�洢��������
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

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
}
