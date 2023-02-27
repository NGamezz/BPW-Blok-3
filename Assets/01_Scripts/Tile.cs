using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject[] walls;
    [SerializeField] private GameObject[] doors;
    [SerializeField] private GameObject highLight;

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

    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
