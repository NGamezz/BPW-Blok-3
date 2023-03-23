using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Enemy : Character
{
    [SerializeField] private GameObject enemyMesh;
    [SerializeField] private int currentAmountOfTurns;
    private int maxAmountOfTurns;
    private bool allowTurn = true;
    [SerializeField] private float delay;

    private void Start()
    {
        Invoke(nameof(OnStart), 2f);
        maxAmountOfTurns = currentAmountOfTurns;
    }

    private void OnStart()
    {
        Tile randomTile = DungeonGenerator.Instance.GetRandomTile();
        Vector3 position = randomTile.gameObject.transform.position;

        entityPosition = new Vector2Int(randomTile.position.x, randomTile.position.y);
        transform.position = new Vector3(position.x, 2f, position.z);
        DungeonGenerator.Instance.ActivateTile(entityPosition);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartCombat, StartCombat);
        EventManager.AddListener(EventType.ExitCombat, ExitCombat);
    }

    private void ExitCombat()
    {

    }


    private void StartCombat()
    {
        enemyMesh.SetActive(false);
    }
    private void Update()
    {
        if (CurrentTurn)
        {
            if (currentAmountOfTurns <= 0)
            {
                TurnManager.Instance.ChangeTurn();
                currentAmountOfTurns = maxAmountOfTurns;
                CurrentTurn = false;
            }
            else if (allowTurn)
            {
                StartCoroutine(TurnDelay());
            }
        }
    }

    private IEnumerator TurnDelay()
    {
        allowTurn = false;
        currentAmountOfTurns--;

        List<Tile> neighbours = DungeonGenerator.Instance.ReturnPlayerNeighbours(entityPosition);
        Tile targetTile = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
        gameObject.transform.position = DungeonGenerator.Instance.MoveEntity(targetTile, transform.position, entityPosition, this, false);

        yield return new WaitForSeconds(delay);
        allowTurn = true;
    }
}
