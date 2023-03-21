using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Skills")]
public class Skills : ScriptableObject
{
    public List<Item> Items = new();

}
