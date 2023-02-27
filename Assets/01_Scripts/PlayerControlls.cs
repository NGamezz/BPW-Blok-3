using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : MonoBehaviour
{
    private new Camera camera;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerMesh;
    private Transform start;
    [SerializeField] private float spawnHeight;
    [SerializeField] private float spawnDelay = .5f;
    [SerializeField] private float walkDistance;
    private Vector3 vel = Vector3.zero;

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
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            Transform objectHit = hit.transform;
            if (objectHit.TryGetComponent(out Tile target))
            {
                target.HighLight();
                if (Input.GetMouseButton(0))
                {
                    playerMesh.position = Vector3.MoveTowards(playerMesh.position, new Vector3(objectHit.position.x, spawnHeight, objectHit.position.z), walkDistance);
                }
            }
        }
    }

    void Update()
    {
        PlayerInput();
    }
}
