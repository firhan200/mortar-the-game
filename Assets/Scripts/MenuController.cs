using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static bool isFirstAppear = true;

    private void Awake()
    {
        //unload scene
        if(isFirstAppear)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);

            isFirstAppear = false;
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void QuitApplication()
    {
        Application.Quit(0);
    }
}
