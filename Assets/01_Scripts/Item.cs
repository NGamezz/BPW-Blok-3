using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] public GameObject itemMesh;
    [SerializeField] Image image;

    [SerializeField] private Skill skill;
    public Skill Skill { get { return skill; } }

    public InventoryManager InventoryManager { get; private set; }

    private void Awake()
    {
        InventoryManager = GetComponentInParent<InventoryManager>();
    }

    public string GetSkillName()
    {
        string value = "Skill";
        if (Skill == null) { return value; }
        if (Skill.SkillName != null)
        {
            value = Skill.SkillName;
        }
        return value;
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
