using CodeStage.AntiCheat.ObscuredTypes;

public class Constant
{
    public const string Token = "Token";
    public const string Authorization = "authorization";

    public const string Finish = "Finish";
    public const string Next = "Next";
    public const string LastTimeStamp = "LastTimeStamp";
    public const string UserAddress = "UserAddress";
    public const string SIGNATURE = "SIGNATURE";
    public const string NOTICE = "NOTICE";
    public const string CONNECTION_ERROR_NAME = "CONNECTION ERROR";
    public const string CONNECTION_ERROR_CONTENT = "NO INTERNET ACCESS.\nPLEASE VERIFY YOUR INTERNET NETWORK!";
    public const string LoginFailed = "Login Failed";
    public const string COMMON_ERROR = "SOMETHING WRONG, TRY AGAIN LATER!";
    public const string BALANCE_NOT_ENOUGH = "YOUR BALANCE IS NOT ENOUGH!";
    public static readonly ObscuredInt EXPIRED_TIME = 86402;
}

public class SMErrorCode
{
    public const string SUCCESS = "OW-SM:200";
    public const string SIGNATURE_INVALID = "OW-SM:203";
    public const string TIMESTAMP_INVALID = "OW-SM:204";
    public const string MISSING_PARAM = "OW-SM:205";
    public const string TOKEN_REQUIRED = "OW-SM:206";
    public const string TOKEN_INVALID = "OW-SM:207";
    public const string ADDRESS_INVALID = "OW-SM:208";
    public const string INSUFFICIENT_FUND = "OW-SM:301";
    public const string ADDRESS_INVALID_OR_UNSUPPORTED = "OW-SM:302";
    public const string PAYMENT_FAIL = "OW-SM:303";
    public const string AMDIN_KEY_REQUIRED = "OW-SM:401";
    public const string AMDIN_KEY_INVALID = "OW-SM:402";
    public const string MUST_LEAST_ONE_UPDATE_FIELD = "OW-SM:403";
    public const string CONFIRM_QUIT_SM = "Game is running! Are you sure to close Slot machine ?";
}

public class EventName
{
    public const string CONSUMED = "CONSUMED";
    public const string JOIN_SUCCESS = "JOIN_SUCCESS";
    public const string PING = "PING";
    public const string NEW_CHAT_MESSAGE = "NEW_CHAT_MESSAGE";
    public const string GET_CHAT_HISTORY = "GET_CHAT_HISTORY";
    public const string GET_CLASSROOM_RESOURCES = "GET_CLASSROOM_RESOURCES";
    public const string INTERACT_REMOTE = "INTERACT_REMOTE";
    public const string SWITCH_PROJECTOR_MODE = "SWITCH_PROJECTOR_MODE";
    public const string UPDATE_VIDEO_VALUE = "UPDATE_VIDEO_VALUE";
    public const string entityUpdate = "entityUpdate";
    public const string END_QUIZ = "END_QUIZ";
}

