using System.Collections.Generic;
using UnityEngine;

public static class Systems
{
    public static void RegisterSystem<System>(System system) where System : class
    {
        var systemName = StringifyType<System>();
        //Debug.Log(systemName);
        Debug.Assert(!registeredSystems.ContainsKey(systemName));
        registeredSystems.Add(systemName, system);
    }

    public static void DeregisterSystem<System>(System system) where System : class
    {
        string systemName = typeof(System).Name;
        registeredSystems.Remove(systemName);
    }

    public static System Get<System>() where System: class
    {
        string systemName = StringifyType<System>();
        Debug.Assert(registeredSystems.ContainsKey(systemName));
        return registeredSystems[systemName] as System;
    }

    private static string StringifyType<Type>()
    {
        return typeof(Type).ToString();
    }
    private static Dictionary<string, object> registeredSystems = new();
}
