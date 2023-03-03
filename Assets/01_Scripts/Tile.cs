using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject[] walls;
    [SerializeField] private GameObject[] doors;
    [SerializeField] private GameObject highLight;
    public bool[] Status = new bool[4];

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
