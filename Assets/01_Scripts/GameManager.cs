using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerControlls Player { get { return player; } }
    public List<Character> Entities { get { return entities; } }
    public List<Character> EntitiesInCombat { get { return entitiesInCombat; } }
    public bool GameStarted { get; private set; }
    public int CurrentTurn { get; private set; }
    public Character CurrentEntity { get { return currentEntity; } }

    [SerializeField] private List<Character> entities = new();

    [SerializeField] private List<Character> entitiesInCombat = new();

    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private GameObject victoryScreen;

    [SerializeField] private int mainMenuSceneIndex;

    [SerializeField] private Character currentEntity;

    private bool inCombat = false;

    [SerializeField] private PlayerControlls player = null;

    public void SetPlayer(PlayerControlls player)
    {
        if (this.player == null)
        {
            if (!GameStarted)
            {
                this.player = player;
                entities.Add(player);
                DontDestroyOnLoad(this.player.PlayerHolder);
            }
        }
        else if (this.player != null)
        {
            Destroy(player.PlayerHolder);
        }
    }

    public void ChangeTurn()
    {
        if (entities.Count == 1 && entities[0] == player)
        {
            victoryScreen.SetActive(true);
            Invoke(nameof(Victory), 2f);
        }
        if (inCombat)
        {
            if (entities.Count == 0) { return; }
            entitiesInCombat[CurrentTurn].CurrentTurn = false;

            CurrentTurn++;
            if (CurrentTurn > entitiesInCombat.Count - 1)
            {
                CurrentTurn = 0;
            }

            currentEntity = entitiesInCombat[CurrentTurn];
            entitiesInCombat[CurrentTurn].CurrentTurn = true;
        }
        if (!inCombat)
        {
            if (entities.Count == 0) { return; }
            entities[CurrentTurn].CurrentTurn = false;

            CurrentTurn++;
            if (CurrentTurn > entities.Count - 1)
            {
                CurrentTurn = 0;
            }

            currentEntity = entities[CurrentTurn];
            entities[CurrentTurn].CurrentTurn = true;
        }
    }

    private void Victory()
    {
        Debug.Log("Victory");
        EventManager.InvokeEvent(EventType.Restart);
    }

    public void SetEnemies(Enemy entity)
    {
        if (!GameStarted)
        {
            entities.Add(entity);
            DontDestroyOnLoad(entity.gameObject);
        }
        else
        {
            Destroy(entity.gameObject);
        }
    }

    public void GameOver()
    {
        if (this == null) { return; }
        SceneManager.LoadScene(mainMenuSceneIndex);
        gameOverScreen.SetActive(true);
        Invoke(nameof(Restart), 2f);
    }

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
    }

    private void Restart()
    {
        if (this == null) { return; }

        Destroy(DungeonGenerator.Instance.gameObject);

        Character[] characters = FindObjectsOfType<Character>();
        entities.AddRange(characters);

        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] == null) { continue; }
            entities[i].CurrentTurn = false;
            Destroy(entities[i].gameObject);
        }

        entities.Clear();
        Destroy(FindObjectOfType<PlayerControlls>().PlayerHolder);
        GameStarted = false;
        Destroy(GameManager.Instance.gameObject);
        Destroy(gameObject);
    }

    private void ExitCombat()
    {
        inCombat = false;
        entitiesInCombat.Clear();
        CurrentTurn = entities.IndexOf(player);

        SetTurnToPlayer();

        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] == null)
            {
                entities.RemoveAt(i);
                continue;
            }
            entities[i].gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.Restart, Restart);
        EventManager.AddListener(EventType.StartCombat, StartCombat);
        EventManager.AddListener(EventType.GameOver, GameOver);
        EventManager.AddListener(EventType.ExitCombat, ExitCombat);
    }

    private void StartCombat()
    {
        inCombat = true;
        entitiesInCombat.AddRange(entities);
        for (int i = 0; i < entitiesInCombat.Count; i++)
        {
            if (!entitiesInCombat[i].inCombat)
            {
                entitiesInCombat.RemoveAt(i);
            }
        }

        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] == null)
            {
                entities.RemoveAt(i);
                continue;
            }
            if (entities[i].inCombat)
            {
                entities[i].gameObject.SetActive(true);
            }
            else
            {
                entities[i].gameObject.SetActive(false);
            }
        }

        CurrentTurn = entitiesInCombat.IndexOf(player);
    }

    private void Start()
    {
        if (this == null) { return; }
        SetTurnToPlayer();
        victoryScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Invoke(nameof(StartGame), 2f);
    }

    private void StartGame()
    {
        GameStarted = true;
    }

    private void SetTurnToPlayer()
    {
        if (player == null) { return; }
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].CurrentTurn = false;
        }

        CurrentTurn = entities.IndexOf(player);
        player.CurrentTurn = true;
        currentEntity = player;
    }
}