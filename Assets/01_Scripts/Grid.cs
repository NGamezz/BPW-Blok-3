using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    private Cell[,] dungeon;

    [SerializeField] private List<Cell> neighbours = new();


    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        dungeon = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new();
                dungeon[x, y] = cell;
            }
        }
        neighbours = GetNeighbours(dungeon, new Vector2Int(0, 0));
        Debug.Log(neighbours.Count);
    }

    private List<Cell> GetNeighbours(Cell[,] cells, Vector2Int pos)
    {
        List<Cell> neighbours = new();


        if (cells[pos.x, (pos.y + 1)] != null)
        {
            neighbours.Add(cells[pos.x, (pos.y + 1)]);
        }
        if (cells[pos.x, (pos.y + 1)] != null)
        {
            neighbours.Add(cells[pos.x, (pos.y + 1)]);
        }
        if (cells[pos.x, (pos.y - 1)] != null)
        {
            neighbours.Add(cells[pos.x, (pos.y - 1)]);
        }
        if (cells[(pos.x + 1), (pos.y + 1)] != null)
        {
            neighbours.Add(cells[(pos.x + 1), (pos.y + 1)]);
        }
        if (cells[(pos.x - 1), (pos.y - 1)] != null)
        {
            neighbours.Add(cells[(pos.x - 1), (pos.y - 1)]);
        }
        if (cells[(pos.x + 1), (pos.y + 1)] != null)
        {
            neighbours.Add(cells[(pos.x + 1), (pos.y + 1)]);
        }
        if (cells[(pos.x - 1), pos.y] != null)
        {
            neighbours.Add(cells[(pos.x - 1), pos.y]);
        }


        return neighbours;
    }


}

public class Cell
{
    public bool Visited = false;
    public bool[] Status = new bool[4];
    public Vector3Int position;
}