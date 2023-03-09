public class SoldiersVO : BaseMutilVO
{
    public SoldiersVO()
    {
        LoadDataByDirectories<BaseVO>("Tower");
    }

    public override T GetData<T>(string type, int level)
    {
        T result = base.GetData<T>(type, level);
        //if (result is ShootInfo)
        //{
        //    ShootInfo shootInfo = result as ShootInfo;
        //    JSONObject json = dic_Data[type].GetData(level);
        //    shootInfo.bulletInfo = JsonUtility.FromJson(json["bulletInfo"].ToString(), Type.GetType(shootInfo.typeBullet));
        //}
        return result;
    }

    //public RecordLevelUp[] GetRecordLevelUp(string file_name)
    //{
    //    RecordLevelUp[] levelUpInfos = DataController.Instance.soldiersVO.GetDatasByName<RecordLevelUp>(file_name);
    //    //if(levelUpInfos != null && levelUpInfos.Length > 0)
    //    //{
    //    //    int length = levelUpInfos.Length;
    //    //    for (int i = 0; i < length; i++)
    //    //    {
    //    //        RecordLevelUp record = levelUpInfos[i];
    //    //        if (!string.IsNullOrEmpty(record.data_skill_passive_info_file.file_name) && !string.IsNullOrEmpty(record.data_skill_passive_info_file.key_names))
    //    //        {
    //    //            string[] keys = new string[1];
    //    //            if (record.data_skill_passive_info_file.key_names.Contains(","))
    //    //                keys = record.data_skill_passive_info_file.key_names.Split(',');
    //    //            else
    //    //                keys[0] = record.data_skill_passive_info_file.key_names;
    //    //            int lengthSkill = keys.Length;
    //    //            levelUpInfos[i].skill_passive_info = new string[lengthSkill];
    //    //            for (int j = 0; j < lengthSkill; j++)
    //    //            {
    //    //                string skillName = DataController.Instance.enemiesVO.GetDataByName(record.data_skill_passive_info_file.file_name, keys[j].Trim());
    //    //                levelUpInfos[i].skill_passive_info[j] = skillName.Replace("\"", "");
    //    //            }
    //    //        }

    //    //        if (!string.IsNullOrEmpty(record.atk_info.data_effect_path_file.file_name) && !string.IsNullOrEmpty(record.atk_info.data_effect_path_file.key_names))
    //    //        {
    //    //            string[] keys = new string[1];
    //    //            if (record.atk_info.data_effect_path_file.key_names.Contains(","))
    //    //                keys = record.atk_info.data_effect_path_file.key_names.Split(',');
    //    //            else
    //    //                keys[0] = record.atk_info.data_effect_path_file.key_names;
    //    //            int lengthSkill = keys.Length;
    //    //            levelUpInfos[i].atk_info.effect_path = new RecordObjSummon.Effect[lengthSkill];
    //    //            for (int j = 0; j < lengthSkill; j++)
    //    //            {
    //    //                RecordObjSummon.Effect effect = DataController.Instance.enemiesVO.GetDataByName<RecordObjSummon.Effect>(record.atk_info.data_effect_path_file.file_name, keys[j].Trim());
    //    //                levelUpInfos[i].atk_info.effect_path[j] = effect;
    //    //            }
    //    //        }
    //    //    }
    //    //}
    //    return levelUpInfos;
    //}

    //public RecordEnemyInfo GetInfos(string unit_name)
    //{
    //    RecordEnemyInfo record = DataController.Instance.soldiersVO.GetDataByName<RecordEnemyInfo>($"data_{unit_name.ToLower()}_infos");
    //    if (!string.IsNullOrEmpty(record.atk_info.data_effect_path_file.file_name) && !string.IsNullOrEmpty(record.atk_info.data_effect_path_file.key_names))
    //    {
    //        string[] keys = new string[1];
    //        if (record.atk_info.data_effect_path_file.key_names.Contains(","))
    //            keys = record.atk_info.data_effect_path_file.key_names.Split(',');
    //        else
    //            keys[0] = record.atk_info.data_effect_path_file.key_names;
    //        int lengthSkill = keys.Length;
    //        record.atk_info.effect_path = new RecordObjSummon.Effect[lengthSkill];
    //        for (int j = 0; j < lengthSkill; j++)
    //        {
    //            RecordObjSummon.Effect effect = DataController.Instance.soldiersVO.GetDataByName<RecordObjSummon.Effect>(record.atk_info.data_effect_path_file.file_name, keys[j].Trim());
    //            record.atk_info.effect_path[j] = effect;
    //        }
    //    }

    //    if (!string.IsNullOrEmpty(record.data_skill_passive_info_file.file_name) && !string.IsNullOrEmpty(record.data_skill_passive_info_file.key_names))
    //    {
    //        string[] keys = new string[1];
    //        if (record.data_skill_passive_info_file.key_names.Contains(","))
    //            keys = record.data_skill_passive_info_file.key_names.Split(',');
    //        else
    //            keys[0] = record.data_skill_passive_info_file.key_names;
    //        int lengthSkill = keys.Length;
    //        record.skill_passive_info = new string[lengthSkill];
    //        for (int j = 0; j < lengthSkill; j++)
    //        {
    //            string skillName = DataController.Instance.soldiersVO.GetDataByName(record.data_skill_passive_info_file.file_name, keys[j].Trim());
    //            record.skill_passive_info[j] = skillName;
    //        }
    //    }

    //    return record;
    //}
}
