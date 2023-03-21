using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    ShakeCamera
}

public static class EventManager
{

    private static Dictionary<EventType, System.Action> events = new();
    public static Dictionary<EventType, System.Action> Events { get { return events; } }


    public static void AddListener(EventType type, System.Action action)
    {
        if (!Events.ContainsKey(type))
        {
            Events.Add(type, action);
        }
        else
        {
            Events[type] += action;
        }
    }

    public static void RemoveListener(EventType type, System.Action action)
    {
        if (!Events.ContainsKey(type)) { return; }
        Events[type] -= action;
    }

    public static void InvokeEvent(EventType type)
    {
        Events[type]?.Invoke();
    }

}
