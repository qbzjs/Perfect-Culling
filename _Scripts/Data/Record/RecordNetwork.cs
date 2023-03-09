using System.Collections.Generic;

public struct RecordNetwork { }

#region Login Request
[System.Serializable]
public struct RecordLoginRequest
{
    public string signature;
    public string address;
    public long timestamp;
}
#endregion

#region Response
[System.Serializable]
public struct TPRLResponse
{
    public bool error;
    public string message;
    public string output;
}
#endregion

#region UserInfoResponse
[System.Serializable]
public class UserInfoResponse
{
    public bool error;
    public string message;
    public Output output;

    [System.Serializable]
    public class Output
    {
        public string _id;
        public string address;
        public string username;
        public bool is_skip_username;
        public string created_at;
        public long last_updated;
        public string inventory;
    }
}
#endregion

#region UserBalanceResponse
[System.Serializable]
public class UserBalanceResponse
{
    public bool error;
    public string message;
    public Output output;

    [System.Serializable]
    public class Output
    {
        public Wallet[] wallets;
        public string address;

    }
}
#endregion

#region UserProgress
[System.Serializable]
public class UserProgressResponse
{
    public bool error;
    public string message;
    public Output output;
    [System.Serializable]
    public class Output
    {
        public string address;
        public bool is_tutorial_completed;
        public RecordMissionDailyAPI[] daily;
        public RecordMissionSubAPI[] sub;
        public RecordMissionMainAPI main;
    }
}
[System.Serializable]
public class RecordMissionMainAPI
{
    public int mission_id;
    public int current_progress;
}
[System.Serializable]
public struct RecordMissionDailyAPI
{
    public int mission_id;
    public int current_progress;
    public bool is_reward_received;
}
[System.Serializable]
public struct RecordMissionSubAPI
{
    public int mission_id;
    public int current_progress;
    public bool is_reward_received;
}
[System.Serializable]
public struct RecordUserUpdateProgressRequest
{
    public string address;
    public string type;
    public int mission_id;
    public int current_progress;
}
[System.Serializable]
public struct RecordUserUpdateProgressReponse
{
    public bool error;
    public string message;
    public Output output;
    [System.Serializable]
    public class Output
    {
        public string type;
        public int mission_id;
        public bool is_completed;
    }
}
[System.Serializable]
public struct RecordUserRewardRequest
{
    public string address;
    public string type;
    public int mission_id;
}

[System.Serializable]
public struct RecordRewardResponse
{
    public bool error;
    public string message;
    public Output[] output;
    [System.Serializable]
    public class Output
    {
        public int item_id;
        public string item_type;
        public int quantity;
    }
}


#endregion

#region SubmitTutorial
public struct RecordSubMitTutorialRequest
{
    public string address;
}
#endregion

public struct PlaySlotMachineRequest
{
    public string address;
    public string token_address;
}

[System.Serializable]
public class PlaySlotMachineResponse
{
    public bool error;
    public string message;
    public PlaySlotMachineOutput output;
}

[System.Serializable]
public class PlaySlotMachineOutput
{
    public int[] values;
    public int multi;
}

[System.Serializable]
public struct RecordJoinOpenworldRoomRequest
{
    public string token;
    public string username;
}

[System.Serializable]
public struct RecordJoinOpenworldRoomResponse
{
    public bool error;
    public ColyseusRoom output;
}

[System.Serializable]
public struct ColyseusRoom
{
    public SeatReservation seatReservation;

    [System.Serializable]
    public struct SeatReservation
    {
        public Room room;
        public string sessionId;
    }

    [System.Serializable]
    public struct Room
    {
        public int clients;
        public string createdAt;
        public int maxClients;
        public string name;
        public string processId;
        public string roomId;
    }
}


[System.Serializable]
public struct RecordJoinClassRoomRequest
{
    public string username;
    public string class_id;
    public string password;
    public string ow_ss_id;
}

[System.Serializable]
public struct RecordJoinClassRoomResponse
{
    public bool error;
    public ColyseusRoom output;
}

[System.Serializable]
public struct RecordClassRoomResources
{
    public Link video;
    public Link slide;

    [System.Serializable]
    public struct Link
    {
        public string link_prefix;
        public int total;
    }
}

public class RecordChatHistory
{
    public List<RecordChat> chat_histories { get; set; }
}

public class RecordChat
{
    public int id { get; set; }
    public string s_id { get; set; }
    public string m { get; set; }
}

