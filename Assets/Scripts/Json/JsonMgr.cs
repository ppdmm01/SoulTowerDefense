using LitJson;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//��ȡ���������ͣ��ɷ������һ��Ǳ����ɣ�
public enum JsonType
{
    JsonUtility,
    LitJson,
}

//Json������
public class JsonMgr 
{
    private static JsonMgr instance = new JsonMgr();   
    public static JsonMgr Instance => instance; 
    private JsonMgr() { }

    //��������
    public void SaveData(object data,string fileName/*,JsonType type = JsonType.LitJson*/)
    {
        //·��
        string path = Application.persistentDataPath + "/" + fileName + ".json";

        //���л�
        string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        //string jsonStr = "";
        //switch (type)
        //{
        //    case JsonType.JsonUtility:
        //        jsonStr = JsonUtility.ToJson(data);
        //        break;
        //    case JsonType.LitJson:
        //        jsonStr = JsonMapper.ToJson(data);
        //        break;
        //}

        //д���ļ�
        File.WriteAllText(path, jsonStr);
    }

    //��ȡ����
    public T LoadData<T>(string fileName/*,JsonType type = JsonType.LitJson*/) where T : new()
    {
        //·��
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        if (!File.Exists(path))
        {
            path = Application.streamingAssetsPath + "/" + fileName + ".json";
            if (!File.Exists(path))
            {
                return new T();
            }
        }

        //��ȡ�ļ�
        string jsonStr = File.ReadAllText(path);

        //�����л� 
        //T data = default(T);
        //switch (type)
        //{
        //    case JsonType.JsonUtility:
        //        data = JsonUtility.FromJson<T>(jsonStr);
        //        break;
        //    case JsonType.LitJson:
        //        data = JsonMapper.ToObject<T>(jsonStr);
        //        break;
        //}
        T data = JsonConvert.DeserializeObject<T>(jsonStr);

        return data;
    }
}
