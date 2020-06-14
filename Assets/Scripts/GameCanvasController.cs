using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCanvasController : MonoBehaviour
{
    GameObject quitConfirmationPanel;
    GameController gameController;

    private void Start()
    {
        quitConfirmationPanel = GameObject.Find("Quit Confirmation Panel");
        quitConfirmationPanel.SetActive(false);

        gameController = GameObject.Find("World").GetComponent<GameController>();
    }

    public void BackToMenu()
    {
        //save high score
        gameController.SaveCurrentScore();

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }

    public void ShowConfirmationDialog()
    {
        quitConfirmationPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        quitConfirmationPanel.SetActive(false);
    }
}
