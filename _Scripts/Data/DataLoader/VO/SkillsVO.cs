


using UnityEngine;

public class SkillInfo
{
    //public SubInfo condition;
    //public SkillCharInfo skillCharInfo;
}

public class SkillsVO : BaseMutilVO
{
    public SkillsVO()
    {
        LoadDataByDirectories<BaseVO>("Skills");
    }

    //public RecordObjSummon GetInfo(string skill_name)
    //{
   
    //    RecordObjSummon record = DataController.Instance.SkillsVO.GetDataByName<RecordObjSummon>(skill_name.Replace("\"", ""));
    //    if (!string.IsNullOrEmpty(record.data_effect_path_file.file_name) && !string.IsNullOrEmpty(record.data_effect_path_file.key_names))
    //    {
    //        string[] keys = new string[1];
    //        if (record.data_effect_path_file.key_names.Contains(","))
    //            keys = record.data_effect_path_file.key_names.Split(',');
    //        else
    //            keys[0] = record.data_effect_path_file.key_names;
    //        int length = keys.Length;
    //        record.effect_path = new RecordObjSummon.Effect[length];
    //        for (int j = 0; j < length; j++)
    //        {
    //            RecordObjSummon.Effect effect = DataController.Instance.SkillsVO.GetDataByName<RecordObjSummon.Effect>(record.data_effect_path_file.file_name, keys[j].Trim());
    //            record.effect_path[j] = effect;
    //        }
    //    }

    //    return record;
    //}

    //public SkillInfo GetSkillInfo(string skillName, int level)
    //{
    //    SkillInfo skillInfo = new SkillInfo();
    //    JSONObject json = dic_Data[skillName].GetData(level);
    //    Debug.Log(json);
    //    JSONObject dataCondition = json["condition"].AsObject;
    //    SubInfo condition = new SubInfo();
    //    Debug.Log(dataCondition.ToString());
    //    condition.type = dataCondition["type"];
    //    if (dataCondition["typeInfo"] != null)
    //    {
    //        string typeInfo = dataCondition["typeInfo"];
    //        condition.data = JsonUtility.FromJson(dataCondition["data"].ToString(), Type.GetType(typeInfo));
    //    }
    //    skillInfo.condition = condition;
    //    Debug.Log(skillInfo.condition.type);
    //    JSONObject dataskillCharInfo = json["skillCharInfo"].AsObject;
    //    Debug.Log(dataskillCharInfo.ToString());
    //    SkillCharInfo skillCharInfo = JsonUtility.FromJson<SkillCharInfo>(dataskillCharInfo.ToString());
    //    JSONObject dataSkillInfo = dataskillCharInfo["skillInfo"].AsObject;
    //    Debug.Log(skillCharInfo.EndSkillAnim);
    //    if (dataSkillInfo["typeInfo"] != null)
    //    {
    //        string typeInfo = dataSkillInfo["typeInfo"];
    //        skillCharInfo.skillInfo.data = JsonUtility.FromJson(dataSkillInfo["data"].ToString(), Type.GetType(typeInfo));
    //    }
    //    skillInfo.skillCharInfo = skillCharInfo;
    //    return skillInfo;
    //}
}
