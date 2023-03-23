using UnityEngine;
using System;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    private bool currentTurn;
    public bool CurrentTurn
    {
        get
        {
            return currentTurn;
        }
        set
        {
            currentTurn = value;
            ChangeTurnAction();
        }
    }
    public Vector2Int entityPosition = Vector2Int.zero;

    public bool inCombat = false;

    [SerializeField] protected Skills skillsScriptableObject;

    public float Health = 100f;

    public bool Shield = false;

    public List<Item> skills = new();

    public List<InventorySlot> inventorySlots = new();

    public void TakeDamage(float amount)
    {
        Health -= amount;
    }

    public void AddPower(Item skill, bool player)
    {
        if (!skills.Contains(skill) && skills.Count < 3 && !player)
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

    public virtual void ChangeTurnAction()
    {
    }
}
