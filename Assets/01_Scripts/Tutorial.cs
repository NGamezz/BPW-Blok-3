using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] objects = new GameObject[4];

    private bool[] status = new bool[5];

    [SerializeField] private GameObject tutorialCompleted;

    private int objectIndex = 0;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == objects[objectIndex])
            {
                objects[i].SetActive(true);
            }
            else
            {
                objects[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (status[0] == true) { return; }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            objects[objectIndex].SetActive(false);
            status[0] = true;
            objectIndex = 1;
            objects[objectIndex].SetActive(true);
        }
    }

    private void MoveCheck()
    {
        if (status[1] == true) { return; }
        if (objectIndex == 1)
        {
            objects[objectIndex].SetActive(false);
            status[1] = true;
            objectIndex = 2;
            objects[objectIndex].SetActive(true);
        }
    }

    private void ExitCombatTutorial()
    {
        if (status[4] == true || this == null || objectIndex != 4) { return; }
        objects[objectIndex].SetActive(false);
        status[4] = true;
        tutorialCompleted.SetActive(true);
        Invoke(nameof(ReturnToMainMenu), 2f);
    }

    private void DropItemCheck()
    {
        if (status[2] == true || this == null || objectIndex != 2) { return; }
        status[2] = true;
        objects[objectIndex].SetActive(false);
        objectIndex = 3;
        objects[objectIndex].SetActive(true);
    }

    private void PickUpItemCheck()
    {
        if (status[3] == true || this == null || objectIndex != 3) { return; }
        objects[objectIndex].SetActive(false);
        status[3] = true;
        objectIndex = 4;
        objects[objectIndex].SetActive(true);
    }

    private void RestartTutorialObject()
    {
        if (this == null) { return; }
        Destroy(gameObject);
    }

    private void ReturnToMainMenu()
    {
        EventManager.InvokeEvent(EventType.Restart);
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.MovePlayer, MoveCheck);
        EventManager.AddListener(EventType.ItemDrop, DropItemCheck);
        EventManager.AddListener(EventType.PickupItem, PickUpItemCheck);
        EventManager.AddListener(EventType.ExitCombat, ExitCombatTutorial);
        EventManager.AddListener(EventType.Restart, RestartTutorialObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.MovePlayer, MoveCheck);
        EventManager.RemoveListener(EventType.ItemDrop, DropItemCheck);
        EventManager.RemoveListener(EventType.ExitCombat, ExitCombatTutorial);
        EventManager.RemoveListener(EventType.Restart, RestartTutorialObject);
    }
}