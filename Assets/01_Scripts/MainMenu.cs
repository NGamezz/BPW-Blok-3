using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int dungeonSceneIndex;
    [SerializeField] private int tutorialSceneIndex;

    private void Start()
    {
        GameManager[] gameObjects = FindObjectsOfType<GameManager>();
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i].gameObject);
        }
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(dungeonSceneIndex);
    }

    public void TutorialGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(tutorialSceneIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
