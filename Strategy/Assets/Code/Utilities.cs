using System;
using System.Collections.Generic;
using UnityEngine;
public static class ExtensionMethods
{
    public static string Stringify<T>(this IEnumerable<T> enumerable)
    {
        string result = "[";
        foreach (var elem in enumerable)
        {
            result += elem;
        }
        result += ']';

        return result;
    }

    public static Dictionary<V, K> Reverse<K, V>(this IDictionary<K, V> dict)
    {
        var inverseDict = new Dictionary<V, K>();
        foreach (var kvp in dict)
        {
            if (!inverseDict.ContainsKey(kvp.Value))
            {
                inverseDict.Add(kvp.Value, kvp.Key);
            }
        }
        return inverseDict;
    }
}

public static class Utils
{
    public static Timer CreateTimer(GameObject gameObject, float duration, Action callback)
    {
        var timer = gameObject.AddComponent<Timer>();
        timer.TotalTime = duration;
        timer.OnTimeElapsed = callback;
        return timer;
    }

    public static Timer CreateTimer(GameObject gameObject, float duration, Action callback, Action<float, float> onProgressUpdate)
    {
        var timer = gameObject.AddComponent<Timer>();
        timer.TotalTime = duration;
        timer.OnTimeElapsed = callback;
        timer.OnProgressUpdate = onProgressUpdate;
        return timer;
    }

    public static Timer CreateRepeatingTimer(GameObject gameObject, float duration, Action callback)
    {
        var timer = gameObject.AddComponent<Timer>();
        timer.TotalTime = duration;
        timer.OnTimeElapsed = callback;
        timer.IsRepeating = true;
        return timer;
    }
}
