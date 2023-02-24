using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] tiles;

    [SerializeField] private int amountOfTilesToGenerate;
    [SerializeField] private int tileSize;
    private Dictionary<int, Transform> GridLocations = new Dictionary<int, Transform>();


    void Start()
    {
    }
}


//tiles[i].transform.position = new Vector3(index, 0, 0);
//index += tileSize / 2;