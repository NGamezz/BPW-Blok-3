using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class Character : MonoBehaviour
{
    public List<Item> skills = new();

    public List<InventorySlot> inventorySlots = new();
    public bool CurrentTurn
    {
        get
        {
            return currentTurn;
        }
        set
        {
            currentTurn = value;
            ChangeTurnAction(value);
        }
    }
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            if (inCombat)
            {
                healthSlider.value = (Health / maxHealth);
                if (health <= 0)
                {
                    HealthDepletedAction();
                }
            }
        }
    }

    public int CurrentAmountOfTurns { get { return currentAmountOfTurns; } set { currentAmountOfTurns = value; } }

    public bool Shield = false;

    public bool inCombat = false;

    public Vector2Int entityPosition = Vector2Int.zero;

    public TMP_Text MoveText;

    [SerializeField] protected UnityEngine.UI.Slider healthSlider;

    [SerializeField] private float health = 100f;

    [SerializeField] protected int currentAmountOfTurns;

    [SerializeField] protected float shakeAmplitude = 4;

    [SerializeField] protected float shakeDuration = 1.5f;

    [SerializeField] protected bool canShakePlayer;

    [SerializeField] private bool currentTurn;

    protected Vector3 initialLocalPosition = Vector3.zero;

    protected float shakeTimer;

    protected float maxHealth;

    public void ShakePlayer()
    {
        canShakePlayer = true;
        shakeTimer = shakeDuration;
    }

    public virtual void ShakeCharacter()
    {
    }

    public void Heal(float amount)
    {
        Health += amount;
        if (Health > maxHealth)
        {
            Health = maxHealth;
        }
        Shield = false;
    }

    public void TakeDamage(float amount)
    {
        if (!Shield)
        {
            Health -= amount;
        }
        Shield = false;
    }

    public void AddPower(Item skill, bool player, Transform transform)
    {
        if (!skills.Contains(skill) && skills.Count < 3 && !player)
        {
            skills.Add(skill);
            skill.gameObject.transform.SetParent(transform);
        }
    }

    public void RemovePower(Item skill)
    {
        if (skills.Contains(skill))
        {
            skills.Remove(skill);
        }
    }

    public virtual void HealthDepletedAction()
    {
        EventManager.InvokeEvent(EventType.ExitCombat);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public virtual void ChangeTurnAction(bool value)
    {
    }
}
