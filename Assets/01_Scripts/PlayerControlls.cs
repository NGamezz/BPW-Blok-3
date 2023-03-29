using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControlls : Character
{
    public GameObject PlayerHolder { get { return playerHolder; } }
    public Transform PlayerMesh { get { return playerMesh; } }

    [SerializeField] private Transform player;
    [SerializeField] private Transform playerMesh;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject playerHolder;
    [SerializeField] private GameObject menuObject;

    [SerializeField] private TMP_Text currentTurnText;

    [SerializeField] private GameObject dungeonUI;
    [SerializeField] private GameObject combatUI;

    [SerializeField] private float spawnHeight;
    [SerializeField] private float walkDistance;

    [SerializeField] private int combatSceneIndex;
    [SerializeField] private int dungeonSceneIndex;
    [SerializeField] private int mainMenuSceneIndex;

    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private Button[] buttons = new Button[3];
    [SerializeField] private TMP_Text[] buttonTexts = new TMP_Text[3];

    private new Camera camera;

    private bool inMenu = false;

    private int maxAmountOfTurns;

    public void MainMenu()
    {
        EventManager.InvokeEvent(EventType.Restart);
    }

    public void Resume()
    {
        inMenu = false;
        menuObject.SetActive(false);
        EventManager.InvokeEvent(EventType.Resume);
    }

    public override void ChangeTurnAction(bool value)
    {
        if (this == null) { return; }
        currentTurnText.text = "Current Turn : " + CurrentTurn;
        if (inCombat)
        {
            buttonHolder.SetActive(value);
        }
    }

    public override void HealthDepletedAction()
    {
        base.HealthDepletedAction();
        EventManager.InvokeEvent(EventType.GameOver);
        GameManager.Instance.Entities.Remove(this);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    private void GameOver()
    {
        if (this == null) { return; }
        inCombat = false;
        playerMesh.gameObject.SetActive(false);
        dungeonUI.SetActive(false);
        combatUI.SetActive(false);
        buttonHolder.SetActive(false);
        Destroy(playerHolder);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        maxHealth = Health;
        healthSlider.value = healthSlider.maxValue;
        playerMesh.gameObject.SetActive(true);
        dungeonUI.SetActive(true);
        combatUI.SetActive(false);
        camera = Camera.main;
        maxAmountOfTurns = currentAmountOfTurns;

        GameManager.Instance.SetPlayer(this);
    }

    private void StartCombat()
    {
        if (this == null) { return; }
        if (currentTurnText != null)
        {
            currentTurnText.gameObject.SetActive(false);
        }
        dungeonUI.SetActive(false);
        combatUI.SetActive(true);
        playerMesh.gameObject.SetActive(false);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].transform.childCount != 0)
            {
                Item currentItem = inventorySlots[i].GetComponentInChildren<Item>();
                if (skills.Contains(currentItem)) { continue; }
                skills.Add(currentItem);
            }
        }

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].Skill.Initialize(this);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        buttonHolder.SetActive(CurrentTurn);

        for (int i = 0; i < skills.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].onClick.AddListener(skills[i].Skill.Attack);
            buttonTexts[i].text = skills[i].GetSkillName();
        }

        SceneManager.LoadScene(combatSceneIndex);
    }

    private void ExitCombat()
    {
        if (this == null) { return; }
        currentAmountOfTurns = maxAmountOfTurns;
        inCombat = false;
        if (currentTurnText != null)
        {
            currentTurnText.gameObject.SetActive(true);
        }
        combatUI.SetActive(false);
        playerMesh.gameObject.SetActive(true);
        dungeonUI.SetActive(true);
        buttonHolder.SetActive(false);
        SceneManager.LoadScene(dungeonSceneIndex);
    }

    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
        {
            inMenu = true;
            EventManager.InvokeEvent(EventType.Pauze);
            menuObject.SetActive(inMenu);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && inMenu)
        {
            inMenu = false;
            EventManager.InvokeEvent(EventType.Resume);
            menuObject.SetActive(inMenu);
        }

        if (!CurrentTurn || inMenu) { return; }

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;

            if (!objectHit.TryGetComponent(out Tile target)) { return; }

            target.HighLight();
            if (Input.GetMouseButtonDown(0))
            {
                currentAmountOfTurns--;
                playerMesh.position = DungeonGenerator.Instance.MoveEntity(target, playerMesh.position, entityPosition, this, true);
                playerMesh.position = DungeonGenerator.Instance.MoveEntity(target, playerMesh.position, entityPosition, this, true);
                EventManager.InvokeEvent(EventType.ShakeCamera);
                audioSource.Play();

                EventManager.InvokeEvent(EventType.MovePlayer);

                if (currentAmountOfTurns <= 0)
                {
                    GameManager.Instance.ChangeTurn();
                    currentAmountOfTurns = maxAmountOfTurns;

                }
            }
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.Restart, GoToMainMenu);
        EventManager.AddListener(EventType.StartCombat, StartCombat);
        EventManager.AddListener(EventType.ExitCombat, ExitCombat);
        EventManager.AddListener(EventType.GameOver, GameOver);
    }

    private void Update()
    {
        PlayerInput();
    }
}
