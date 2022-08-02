using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
