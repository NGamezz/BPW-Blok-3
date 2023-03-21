using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool neighbour;
    public bool Neighbour
    {
        get
        {
            return neighbour;
        }
        set
        {
            gameObject.SetActive(value);
            neighbour = value;
        }
    }

    [SerializeField] private bool hasItem = false;
    public bool HasItem
    {
        get
        {
            return hasItem;
        }

        set
        {
            hasItem = value;
            pickup.SetActive(value);
        }
    }

    public Item Item;

    [SerializeField] private GameObject pickup;

    public GameObject Pickup { get { return pickup; } }

    [SerializeField] private GameObject[] walls;
    [SerializeField] private GameObject[] doors;
    [SerializeField] private GameObject highLight;
    [SerializeField] private GameObject neighbourHighlight;

    public bool[] Status = new bool[4];
    public Vector2Int position;

    private void Awake()
    {
        highLight = FindObjectOfType<HighLight>().gameObject;
    }

    private void Start()
    {
        highLight.SetActive(true);
    }

    public void HighLight()
    {
        highLight.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
    }

    public void UpdateRoom(bool[] _status)
    {
        for (int i = 0; i < _status.Length; i++)
        {
            doors[i].SetActive(_status[i]);
            walls[i].SetActive(!_status[i]);
        }
    }
}
