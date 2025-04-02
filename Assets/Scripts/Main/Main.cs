using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�����
/// </summary>
public class Main : MonoBehaviour
{
    // �ȵ㣨ָ��ĵ���㣬�������ĵ��λ�ã�һ������ΪͼƬ���Ļ����Ͻǣ�  
    public Vector2 hotspot = Vector2.zero;
    // ���ģʽ  
    public CursorMode cursorMode = CursorMode.Auto;
    void Start()
    {
        Texture2D cursorTexture = Resources.Load<Texture2D>("Cursor/cursor");
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        UIManager.Instance.ShowPanel<BeginPanel>();
        DOTween.Init();
        AudioManager.Instance.PlayBGM("BGM/Music2");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (LevelManager.Instance.isInLevel)
            {
                LevelManager.Instance.SkipThisWave();
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            BaseGrid grid = GridManager.Instance.GetGridByName("StorageBox");
            if (grid != null)
            {
                GridManager.Instance.AddRandomItem(3, grid);
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            BaseGrid grid = GridManager.Instance.GetGridByName("StorageBox");
            if (grid != null)
            {
                GridManager.Instance.ClearAllItem(grid);
            }
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Application.Quit(); //�˳���Ϸ
        //}
    }
}
