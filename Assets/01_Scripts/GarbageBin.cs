using UnityEngine;
using UnityEngine.EventSystems;

public class GarbageBin : MonoBehaviour, IDropHandler
{
    private PlayerControlls playerControlls;

    public void OnDrop(PointerEventData eventData)
    {
        if (playerControlls.CurrentTurn)
        {
            GameObject dropped = eventData.pointerDrag;
            Item item = dropped.GetComponent<Item>();
            playerControlls.RemovePower(item);
            InventoryManager inventory = item.InventoryManager;
            Destroy(item.gameObject);
            inventory.CurrentItem = null;
        }
    }

    private void Awake()
    {
        playerControlls = FindObjectOfType<PlayerControlls>();
    }

}
