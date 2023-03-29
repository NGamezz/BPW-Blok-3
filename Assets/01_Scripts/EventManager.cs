using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public enum EventType
{
    ShakeCamera,
    StartCombat,
    ExitCombat,
    GameOver,
    Restart,
    Pauze,
    Resume,
    MovePlayer,
    ItemDrop,
    PickupItem
}

public static class EventManager
{
    public static Dictionary<EventType, System.Action> Events { get { return events; } }

    private static Dictionary<EventType, System.Action> events = new();

    public static void ClearEvents()
    {
        events.Clear();
    }

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
        if (!Events.ContainsKey(type)) { return; }
        Events[type]?.Invoke();
    }

}
