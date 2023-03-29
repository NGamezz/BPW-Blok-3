using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Enemy : Character
{
    [SerializeField] private GameObject enemyMesh;
    [SerializeField] private GameObject enemyCombatObject;
    [SerializeField] private float delay;
    private int maxAmountOfTurns;
    [SerializeField] private bool allowTurn = true;
    private bool canAttack = true;
    private AudioSource audioSource;
    private bool pauze = false;

    public override void HealthDepletedAction()
    {
        GameManager.Instance.Entities.Remove(this);
        base.HealthDepletedAction();
        Destroy(gameObject);
    }

    private void Start()
    {
        GameManager.Instance.SetEnemies(this);

        audioSource = GetComponent<AudioSource>();
        Invoke(nameof(OnStart), 2f);

        maxHealth = Health;

        maxAmountOfTurns = currentAmountOfTurns;
        healthSlider.value = healthSlider.maxValue;
    }

    private void OnStart()
    {
        Tile randomTile = DungeonGenerator.Instance.GetRandomTile();
        Vector3 position = randomTile.gameObject.transform.position;

        allowTurn = true;

        if (enemyMesh != null)
        {
            enemyMesh.SetActive(true);
            enemyCombatObject.SetActive(false);
        }

        entityPosition = new Vector2Int(randomTile.position.x, randomTile.position.y);
        transform.position = new Vector3(position.x, 2f, position.z);
        DungeonGenerator.Instance.ActivateTile(entityPosition);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.Pauze, () => pauze = true);
        EventManager.AddListener(EventType.Resume, () => pauze = false);
        EventManager.AddListener(EventType.StartCombat, StartCombat);
        EventManager.AddListener(EventType.ExitCombat, ExitCombat);
    }

    private void ExitCombat()
    {
        allowTurn = true;
        inCombat = false;

        if (enemyMesh != null)
        {
            enemyMesh.SetActive(true);
            enemyCombatObject.SetActive(false);
        }

        if (this == null) { return; }
        Invoke(nameof(OnStart), 2f);
    }

    private void StartCombat()
    {
        if (this == null || !inCombat) { return; }
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].Skill.Initialize(this);
        }

        if (enemyMesh != null)
        {
            enemyMesh.SetActive(false);
        }
        if (enemyCombatObject != null)
        {
            enemyCombatObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (pauze) { return; }
        if (CurrentTurn && !inCombat)
        {
            if (allowTurn)
            {
                StartCoroutine(TurnDelay());
            }
            if (currentAmountOfTurns <= 0)
            {
                GameManager.Instance.ChangeTurn();
                currentAmountOfTurns = maxAmountOfTurns;
                CurrentTurn = false;
            }
        }
        if (CurrentTurn && inCombat && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        if (this == null) { yield break; }
        canAttack = false;
        Item randomItem = skills[Random.Range(0, skills.Count)];
        randomItem.Skill.Attack();

        yield return new WaitForSeconds(delay);

        GameManager.Instance.ChangeTurn();
        canAttack = true;
    }

    private IEnumerator TurnDelay()
    {
        if (this == null) { yield break; }
        allowTurn = false;
        currentAmountOfTurns--;

        audioSource.Play();
        List<Tile> neighbours = DungeonGenerator.Instance.ReturnPlayerNeighbours(entityPosition);
        if (neighbours.Count == 0) { yield break; }
        Tile targetTile = neighbours[Random.Range(0, neighbours.Count)];
        gameObject.transform.position = DungeonGenerator.Instance.MoveEntity(targetTile, transform.position, entityPosition, this, false);
        gameObject.transform.position = DungeonGenerator.Instance.MoveEntity(targetTile, transform.position, entityPosition, this, false);

        yield return new WaitForSeconds(delay);


        allowTurn = true;
    }
}
