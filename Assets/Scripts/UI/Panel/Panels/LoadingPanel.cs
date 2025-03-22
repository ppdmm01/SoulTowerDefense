using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public float duration = 1f;
    private float timer;
    private UnityAction completedCallback; //����������ɺ�Ļص�
    private UnityAction fadeInCallback; //�������Ļص�������ʱ��
    private string sceneName;
    private AsyncOperation operation;

    public override void Init()
    {
        timer = 0;
    }

    /// <summary>
    /// ���س���
    /// </summary>
    /// <param name="sceneName">��������</param>
    /// <param name="fadeInCallback">������ɺ�Ļص�</param>
    /// <param name="completedCallback">����������ɺ�Ļص�</param>
    public void LoadScene(string sceneName,UnityAction fadeInCallback, UnityAction completedCallback)
    {
        this.sceneName = sceneName;
        this.completedCallback = completedCallback;
        this.fadeInCallback = fadeInCallback;

        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        operation.completed += (obj) =>
        {
            this.completedCallback?.Invoke(); //������ɣ����ûص�
            this.completedCallback = null;
            UIManager.Instance.HidePanel<LoadingPanel>();
        };

        timer = 0;
    }

    protected override void Update()
    {
        base.Update();
        //������ɲſ�ʼ��ʱ
        if (canvasGroup.alpha == 1)
        {
            fadeInCallback?.Invoke(); //������ɣ����ûص�
            fadeInCallback = null;
            timer += Time.deltaTime;
            if (timer > duration && operation.progress >= 0.9f)
            {
                timer = 0;
                operation.allowSceneActivation = true;
            }
        }
    }
}
