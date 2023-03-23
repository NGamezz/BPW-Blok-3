using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{

    private List<Character> entities = new();

    private void Awake()
    {
        Character[] characters = FindObjectsOfType<Character>();
        foreach (Character character in characters)
        {
            entities.Add(character);
        }
    }

    void Start()
    {
        foreach (Character character in entities)
        {
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
