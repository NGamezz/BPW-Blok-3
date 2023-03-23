using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private List<Character> entities = new();
    public List<Character> Entities { get { return entities; } }

    public int CurrentTurn { get; private set; }

    private Character player;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        player = FindObjectOfType<PlayerControlls>();
        entities.Add(player);

        Character[] characters = FindObjectsOfType<Character>();
        foreach (Character entity in characters)
        {
            if (!entities.Contains(entity))
            {
                entities.Add(entity);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartCombat, StartCombat);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.StartCombat, StartCombat);
    }
    private void StartCombat()
    {
        foreach (Character character in entities)
        {
            if (character.inCombat)
            {
                character.gameObject.SetActive(true);
            }
            else
            {
                character.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        CurrentTurn = 1;

        foreach (Character entity in entities)
        {
            if (entity == player)
            {
                entity.CurrentTurn = true;
            }
            else
            {
                entity.CurrentTurn = false;
            }
        }
    }

    public void ChangeTurn()
    {
        if (entities.Count < 2) { return; }

        entities[CurrentTurn - 1].CurrentTurn = false;

        CurrentTurn++;
        if (CurrentTurn > entities.Count)
        {
            CurrentTurn = 1;
        }

        entities[CurrentTurn - 1].CurrentTurn = true;
    }
}
