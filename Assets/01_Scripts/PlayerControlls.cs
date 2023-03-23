using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlls : Character
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerMesh;

    [SerializeField] private TMP_Text currentTurnText;

    [SerializeField] private GameObject dungeonUI;
    [SerializeField] private GameObject combatUI;

    [SerializeField] private Slider healthSlider;

    public Transform PlayerMesh { get { return playerMesh; } }

    [SerializeField] private float spawnHeight;
    [SerializeField] private float walkDistance;

    [SerializeField] private int combatSceneIndex;
    [SerializeField] private int dungeonSceneIndex;

    [SerializeField] private Button[] buttons = new Button[3];
    [SerializeField] private TMP_Text[] buttonTexts = new TMP_Text[3];

    private new Camera camera;

    [SerializeField] private int currentAmountOfTurns;
    private int maxAmountOfTurns;

    public override void ChangeTurnAction()
    {
        currentTurnText.text = "Current Turn : " + CurrentTurn;
    }

    private void Start()
    {
        skillsScriptableObject.Items.Clear();
        playerMesh.gameObject.SetActive(true);
        dungeonUI.SetActive(true);
        combatUI.SetActive(false);
        camera = Camera.main;
        maxAmountOfTurns = currentAmountOfTurns;
    }

    private void StartCombat()
    {
        currentTurnText.gameObject.SetActive(false);
        dungeonUI.SetActive(false);
        combatUI.SetActive(true);
        playerMesh.gameObject.SetActive(false);

        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.transform.childCount != 0)
            {
                Item currentItem = inventorySlot.GetComponentInChildren<Item>();
                if (skillsScriptableObject.Items.Contains(currentItem)) { continue; }
                skillsScriptableObject.Items.Add(currentItem);
                Debug.Log(currentItem);
            }
        }

        skills.Clear();
        skills.AddRange(skillsScriptableObject.Items);

        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        for (int i = 0; i < skills.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].onClick.AddListener(skills[i].Skill.Attack);
            buttonTexts[i].text = skills[i].GetSkillName();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(combatSceneIndex);
    }

    private void ExitCombat()
    {
        currentTurnText.gameObject.SetActive(true);
        combatUI.SetActive(false);
        playerMesh.gameObject.SetActive(true);
        dungeonUI.SetActive(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene(dungeonSceneIndex);
    }

    private void PlayerInput()
    {
        if (!CurrentTurn) { return; }

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;

            if (!objectHit.TryGetComponent(out Tile target)) { return; }

            target.HighLight();
            if (Input.GetMouseButtonDown(0))
            {
                if (currentAmountOfTurns <= 0)
                {
                    TurnManager.Instance.ChangeTurn();
                    currentAmountOfTurns = maxAmountOfTurns;
                }
                else
                {
                    currentAmountOfTurns--;
                    playerMesh.position = DungeonGenerator.Instance.MoveEntity(target, playerMesh.position, entityPosition, this, true);
                    playerMesh.position = DungeonGenerator.Instance.MoveEntity(target, playerMesh.position, entityPosition, this, true);
                    EventManager.InvokeEvent(EventType.ShakeCamera);
                }
            }
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartCombat, StartCombat);
        EventManager.AddListener(EventType.ExitCombat, ExitCombat);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.ExitCombat, ExitCombat);
        EventManager.RemoveListener(EventType.StartCombat, StartCombat);
    }

    public void ChangeAmountOfTurns(int amount)
    {
        maxAmountOfTurns += amount;
        currentAmountOfTurns += amount;
    }

    void Update()
    {
        PlayerInput();
    }
}
