using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] public GameObject itemMesh;
    [SerializeField] Image image;

    [SerializeField] private Skill skill;

    public InventoryManager InventoryManager { get; private set; }

    private void Awake()
    {
        InventoryManager = GetComponentInParent<InventoryManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager == null)
        {
            InventoryManager = GetComponentInParent<InventoryManager>();
        }

        InventoryManager.RegisterDraggedObject(this);
        InventoryManager.currentSlot = transform.parent;
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.Translate(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryManager.UnregisterDraggedObject(eventData.delta);
        image.raycastTarget = true;
    }
}
