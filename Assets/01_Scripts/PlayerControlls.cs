using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : Character
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerMesh;
    [SerializeField] private float spawnHeight;
    [SerializeField] private float spawnDelay = .5f;
    [SerializeField] private float walkDistance;
    private new Camera camera;
    private Transform start;

    private void Start()
    {
        camera = Camera.main;
        Invoke(nameof(SpawnPlayer), spawnDelay);
    }

    private void SpawnPlayer()
    {
        start = FindObjectOfType<StartTile>().transform;
        player.transform.position = new Vector3(start.position.x, spawnHeight, start.position.z);
    }

    private void PlayerInput()
    {
        if (!CurrentTurn) { return; }

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.TryGetComponent(out Tile target))
            {
                target.HighLight();
                if (Input.GetMouseButton(0))
                {
                    TurnManager.Instance.ChangeTurn();
                    Vector3 desiredPosition = new(objectHit.position.x, spawnHeight, objectHit.position.z);
                    playerMesh.position = Vector3.MoveTowards(playerMesh.position, desiredPosition, walkDistance);
                }
            }
        }
    }

    void Update()
    {
        PlayerInput();
    }
}
