using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[System.Serializable]
public class Room
{
    public GameObject room;
    public Vector2Int minPosition;
    public Vector2Int maxPosition;

    public bool[] status = new bool[4];

    public bool Obligatory;

    public int ProbabilityOfSpawning(int x, int y)
    {
        if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
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

public class Grid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Vector2Int startPos;
    [SerializeField] private Vector2Int offSet;
    [SerializeField] private Cell[,] dungeon;

    [SerializeField] private Room[] rooms;
    [SerializeField] private Room[] roomsWithoutStart;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateDungeon()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell currentCell = dungeon[x, y];

                if (currentCell.Visited)
                {
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

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(x * offSet.x, 0, -y * offSet.y), Quaternion.identity, transform).GetComponent<Tile>();
                    newRoom.UpdateRoom(currentCell.Status);
                    newRoom.Status = currentCell.Status;
                    newRoom.name += " " + x + "-" + y;
                }
            }
        }
        CheckForDuplicate();
    }

    private void CheckForDuplicate()
    {
        StartTile[] startTile = FindObjectsOfType<StartTile>();
        foreach (StartTile tile in startTile)
        {
            if (tile.name.Contains("0-0")) { break; }
            Tile tileComponent = tile.GetComponent<Tile>();
            var tempRoom = Instantiate(roomsWithoutStart[UnityEngine.Random.Range(0, roomsWithoutStart.Length)].room, tile.transform.position, Quaternion.identity, transform).GetComponent<Tile>();
            tempRoom.UpdateRoom(tileComponent.Status);
            Debug.Log(tile.name + " removed.");
            Destroy(tile.gameObject);
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

        int t = 0;
        while (t < 500)
        {
            t++;

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

                //0 North, 1 South, 2 East, 3 West
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
