using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.Events;
using Colyseus;

public struct RequestParameter
{
    public string key;
    public string value;
    public RequestParameter(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}

//"link_api": "http://139.99.121.150:6665/ow-slotmachine-api/user/",
//"link_api": "https://ow-sm-api-dev.theparallel.io/ow-slotmachine-api/user/",
public class TPRLAPI : SingletonMonoDontDestroy<TPRLAPI>
{
    private static string _api_url = "";
    private static string APIURL
    {
        get
        {
            if (string.IsNullOrEmpty(_api_url))
                _api_url = EnvironmentConfig.linkApi;
            return _api_url;
        }
    }

    private static string _server_config = "";
    private static string APIServerConfig
    {
        get
        {
            if (string.IsNullOrEmpty(_server_config))
                _server_config = EnvironmentConfig.linkGetServerConfig;
            return _server_config;
        }
    }

    private static void UpdateToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;
        UserDatas.token = token;
    }

    public void GetServerConfig(string url, UnityAction<string> response, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append(EnvironmentConfig.linkGetServerConfig);
        request.Append(url);
        StartCoroutine(IEGet(request.ToString(), default, response, action_error));
    }

    //Get request with only url
    public void Get(string url, RequestParameter header, UnityAction<string> response, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append(APIURL);
        request.Append(url);
        StartCoroutine(IEGet(request.ToString(), header, response, action_error));
    }

    private IEnumerator IEGet(string url, RequestParameter header, UnityAction<string> action_success, UnityAction<string> action_error)
    {

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            if (!header.Equals((RequestParameter)default))
                www.SetRequestHeader(header.key, header.value);
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                string error = www.error;
                Debug.LogError(error);
                action_error?.Invoke(error);
            }
            else
            {
                string token = www.GetResponseHeader(Constant.Authorization);
                UpdateToken(token);
                action_success?.Invoke(www.downloadHandler.text);
            }
        }
    }

    //Get Request with Parameter
    public void GetWithParameters(string url, RequestParameter header, RequestParameter[] parameters, UnityAction<string> response, UnityAction<string> action_error)
    {
        StartCoroutine(IEGetWithParameters(url, header, parameters, response, action_error));
    }

    private IEnumerator IEGetWithParameters(string url, RequestParameter header, RequestParameter[] parameters, UnityAction<string> action_success, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append(APIURL);
        request.Append(url);
        request.Append("?");
        if (parameters != null && parameters.Length > 0)
        {

            int length = parameters.Length;
            for (int i = 0; i < length; i++)
            {
                request.Append(parameters[i].key + "=" + parameters[i].value);
                if (i != length - 1)
                    request.Append("&");
            }
        }

        using (UnityWebRequest www = UnityWebRequest.Get(request.ToString()))
        {
            if (!header.Equals((RequestParameter)default))
                www.SetRequestHeader(header.key, header.value);
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                string error = www.error;
                Debug.LogError(error);
                action_error?.Invoke(error);
            }
            else
            {
                string token = www.GetResponseHeader(Constant.Authorization);
                UpdateToken(token);
                action_success?.Invoke(www.downloadHandler.text);
            }
        }
    }

    //Get request with request body ???
    public void GetWithBody(string url, RequestParameter header, string bodyJsonString, UnityAction<string> response, UnityAction<string> action_error)
    {
        StartCoroutine(IEGetWithBody(url, header, bodyJsonString, response, action_error));
    }

    private IEnumerator IEGetWithBody(string url, RequestParameter header, string bodyJsonString, UnityAction<string> action_success, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append(APIURL);
        request.Append(url);

        using (UnityWebRequest www = UnityWebRequest.Get(request.ToString()))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "*/*");
            if (!header.Equals((RequestParameter)default))
                www.SetRequestHeader(header.key, header.value);
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(bodyJsonString));
            www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                string error = www.error;
                Debug.LogError(error);
                action_error?.Invoke(error);
            }
            else
            {
                string token = www.GetResponseHeader(Constant.Authorization);
                UpdateToken(token);
                action_success?.Invoke(www.downloadHandler.text);
            }
        }
    }

    //Post Request without token
    public void Post(string url, string bodyJsonString, UnityAction<string> response, UnityAction<string> action_error)
    {
        StartCoroutine(IEPost(url, bodyJsonString, response, action_error));
    }

    private IEnumerator IEPost(string url, string bodyJsonString, UnityAction<string> action_success, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append(APIURL);
        request.Append(url);
        using (UnityWebRequest www = UnityWebRequest.Post(request.ToString(), "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                StringBuilder stringS = new StringBuilder();
                stringS.Append(www.error);
                stringS.Append("-");
                stringS.Append(www.responseCode);
                Debug.LogError(stringS.ToString());
                action_error?.Invoke(stringS.ToString());
            }
            else
            {
                string token = www.GetResponseHeader(Constant.Authorization);
                UpdateToken(token);
                action_success?.Invoke(www.downloadHandler.text);
            }
        }
    }

    //Post Request with parameter
    public void PostWithParameter(string prefix, string api, string bodyJsonString, RequestParameter header, UnityAction<string> response, UnityAction<string> action_error)
    {
        StartCoroutine(IEPostWithParameter(prefix, api, bodyJsonString, header, response, action_error));
    }

    private IEnumerator IEPostWithParameter(string prefix, string api, string bodyJsonString, RequestParameter header, UnityAction<string> action_success, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append(prefix);
        request.Append(api);
        using (UnityWebRequest www = UnityWebRequest.Post(request.ToString(), "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            if (!header.Equals((RequestParameter)default))
                www.SetRequestHeader(header.key, header.value);
            Debug.Log("header : " + header.key + " - " + header.value);
            www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                StringBuilder stringS = new StringBuilder();
                stringS.Append(www.error);
                stringS.Append("-");
                stringS.Append(www.responseCode);
                Debug.LogError("error : " + stringS.ToString());
                action_error?.Invoke(stringS.ToString());
            }
            else
            {
                string token = www.GetResponseHeader(Constant.Authorization);
                UpdateToken(token);
                action_success?.Invoke(www.downloadHandler.text);
            }
        }

    }

    //Post Request with parameters
    public void PostWithParameters(string prefix, string api, string bodyJsonString, RequestParameter[] headers, UnityAction<string> response, UnityAction<string> action_error)
    {
        StartCoroutine(IEPostWithParameters(prefix, api, bodyJsonString, headers, response, action_error));
    }

    private IEnumerator IEPostWithParameters(string prefix, string api, string bodyJsonString, RequestParameter[] headers, UnityAction<string> action_success, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append(prefix);
        request.Append(api);
        using (UnityWebRequest www = UnityWebRequest.Post(request.ToString(), "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            if (headers != null && headers.Length > 0)
            {
                int length = headers.Length;
                for (int i = 0; i < length; i++)
                {
                    www.SetRequestHeader(headers[i].key, headers[i].value);
                }
            }
            www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                StringBuilder stringS = new StringBuilder();
                stringS.Append(www.error);
                stringS.Append("-");
                stringS.Append(www.responseCode);
                Debug.LogError(stringS.ToString());
                action_error?.Invoke(stringS.ToString());
            }
            else
            {
                string token = www.GetResponseHeader(Constant.Authorization);
                UpdateToken(token);
                action_success?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public void GetGameConfig(UnityAction action)
    {
        GetServerConfig("get-game-config",
            (data) =>
            {
                EnvironmentConfig.serverConfig = JsonUtility.FromJson<ServerConfig>(data);
                action?.Invoke();
            }
        , (error) => { });
    }

    public void LogIn(string signMessage, string address, long timeStamp, Action<TPRLResponse> action)
    {
        RecordLoginRequest logInRequest = new RecordLoginRequest();
        logInRequest.signature = signMessage;
        logInRequest.address = address;
        logInRequest.timestamp = timeStamp;

        string jsonString = JsonUtility.ToJson(logInRequest);
        Post("login", jsonString, (data) =>
        {
            action?.Invoke(JsonUtility.FromJson<TPRLResponse>(data));
        }, null);
    }
    public void SubmitTutorialResponse(string authorization,string address, UnityAction<string> action)
    {
        RequestParameter requestParameter = new RequestParameter();
        requestParameter.key = "authorization";
        requestParameter.value = authorization;
        RecordSubMitTutorialRequest recordSubMitTutorial = new RecordSubMitTutorialRequest();
        recordSubMitTutorial.address = address;
        string jsonString = JsonUtility.ToJson(recordSubMitTutorial);
        PostWithParameter(APIURL, "submit-tutorial", jsonString, requestParameter, action, null);
    }


    public void GetUserInfo(string authorization, string address, UnityAction<string> action)
    {
        RequestParameter user_infor = new RequestParameter();
        user_infor.key = "authorization";
        user_infor.value = authorization;
        RequestParameter[] requests = new RequestParameter[1];
        RequestParameter balance = new RequestParameter();
        balance.key = "address";
        balance.value = address;
        requests[0] = balance;
        GetWithParameters("get-info", user_infor, requests, action, (error) => { });
    }

    public void GetUserBalance(string authorization, string address, UnityAction<string> action)
    {
        RequestParameter user_infor = new RequestParameter();
        user_infor.key = "authorization";
        user_infor.value = authorization;
        RequestParameter[] requests = new RequestParameter[1];
        RequestParameter balance = new RequestParameter();
        balance.key = "address";
        balance.value = address;
        requests[0] = balance;
        GetWithParameters("get-balance", user_infor, requests, action, (error) => { });
    }

    public void GetUserProgress(string authorization, string address, UnityAction<string> action)
    {
        RequestParameter user_progress = new RequestParameter();
        user_progress.key = "authorization";
        user_progress.value = authorization;
        RequestParameter[] requests = new RequestParameter[1];
        RequestParameter request = new RequestParameter();
        request.key = "address";
        request.value = address;
        requests[0] = request;
        GetWithParameters("progress", user_progress, requests, action, (error) => { });
    }
    public void PostUpdateProgressResponse(string authorization, string address,string type, int mission_id,int current_progress,UnityAction<string>action)
    {
        RequestParameter requestParameter = new RequestParameter();
        requestParameter.key = "authorization";
        requestParameter.value = authorization;
        RecordUserUpdateProgressRequest recordUserUpdateProgressRequest = new RecordUserUpdateProgressRequest();
        recordUserUpdateProgressRequest.address = address;
        recordUserUpdateProgressRequest.type = type;
        recordUserUpdateProgressRequest.mission_id = mission_id;
        recordUserUpdateProgressRequest.current_progress = current_progress;
        string jsonString = JsonUtility.ToJson(recordUserUpdateProgressRequest);
        PostWithParameter(APIURL, "update-progress", jsonString, requestParameter,action, null);
    }
    public void PostRewardResponse(string authorization, string address, string type, int mission_id, UnityAction<string> action)
    {
        RequestParameter requestParameter = new RequestParameter();
        requestParameter.key = "authorization";
        requestParameter.value = authorization;
        RecordUserRewardRequest recordUserRewardRequest = new RecordUserRewardRequest();
        recordUserRewardRequest.address = address;
        recordUserRewardRequest.type = type;
        recordUserRewardRequest.mission_id = mission_id;
        string jsonString = JsonUtility.ToJson(recordUserRewardRequest);
        PostWithParameter(APIURL, "reward", jsonString, requestParameter, action, null);
    }
        public void RequestSpinSlotmachine(string authorization, string address, string token_address, UnityAction<PlaySlotMachineResponse> action)
    {
        PlaySlotMachineRequest playSlotMachineRequest = new PlaySlotMachineRequest();
        playSlotMachineRequest.address = address;
        playSlotMachineRequest.token_address = token_address;

        string jsonString = JsonUtility.ToJson(playSlotMachineRequest);
        RequestParameter user_infor = new RequestParameter();
        user_infor.key = "authorization";
        user_infor.value = authorization;

        RequestParameter[] requestParameters = new RequestParameter[1];
        requestParameters[0] = user_infor;
        PostWithParameters(APIServerConfig, "play-game", jsonString, requestParameters, (data) =>
        {
            action?.Invoke(JsonUtility.FromJson<PlaySlotMachineResponse>(data));
        }, null);
    }

    public void JoinOpenworldRoom(Action<RecordJoinOpenworldRoomResponse> action)
    {
        RecordJoinOpenworldRoomRequest record = new RecordJoinOpenworldRoomRequest();
        record.token = UserDatas.token;
        if (UserDatas.is_guest)
        {
            string indentity = SystemInfo.deviceUniqueIdentifier;
            record.username = $"Guest#{indentity.Substring(indentity.Length - 4, 4)}";
        }
        else
        {
            if (string.IsNullOrEmpty(UserDatas.user_Data.info.username))
                record.username = $"User#{UserDatas.user_Data.info.address.Substring(UserDatas.user_Data.info.address.Length - 4, 4)}";
            else
                record.username = UserDatas.user_Data.info.username;
        }
        string jsonString = JsonUtility.ToJson(record);
        PostOpenworld("JoinOpenworldRoom", jsonString, (data) =>
        {
            action?.Invoke(JsonUtility.FromJson<RecordJoinOpenworldRoomResponse>(data));
        }, null);
    }

    public void JoinClassRoom(string id, string pass, UnityAction<RecordJoinClassRoomResponse> success, UnityAction<string> error)
    {
        string address = UserDatas.user_Data.info.address;
        RecordJoinClassRoomRequest record = new RecordJoinClassRoomRequest();
        string username = "";
        if (UserDatas.is_guest)
        {
            string indentity = SystemInfo.deviceUniqueIdentifier;
            username = $"Guest#{indentity.Substring(indentity.Length - 4, 4)}";
        }
        else
        {
            if (string.IsNullOrEmpty(UserDatas.user_Data.info.username))
                username = $"User#{address.Substring(address.Length - 4, 4)}";
            else
                username = UserDatas.user_Data.info.username;
        }

        record.username = username;
        record.class_id = id;
        record.password = pass;
        record.ow_ss_id = UserDatas.ow_session_id;
        string jsonString = JsonUtility.ToJson(record);
        PostOpenworld("JoinClassRoom", jsonString, (data) =>
        {
            success?.Invoke(JsonUtility.FromJson<RecordJoinClassRoomResponse>(data));
        }, error);
    }


    public void PostOpenworld(string url, string bodyJsonString, UnityAction<string> response, UnityAction<string> action_error)
    {
        StartCoroutine(IEPostOpenworld(url, bodyJsonString, response, action_error));
    }

    private IEnumerator IEPostOpenworld(string url, string bodyJsonString, UnityAction<string> action_success, UnityAction<string> action_error)
    {
        StringBuilder request = new StringBuilder();
        request.Append("https://ow-api-dev.theparallel.io/users/");
        //request.Append(APIURL);
        request.Append(url);
        using (UnityWebRequest www = UnityWebRequest.Post(request.ToString(), "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                StringBuilder stringS = new StringBuilder();
                stringS.Append(www.error);
                stringS.Append("-");
                stringS.Append(www.responseCode);
                Debug.LogError(stringS.ToString());
                action_error?.Invoke(stringS.ToString());
            }
            else
            {
                string token = www.GetResponseHeader(Constant.Authorization);
                UpdateToken(token);
                action_success?.Invoke(www.downloadHandler.text);
            }
        }
    }
}
