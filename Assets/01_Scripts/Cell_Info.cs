using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell_Info
{
    public bool IsCollapsed { get; private set; }
    public Vector3 CellCoordinate { get; private set; }
    public List<SingleState> SuperPosition = new();

}

[System.Serializable]
public class SingleState
{
    public GameObject Prefab;
    public int RotationIndex;
    public string front_socket;
    public string back_socket;
    public string left_socket;
    public string right_socket;

}
//tiles[i].transform.position = new Vector3(index, 0, 0);
//index += tileSize / 2;