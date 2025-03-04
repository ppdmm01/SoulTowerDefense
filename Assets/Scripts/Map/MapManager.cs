using System.Linq;
using System.Xml;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapConfig config; //��ͼ������Ϣ
    public MapView view; //��ͼ��ʾ��

    public Map CurrentMap { get; private set; } //��ǰ�ĵ�ͼ

    private void Start()
    {
        GenerateNewMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        Map map = MapGenerator.GetMap(config); //�������ñ���������ĵ�ͼ
        CurrentMap = map;
        view.ShowMap(map); //��ʾ��ͼ
    }
}
