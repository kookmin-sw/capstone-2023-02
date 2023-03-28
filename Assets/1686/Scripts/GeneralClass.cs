using UnityEngine;

class GeneralClass
{
    public enum debugLevel
    {
        none = -1,
        very_low,
        low,
        middle,
        high,
        very_high
    }

    public static debugLevel debug = debugLevel.low;

    public static void Log(string msg, debugLevel dlv = debugLevel.middle)
    {
        if (debug <= dlv) Debug.Log(msg);
    }
}