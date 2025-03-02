using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//读取方法的类型
public enum JsonType
{
    JsonUtility,
    LitJson,
}

//Json管理类
public class JsonMgr 
{
    private static JsonMgr instance = new JsonMgr();   
    public static JsonMgr Instance => instance; 
    private JsonMgr() { }

    //保存数据
    public void SaveData(object data,string fileName,JsonType type = JsonType.LitJson)
    {
        //路径
        string path = Application.persistentDataPath + "/" + fileName + ".json";

        //序列化
        string jsonStr = "";
        switch (type)
        {
            case JsonType.JsonUtility:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);
                break;
        }

        //写入文件
        File.WriteAllText(path, jsonStr);
    }

    //读取数据
    public T LoadData<T>(string fileName,JsonType type = JsonType.LitJson) where T : new()
    {
        //路径
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        if (!File.Exists(path))
        {
            path = Application.streamingAssetsPath + "/" + fileName + ".json";
            if (!File.Exists(path))
            {
                return new T();
            }
        }

        //读取文件
        string jsonStr = File.ReadAllText(path);

        //反序列化 
        T data = default(T);
        switch (type)
        {
            case JsonType.JsonUtility:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
        }

        return data;
    }
}
