using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    //场景上的Canvas
    private Transform canvasTrans;

    //存储面板的容器
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    
    //存储UI物体的容器
    private List<GameObject> UIObjList = new List<GameObject>();

    private UIManager()
    {
        //创建Canvas
        GameObject canvasObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvasObj.transform;
        //过场景不删除
        GameObject.DontDestroyOnLoad(canvasObj);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>对应面板脚本</returns>
    public T ShowPanel<T>() where T : BasePanel
    {
        //面板名字
        string panelName = typeof(T).Name;
        //如果已经创建了，就直接返回面板
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //加载面板对象
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
        //设置父对象
        panelObj.transform.SetParent(canvasTrans, false);
        //获取面板脚本
        T panel = panelObj.GetComponent<T>();
        //执行显示方法
        panel.ShowMe();
        //添加进容器中
        panelDic.Add(panelName, panel);
        //返回面板对象
        return panel;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="isFade">是否淡出（默认为true,淡出）</param>
    /// <param name="callback">回调函数</param>
    public void HidePanel<T>(bool isFade = true, UnityAction callback = null) where T : BasePanel
    {
        //面板名字
        string panelName = typeof(T).Name;

        if (panelDic.ContainsKey(panelName))
        {
            //获取面板
            T panel = panelDic[panelName] as T;
            if (isFade)
            {
                panel.HideMe(() =>
                {
                    callback?.Invoke();
                    //移除自己
                    GameObject.Destroy(panel.gameObject);
                    //移除容器中的数据
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                callback?.Invoke();
                //直接移除自己
                GameObject.Destroy(panel.gameObject);
                //移除容器中的数据
                panelDic.Remove(panelName);
            }
        }
    }

    /// <summary>
    /// 得到面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>对应面板</returns>
    public T GetPanel<T>() where T : BasePanel
    {
        //面板名字
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }

    /// <summary>
    /// 创建UI物体
    /// </summary>
    /// <param name="resName">资源路径名</param>
    /// <param name="parentTrans">父对象的变换组件</param>
    /// <returns></returns>
    public GameObject CreateUIObj(string resName,Transform parentTrans)
    {
        GameObject UIObj = GameObject.Instantiate(Resources.Load<GameObject>(resName)); //从对象池中获取对象
        UIObj.transform.SetParent(parentTrans, false);
        UIObj.transform.SetAsLastSibling(); //设置在父级的最后一层
        UIObjList.Add(UIObj);
        return UIObj;
    }

    /// <summary>
    /// 删除UI物体
    /// </summary>
    /// <param name="UIObj">要删除的UI物体</param>
    public void DestroyUIObj(GameObject UIObj)
    {
        if (UIObjList.Contains(UIObj))
        {
            UIObjList.Remove(UIObj);
            GameObject.Destroy(UIObj);
        }
    }
}
