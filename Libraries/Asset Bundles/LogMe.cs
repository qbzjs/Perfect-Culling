using UnityEngine;

public class LogMe
{
    private static bool isLogOn = true;
    private static bool isLogWarningOn = true;
    private static bool isLogErrorOn = true;

    static public void Log(string log)
    {
        if (isLogOn)
            Debug.Log(log);
    }

    static public void LogWarning(string log)
    {
        if (isLogWarningOn)
            Debug.LogWarning(log);
    }

    static public void LogError(string log)
    {
        if (isLogErrorOn)
            Debug.LogError(log);
    }

    public static bool CompareUnitName(string unit_name_1, string unit_name_2)
    {
        return unit_name_1.ToLower().Equals(unit_name_2.ToLower());
    }
}
