using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System;
public class BaseVO
{

    protected JSONNode data;

    public JSONNode Data
    {
        get
        {
            return data;
        }
    }

    //static Dictionary<string, JSONNode> datas = new Dictionary<string, JSONNode>();

    //public static void PreloadDatas<T>() where T : TextAsset
    //{
    //    string assetName = typeof(T).Name;
    //    JSONNode json;
    //    if (datas.TryGetValue(assetName, out json) == true)
    //    {
    //        return;
    //    }

    //    json = JSON.Parse(Resources.Load<TextAsset>("Data/" + assetName).text)["data"];
    //    if (json == null)
    //    {
    //        Debug.LogError("cannot load " + assetName);
    //        return;
    //    }

    //    datas[assetName] = json;
    //}

    public static JSONNode LoadData(string dataName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + dataName);

        return JSON.Parse(textAsset.text);
    }

    protected void LoadDataLocal(string dataName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + dataName);
        data = JSON.Parse(textAsset.text)["data"];
    }

    public void LoadData(TextAsset textAsset)
    {
        data = JSON.Parse(textAsset.text);
        if(data["data"] != null)
            data = data["data"];
    }

    public T GetData<T>()
    {
        return JsonUtility.FromJson<T>(data.ToString());
    }

    public int[] GetDataIntArray()
    {
        JSONArray array = data.AsArray;
        int length = array.Count;
        int[] t = new int[length];
        for (int i = 0; i < length; i++)
        {
            t[i] = int.Parse(array[i].ToString());
        }
        return t;
    }
    
    public T[] GetDatas<T>()
    {
        JSONArray array = data.AsArray;
        int length = array.Count;
        T[] t = new T[length];
        for (int i = 0; i < length; i++)
        {
            t[i] = JsonUtility.FromJson<T>(array[i].ToString());
        }
        
        return t;
    }

    public T GetData<T>(string key)
    {
        JSONObject json = data.AsObject;
        return JsonUtility.FromJson<T>(json[key].ToString());
    }

    public string GetData(string key)
    {
        JSONObject json = data.AsObject;
        return json[key].ToString();
    }
   

    public T[] GetDatas<T>(string key)
    {
        JSONObject json = data.AsObject;
        JSONArray array = json[key].AsArray;
        int length = array.Count;
        T[] t = new T[length];
        for (int i = 0; i < length; i++)
        {
            t[i] = JsonUtility.FromJson<T>(array[i].ToString());
        }
        return t;
    }
    public T[] GetDatas<T>(string key_parent, string key)
    {
        JSONNode jObject = data[key_parent].AsObject;
        JSONArray array = jObject[key].AsArray;
        int length = array.Count;
        T[] t = new T[length];
        for (int i = 0; i < length; i++)
        {
            t[i] = JsonUtility.FromJson<T>(array[i].ToString());
        }
        return t;
    }

    public T GetData<T>(int level)
    {
        JSONArray array = data.AsArray;
        if (level > array.Count)
            return JsonUtility.FromJson<T>(array[array.Count - 1].ToString());
        return JsonUtility.FromJson<T>(array[level - 1].ToString());
    }

    public object GetData(string key,Type type)
    {
        JSONObject json = data.AsObject;
        if (json[key].IsNull) return null;

        return JsonUtility.FromJson(json[key].ToString(),type);
    }

    public JSONObject GetData(int level)
    {
        return data[level-1].AsObject;
    }

   public string[] GetKeys()
    {
        JSONObject array = data.AsObject;
        JSONNode.KeyEnumerator keysEnum = array.Keys;
        List<string> keys = new List<string>();
        while (keysEnum.MoveNext())
        {
            keys.Add(keysEnum.Current.ToString().Replace("\"", ""));
        }
        return keys.ToArray();
    }

    public string[] GetKeys(string key)
    {
        JSONObject array = data[key].AsObject;
        JSONNode.KeyEnumerator keysEnum = array.Keys;
        List<string> keys = new List<string>();
        while (keysEnum.MoveNext())
        {
            keys.Add(keysEnum.Current.ToString().Replace("\"", ""));
        }
        return keys.ToArray();
    }


}
