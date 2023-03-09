using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Ultis
{
    private static System.Diagnostics.Stopwatch stopwatch = null;
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static long GetCurrentTimeStamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static string sha256_hash(long timestamp)
    {
        //string value = $"{BuildCode.buildCode}{timestamp}{Application.genuine}";
        StringBuilder Sb = new StringBuilder();

        //using (SHA256 hash = SHA256Managed.Create())
        //{
        //    Encoding enc = Encoding.UTF8;
        //    byte[] result = hash.ComputeHash(enc.GetBytes(value));

        //    foreach (byte b in result)
        //        Sb.Append(b.ToString("x2"));
        //}

        return Sb.ToString();
    }
    public static void SetActiveCursor(bool active)
    {
        if (active)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public static void StopWatchStart()
    {
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
    }

    public static void StopWatchStop()
    {
        if (stopwatch == null) return;
        stopwatch.Stop();
        Debug.LogError(stopwatch.ElapsedMilliseconds);
        stopwatch = null;
    }
}
