using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject[] walls;
    [SerializeField] private GameObject[] doors;
    [SerializeField] private GameObject highLight;
    private bool OnHighLight = false;

    private void Start()
    {
        highLight.SetActive(true);
    }

    private void FixedUpdate()
    {
        highLight.SetActive(OnHighLight ? false : true);
    }

    public void HighLight()
    {
        OnHighLight = true;
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
