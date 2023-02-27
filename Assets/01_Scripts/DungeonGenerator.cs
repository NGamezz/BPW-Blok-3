using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool Visited = false;
        public bool[] Status = new bool[4];
        public Vector3Int position;
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }
    }

    [SerializeField] private Vector2Int size;
    [SerializeField] private int width;
    [SerializeField] private int height;
    private int startPos;
    [SerializeField] private Vector2 offSet;
    public Rule[] rooms;


    private Cell[,] dungeon;

    private Dictionary<Vector3Int, Cell> cells = new Dictionary<Vector3Int, Cell>();
    private List<Cell> board = new List<Cell>();

    private void Start()
    {
        GenerateDungeonPattern();
    }

    private void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                //cells.TryGetValue(new Vector3Int(i, 0, j), out Cell selectedCell);

                Cell selectedCell = dungeon[i, j];

                //Cell currentCell = board[(i + j * size.x)];
                if (selectedCell.Visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    Tile newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offSet.x, 0, -j * offSet.y), Quaternion.identity, transform).GetComponent<Tile>();
                    newRoom.UpdateRoom(selectedCell.Status);
                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
    }

    private void GenerateDungeonPattern()
    {
        dungeon = new Cell[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Cell cell = new();
                //board.Add(cell);

                dungeon[x, y] = cell;

                //if (cells.ContainsKey(new Vector3Int(x, 0, y))) { return; }
                //cells.Add(new Vector3Int(x, 0, y), cell);
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int t = 0;

        while (t < 1000)
        {
            t++;

            board[currentCell].Visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            List<int> neighbours = CheckNeighbours(currentCell);

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

                int newCel = neighbours[Random.Range(0, neighbours.Count)];

                if (newCel > currentCell)
                {
                    if (newCel - 1 == currentCell)
                    {
                        board[currentCell].Status[2] = true;
                        currentCell = newCel;
                        board[currentCell].Status[3] = true;
                    }
                    else
                    {
                        board[currentCell].Status[1] = true;
                        currentCell = newCel;
                        board[currentCell].Status[0] = true;
                    }
                }
                else
                {
                    if (newCel + 1 == currentCell)
                    {
                        board[currentCell].Status[3] = true;
                        currentCell = newCel;
                        board[currentCell].Status[2] = true;
                    }
                    else
                    {
                        board[currentCell].Status[0] = true;
                        currentCell = newCel;
                        board[currentCell].Status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    private List<int> CheckNeighbours(int cell)
    {
        List<int> neighbours = new List<int>();
        if (cell - size.x >= 0 && !board[(cell - size.x)].Visited)
        {
            neighbours.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].Visited)
        {
            neighbours.Add(cell + size.x);
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].Visited)
        {
            neighbours.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].Visited)
        {
            neighbours.Add((cell - 1));
        }

        return neighbours;
    }

}
