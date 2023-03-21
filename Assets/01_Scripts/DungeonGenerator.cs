using System.Collections.Generic;
using System.Net;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Room
{
    public GameObject RoomObject;
    public Vector2Int MinPosition;
    public Vector2Int MaxPosition;
    public Vector2Int Offset;

    public bool[] ContainsPickup = new bool[4];

    public bool[] status = new bool[4];

    public bool Obligatory;

    public int ProbabilityOfSpawning(int x, int y)
    {
        if (x >= MinPosition.x && x <= MaxPosition.x && y >= MinPosition.y && y <= MaxPosition.y)
        {
            return Obligatory ? 2 : 1;
        }
        return 0;
    }
}

[System.Serializable]
public class Cell
{
    public bool Visited = false;
    //0 North, 1 South, 2 East, 3 West
    public bool[] Status = new bool[4];

    public int x;
    public int y;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Vector2Int startPos;
    [SerializeField] private Vector2Int offSet;
    [SerializeField] private Cell[,] dungeon;
    [SerializeField] private Tile[,] tiles;
    [SerializeField] private Room[] rooms;
    [SerializeField] private Room startRoom;

    private List<Character> entities = new();

    private readonly List<Vector2Int> existingTiles = new();
    private PlayerControlls playerControlls;

    private Vector2Int entityPosition = Vector2Int.zero;

    public Tile GetRandomTile()
    {
        Vector2Int randomPos = existingTiles[Random.Range(0, existingTiles.Count)];
        Tile tile = tiles[randomPos.x, randomPos.y];
        return tile;
    }

    public void ActivateTile(Vector2Int position)
    {
        tiles[position.x, position.y].gameObject.SetActive(true);
    }

    public Vector3 MoveEntity(Tile objectHit, Vector3 currentPosition, Vector2Int gridPosition, Character entity, bool player)
    {
        entityPosition = gridPosition;
        List<Tile> neighbours = ReturnPlayerNeighbours(gridPosition);
        neighbours.Add(objectHit);
        if (neighbours.Count == 0) { return currentPosition; }
        Tile hitTile = null;

        if (random)
        {
            hitTile = neighbours[Random.Range(0, neighbours.Count)];
        }

        foreach (Tile tile in neighbours)
        {
            if (tile == null) { continue; }
            tile.gameObject.SetActive(true);
            tile.Neighbour = true;
            if (tile == objectHit)
            {
                hitTile = tile;
            }
        }

        if (hitTile == null) { return currentPosition; }

        if (hitTile.HasItem)
        {
            if (!entity.skills.Contains(hitTile.Item))
            {
                if (player)
                {
                    hitTile.Item.itemMesh.SetActive(true);

                    bool hasBeenPlaced = false;
                    foreach (InventorySlot slot in entity.inventorySlots)
                    {
                        if (slot.transform.childCount == 0 && !hasBeenPlaced)
                        {
                            hasBeenPlaced = true;
                            hitTile.Item.itemMesh.transform.SetParent(slot.transform);
                            hitTile.Item.itemMesh.transform.localScale = new Vector3(1, 1, 1);
                        }
                    }
                }

                entity.AddPower(hitTile.Item);
                hitTile.HasItem = false;
            }
        }

        entityPosition = hitTile.position;

        foreach (Character character in entities)
        {
            if (character == entity) { continue; }
            if (character.entityPosition == gridPosition)
            {
                Debug.Log("Battle");
            }
        }

        return new Vector3(hitTile.transform.position.x, 2f, hitTile.transform.position.z);
    }
    //Must Update Position After Moving
    public Vector2Int UpdatePosition()
    {
        return entityPosition;
    }

    //Perhaps giving an error because not every value in the grid has a tile
    public List<Tile> ReturnPlayerNeighbours(Vector2Int position)
    {
        List<Tile> neighbours = new();

        if (position.x - 1 >= 0)
        {
            neighbours.Add(tiles[position.x - 1, position.y]);
        }
        if (position.x + 1 < width)
        {
            neighbours.Add(tiles[position.x + 1, position.y]);
        }
        if (position.y - 1 >= 0)
        {
            neighbours.Add(tiles[position.x, position.y - 1]);
        }
        if (position.y + 1 < height)
        {
            neighbours.Add(tiles[position.x, position.y + 1]);
        }
        return neighbours;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Character[] characters = FindObjectsOfType<Character>();
        foreach (Character entity in characters)
        {
            entities.Add(entity);
        }

        tiles = new Tile[width, height];
        playerControlls = FindObjectOfType<PlayerControlls>();
        GenerateGrid();
    }

    private void GenerateDungeon()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell currentCell = dungeon[x, y];

                if (!currentCell.Visited) { continue; }

                int randomRoom = -1;
                List<int> availableRooms = new();

                for (int k = 0; k < rooms.Length; k++)
                {
                    int p = rooms[k].ProbabilityOfSpawning(x, y);

                    if (p == 2)
                    {
                        randomRoom = k;
                        break;
                    }
                    else
                    {
                        availableRooms.Add(k);
                    }
                }

                if (randomRoom == -1)
                {
                    if (availableRooms.Count > 0)
                    {
                        randomRoom = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];
                    }
                    else
                    {
                        randomRoom = 0;
                    }
                }

                if (x == 0 && y == 0)
                {
                    SpawnRoom(randomRoom, true, currentCell.Status, x, y);
                }
                else
                {
                    SpawnRoom(randomRoom, false, currentCell.Status, x, y);
                }
            }
        }
    }

    private void SpawnRoom(int index, bool start, bool[] status, int x, int y)
    {
        var newRoom = Instantiate(start ? startRoom.RoomObject : rooms[index].RoomObject, new Vector3(x * startRoom.Offset.x, 0, -y * startRoom.Offset.y), Quaternion.identity, transform).GetComponent<Tile>();
        existingTiles.Add(new Vector2Int(x, y));
        tiles[x, y] = newRoom;
        newRoom.gameObject.SetActive(start);
        newRoom.UpdateRoom(status);
        newRoom.position = new Vector2Int(x, y);
        newRoom.name += " " + x + "-" + y;
        if (start)
        {
            playerControlls.transform.position = new Vector3(newRoom.transform.position.x, playerControlls.transform.position.y, newRoom.transform.position.z);
            playerControlls.PlayerMesh.position = new Vector3(newRoom.transform.position.x, 2f, newRoom.transform.position.z);
        }
    }

    private void GenerateGrid()
    {
        dungeon = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new(x, y);
                dungeon[x, y] = cell;
            }
        }

        Cell currentCell = new(startPos.x, startPos.y);

        for (int i = 0; i < (width * height); i++)
        {
            dungeon[currentCell.x, currentCell.y].Visited = true;

            List<Cell> neighbours = ReturnNeighbours(dungeon[currentCell.x, currentCell.y]);

            Stack<Cell> path = new();

            if (neighbours.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                Cell newCell = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];

                if (newCell.x > currentCell.x)
                {
                    if (newCell.x - 1 == currentCell.x)
                    {
                        dungeon[currentCell.x, currentCell.y].Status[2] = true;
                        currentCell = newCell;
                        dungeon[currentCell.x, currentCell.y].Status[3] = true;
                    }
                    if (newCell.x + 1 == currentCell.x)
                    {
                        dungeon[currentCell.x, currentCell.y].Status[3] = true;
                        currentCell = newCell;
                        dungeon[currentCell.x, currentCell.y].Status[2] = true;
                    }
                }

                else
                {
                    if (newCell.y - 1 == currentCell.y)
                    {
                        dungeon[currentCell.x, currentCell.y].Status[1] = true;
                        currentCell = newCell;
                        dungeon[currentCell.x, currentCell.y].Status[0] = true;
                    }
                    if (newCell.y + 1 == currentCell.y)
                    {
                        dungeon[currentCell.x, currentCell.y].Status[0] = true;
                        currentCell = newCell;
                        dungeon[currentCell.x, currentCell.y].Status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    private List<Cell> ReturnNeighbours(Cell currentCell)
    {
        List<Cell> neighbours = new();

        if (currentCell.x - 1 >= 0)
        {
            neighbours.Add(dungeon[currentCell.x - 1, currentCell.y]);
        }
        if (currentCell.x + 1 < width)
        {
            neighbours.Add(dungeon[currentCell.x + 1, currentCell.y]);
        }
        if (currentCell.y - 1 >= 0)
        {
            neighbours.Add(dungeon[currentCell.x, currentCell.y - 1]);
        }
        if (currentCell.y + 1 < height)
        {
            neighbours.Add(dungeon[currentCell.x, currentCell.y + 1]);
        }
        return neighbours;
    }
}
