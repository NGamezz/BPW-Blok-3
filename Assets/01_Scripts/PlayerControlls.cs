using UnityEngine;

public class PlayerControlls : Character
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerMesh;
    public Transform PlayerMesh { get { return playerMesh; } }
    [SerializeField] private float spawnHeight;
    [SerializeField] private float walkDistance;
    private new Camera camera;

    private void Start()
    {
        camera = Camera.main;
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
                    //TurnManager.Instance.ChangeTurn();
                    DungeonGenerator.Instance.MovePlayer(target, false);
                }
            }
        }
    }

    void Update()
    {
        PlayerInput();
    }
}
