using System.Collections.Generic;
using UnityEngine;
public class BaseMutilVO
{
    public Dictionary<string, BaseVO> dic_Data = new Dictionary<string, BaseVO>();

    protected void LoadData<T>(string dataName) where T : BaseVO, new()
    {
        TextAsset[] texts = Resources.LoadAll<TextAsset>($"Data/{dataName}");

        foreach (TextAsset text in texts)
        {
            T data = new T();
            data.LoadData(text);
            dic_Data.Add(text.name, data);
        }
    }

    protected void LoadDataByDirectories<T>(string dataName) where T : BaseVO, new()
    {
        TextAsset[] texts = Resources.LoadAll<TextAsset>($"Data/{dataName}");
        int length = texts.Length;
        for (int i = 0; i < length; i++)
        {
            TextAsset text = texts[i];
            T data = new T();
            data.LoadData(text);
            dic_Data.Add(text.name, data);
        }
    }

    public virtual T GetData<T>(string type, int level)
    {
        return dic_Data[type].GetData<T>(level);
    }

    public T GetDataByName<T>(string file_name)
    {
        return dic_Data[file_name].GetData<T>();
    }

    public int[] GetDataIntArray(string file_name)
    {
        return dic_Data[file_name].GetDataIntArray();
    }

    public T[] GetDatasByName<T>(string file_name)
    {
        if (dic_Data.ContainsKey(file_name))
        {
            return dic_Data[file_name].GetDatas<T>();
        }

        return null;
    }

    public T GetDataByName<T>(string file_name, string key)
    {
        return dic_Data[file_name].GetData<T>(key);
    }

    public string GetDataByName(string file_name, string key)
    {
        return dic_Data[file_name].GetData(key);
    }

    public T[] GetDatasByName<T>(string file_name, string key)
    {
        return dic_Data[file_name].GetDatas<T>(key);
    }

    public T[] GetDatasByName<T>(string file_name, string key_parent, string key)
    {
        return dic_Data[file_name].GetDatas<T>(key_parent, key);
    }

    public string[] GetKeys(string file_name)
    {
        return dic_Data[file_name].GetKeys();
    }

    public string[] GetKeys(string file_name, string key)
    {
        return dic_Data[file_name].GetKeys(key);
    }
}
