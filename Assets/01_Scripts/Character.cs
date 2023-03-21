using UnityEngine;
using System;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    public bool CurrentTurn;
    public GameObject EntityMesh;
    public Vector2Int entityPosition = Vector2Int.zero;

    public List<Item> skills = new();

    public List<InventorySlot> inventorySlots = new();

    public void AddPower(Item skill)
    {
        if (!skills.Contains(skill) && skills.Count < 3)
        {
            skills.Add(skill);
        }
    }

    public void RemovePower(Item skill)
    {
        if (skills.Contains(skill))
        {
            skills.Remove(skill);
        }
    }
}
