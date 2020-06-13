using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCanvasController : MonoBehaviour
{
    GameObject quitConfirmationPanel;

    private void Start()
    {
        quitConfirmationPanel = GameObject.Find("Quit Confirmation Panel");
        quitConfirmationPanel.SetActive(false);
    }

    public void BackToMenu()
    {
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
