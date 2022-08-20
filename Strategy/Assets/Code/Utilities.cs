using System;
using System.Linq;
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

    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        Debug.Assert(enumerable.Count() != 0);
        var index = UnityEngine.Random.Range(0, enumerable.Count());
        return enumerable.ElementAt(index);
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
    public static Timer CreateTimer(GameObject gameObject, float duration, Action callback, string name = "")
    {
        var timer = gameObject.AddComponent<Timer>();
        timer.TotalTime = duration;
        timer.OnTimeElapsed = callback;
        timer.Name = name;
        return timer;
    }

    public static Timer CreateTimer(GameObject gameObject, float duration, Action callback, string name, Action<float, float> onProgressUpdate)
    {
        var timer = CreateTimer(gameObject, duration, callback, name);
        timer.OnProgressUpdate = onProgressUpdate;
        return timer;
    }

    public static Timer CreateRepeatingTimer(GameObject gameObject, float duration, Action callback)
    {
        var timer = CreateTimer(gameObject, duration, callback);
        timer.IsRepeating = true;
        return timer;
    }

    public static int CalculateArmyPower(Army army)
    {
        Func<SoldierGroup, int> calculateUnitsPower = soldierGroup =>
        {
            int power = soldierGroup.unitData.MaxHP / 2;
            power += soldierGroup.unitData.UnitType == UnitType.Melee ? soldierGroup.unitData.MeeleAttack : soldierGroup.unitData.RangedAttack;
            return power * soldierGroup.NumberOfMembers;
        };

        return army.soldiers.Sum(calculateUnitsPower);
    }
}
