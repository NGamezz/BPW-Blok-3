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
    [SerializeField] private float walkDistance;
    private Vector3 vel = Vector3.zero;
    private bool isWalking;

    private void Start()
    {
        camera = Camera.main;
        Invoke(nameof(SpawnPlayer), .5f);
    }

    private void SpawnPlayer()
    {
        start = FindObjectOfType<StartTile>().transform;
        player.transform.position = new Vector3(start.position.x, spawnHeight, start.position.z);
    }

    private void PlayerInput()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (isWalking) { return; }
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.TryGetComponent(out Tile target) && Input.GetKey(KeyCode.Mouse0))
            {

                target.HighLight();
                playerMesh.position = Vector3.SmoothDamp(playerMesh.position, new Vector3(objectHit.position.x, spawnHeight, objectHit.position.z), ref vel, walkDistance);
            }
        }
    }

    void Update()
    {
        PlayerInput();
    }
}
