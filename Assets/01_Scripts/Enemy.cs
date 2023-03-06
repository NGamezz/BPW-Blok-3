using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private GameObject enemyMesh;

    private void Start()
    {
        //Invoke(nameof(OnStart), 5f);
    }

    private void OnStart()
    {
        Vector3 position = DungeonGenerator.Instance.RandomSpawnLocation();
        transform.position = new Vector3(position.x, 2f, position.z);
    }

    private void FixedUpdate()
    {
        if (CurrentTurn)
        {
            if (Physics.SphereCast(transform.position, 2f, -transform.up, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent<Tile>(out Tile target))
                {
                    DungeonGenerator.Instance.MovePlayer(target, true);
                    TurnManager.Instance.ChangeTurn();
                }
            }
        }
    }
}
