using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Transform currentSlot;

    public Item CurrentItem = null;

    public void RegisterDraggedObject(Item item)
    {
        CurrentItem = item;
    }

    public void UnregisterDraggedObject(Vector2 position)
    {
        CurrentItem.transform.SetParent(currentSlot);
        CurrentItem.transform.localPosition = Vector3.zero;
        CurrentItem = null;
    }
}
