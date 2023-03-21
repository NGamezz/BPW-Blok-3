using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : Character
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerMesh;

    [SerializeField] private Skills skillsScriptableObject;

    public Transform PlayerMesh { get { return playerMesh; } }
    [SerializeField] private float spawnHeight;
    [SerializeField] private float walkDistance;

    private new Camera camera;

    [SerializeField] private int currentAmountOfTurns;
    private int maxAmountOfTurns;

    private void Start()
    {
        camera = Camera.main;
        maxAmountOfTurns = currentAmountOfTurns;
        StartCombat();
    }

    private void StartCombat()
    {
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
                    entityPosition = DungeonGenerator.Instance.UpdatePosition();
                    EventManager.InvokeEvent(EventType.ShakeCamera);
                }
            }
        }
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
