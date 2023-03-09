using CodeStage.AntiCheat.ObscuredTypes;
using SimpleJSON;
using System;
using UnityEngine;

public class EnvironmentConfig
{
    private static JSONObject _config = null;
    private static JSONObject config
    {
        get
        {
            if (_config == null)
            {
                _config = JSON.Parse(Resources.Load<TextAsset>("Config/EnvironmentConfig").text).AsObject;
            }
            return _config;
        }
    }

    private static ObscuredString _current_environment = "";
    private static ObscuredString current_environment
    {
        get
        {
            if (string.IsNullOrEmpty(_current_environment))
            {
                _current_environment = config["environment"].ToString().Replace("\"", "");
            }
            return _current_environment;
        }
    }
    private static EnvironmentType _current_environment_enum = EnvironmentType.none;
    public static EnvironmentType currentEnvironmentEnum
    {
        get
        {
            if (_current_environment_enum == EnvironmentType.none)
            {
                _current_environment_enum = Enum.Parse<EnvironmentType>(current_environment);
            }
            return _current_environment_enum;
        }
    }

    private static ObscuredString _link_data = "";
    public static ObscuredString linkData
    {
        get
        {
            if (string.IsNullOrEmpty(_link_data))
            {
                string link_data = config[current_environment]["link_data"].ToString();
                if (link_data.StartsWith("\""))
                {
                    link_data = link_data.Remove(0, 1);
                    link_data = link_data.Remove(link_data.Length - 1, 1);
                }
                _link_data = link_data;
            }
            return _link_data;
        }
    }

    private static ObscuredString _link_get_current_version = "";
    public static ObscuredString linkGetCurrentVersion
    {
        get
        {
            if (string.IsNullOrEmpty(_link_get_current_version))
            {
                string link_get_current_version = config[current_environment]["link_get_current_version"].ToString();
                if (link_get_current_version.StartsWith("\""))
                {
                    link_get_current_version = link_get_current_version.Remove(0, 1);
                    link_get_current_version = link_get_current_version.Remove(link_get_current_version.Length - 1, 1);
                }
                _link_get_current_version = link_get_current_version;
            }
            return _link_get_current_version;
        }
    }
    private static string _link_game_ascention = "";
    public static string linkGameAscention
    {
        get
        {
            if (string.IsNullOrEmpty(_link_game_ascention))
            {
                _link_game_ascention = config[current_environment]["link_game_ascention"].ToString();
                if (_link_game_ascention.StartsWith("\""))
                {
                    _link_game_ascention = _link_game_ascention.Remove(0, 1);
                    _link_game_ascention = _link_game_ascention.Remove(_link_game_ascention.Length - 1, 1);
                }
            }
            return _link_game_ascention;
        }
    }
    private static ObscuredString _link_server_address = "";
    public static ObscuredString linkServerAddress
    {
        get
        {
            if (string.IsNullOrEmpty(_link_server_address))
            {
                string link_server_address = config[current_environment]["link_server_address"].ToString();
                if (link_server_address.StartsWith("\""))
                {
                    link_server_address = link_server_address.Remove(0, 1);
                    link_server_address = link_server_address.Remove(link_server_address.Length - 1, 1);
                }
                _link_server_address = link_server_address;
            }
            return _link_server_address;
        }
    }

    private static ObscuredString _link_api = "";
    public static ObscuredString linkApi
    {
        get
        {
            if (string.IsNullOrEmpty(_link_api))
            {
                string link_api = config[current_environment]["link_api"].ToString();
                if (link_api.StartsWith("\""))
                {
                    link_api = link_api.Remove(0, 1);
                    link_api = link_api.Remove(link_api.Length - 1, 1);
                }
                _link_api = link_api;
            }
            return _link_api;
        }
    }

    private static string _link_get_server_config = "";
    public static string linkGetServerConfig
    {
        get
        {
            if (string.IsNullOrEmpty(_link_get_server_config))
            {
                _link_get_server_config = config[current_environment]["link_get_server_config"].ToString();
                if (_link_get_server_config.StartsWith("\""))
                {
                    _link_get_server_config = _link_get_server_config.Remove(0, 1);
                    _link_get_server_config = _link_get_server_config.Remove(_link_get_server_config.Length - 1, 1);
                }
            }
            return _link_get_server_config;
        }
    }
    private static string _link_market = "";
    public static string linkMarket
    {
        get
        {
            if (string.IsNullOrEmpty(_link_market))
            {
                _link_market = config[current_environment]["link_market"].ToString();
                if (_link_market.StartsWith("\""))
                {
                    _link_market = _link_market.Remove(0, 1);
                    _link_market = _link_market.Remove(_link_market.Length - 1, 1);
                }
            }
            return _link_market;
        }
    }
    private static string _link_rename = "";
    public static string linkRename
    {
        get
        {
            if (string.IsNullOrEmpty(_link_rename))
            {
                _link_rename = config[current_environment]["link_rename"].ToString();
                if (_link_rename.StartsWith("\""))
                {
                    _link_rename = _link_rename.Remove(0, 1);
                    _link_rename = _link_rename.Remove(_link_rename.Length - 1, 1);
                }
            }
            return _link_rename;
        }
    }
    private static string _link_server_voice = "";
    public static string linkServerVoice
    {
        get
        {
            if (string.IsNullOrEmpty(_link_server_voice))
            {
                _link_server_voice = config[current_environment]["link_server_voice"].ToString();
                if (_link_server_voice.StartsWith("\""))
                {
                    _link_server_voice = _link_server_voice.Remove(0, 1);
                    _link_server_voice = _link_server_voice.Remove(_link_server_voice.Length - 1, 1);
                }
            }
            return _link_server_voice;
        }
    }
    //private static string _link_webhome = "";
    //public static string linkWebHome
    //{
    //    get
    //    {
    //        if (string.IsNullOrEmpty(_link_webhome))
    //        {
    //            _link_webhome = config[current_environment]["link_webhome"].ToString();
    //            if (_link_webhome.StartsWith("\""))
    //            {
    //                _link_webhome = _link_webhome.Remove(0, 1);
    //                _link_webhome = _link_webhome.Remove(_link_webhome.Length - 1, 1);
    //            }
    //        }
    //        return _link_webhome;
    //    }
    //}
    public static ServerConfig serverConfig;
}

[System.Serializable]
public struct ServerConfig
{
    public string current_game_version;
    public string current_data_version;
    public DataTest data_test;
    public Slotmachine slot_machine;

    [System.Serializable]
    public struct DataTest
    {
        public bool test_on;
        public string version;
        public string link_data;
    }

    [System.Serializable]
    public struct Slotmachine
    {
        public SpinFee[] spin_fee;
        public RollItem[] roll_item;
        public int roll_number;
        public bool is_maintain;
        public int max_user_play_per_machine;

        [System.Serializable]
        public struct SpinFee
        {
            public string contract_address;
            public string symbol;
            public float fee;
            public bool is_support;
        }
        [System.Serializable]
        public struct RollItem
        {
            public int key;
            public string sticker;
        }
    }
}
