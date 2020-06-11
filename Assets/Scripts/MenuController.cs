using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    GameObject nextButton;

    [SerializeField]
    GameObject prevButton;

    [SerializeField]
    UnityEngine.UI.Text highScoreText;

    public static bool isFirstAppear = true;

    //player current state
    private PlayerData playerData;

    //selected weapon
    int selectedWeaponIndex = 0; //default

    private void Awake()
    {
        //unload scene
        if(isFirstAppear)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);

            isFirstAppear = false;
        }
    }

    private void Start()
    {
        playerData = SaveData.LoadPlayerData();
        if(playerData == null)
        {
            //create default player data
            SaveData.Save(0, 0);
            playerData = SaveData.LoadPlayerData();
        }

        //load high score
        highScoreText.text = "High Score: "+playerData.GetHighScore().ToString();

        //show selected weapon
        selectedWeaponIndex = playerData.GetSelectedWeaponIndex();
        ShowWeapon(selectedWeaponIndex);
    }

    public void NextWeapon()
    {
        selectedWeaponIndex++;

        ShowWeapon(selectedWeaponIndex);
    }

    public void PrevWeapon()
    {
        selectedWeaponIndex = selectedWeaponIndex > 0 ? selectedWeaponIndex - 1 : 0;

        ShowWeapon(selectedWeaponIndex);
    }

    void ShowWeapon(int selectedWeaponIndex)
    {
        int totalWeapon = 0;

        //get weapons holder
        int counter = 0;
        GameObject weapons = GameObject.Find("Weapons Collection") as GameObject;

        foreach (Transform transform in weapons.transform)
        {
            //check if weapon prefab to get total weapon available
            if(transform.gameObject.tag == "Weapon Prefab")
            {
                totalWeapon++;
            }

            if (counter == selectedWeaponIndex)
            {
                transform.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }

            counter++;
        }

        Debug.Log(totalWeapon);

        //hide or show arrow
        if(selectedWeaponIndex == 0)
        {
            //remove prev button
            prevButton.SetActive(false);
            //show next button
            nextButton.SetActive(true);
        }
        else if(selectedWeaponIndex > 0  & selectedWeaponIndex < (totalWeapon - 1))
        {
            //show prev button
            prevButton.SetActive(true);
            //show next button
            nextButton.SetActive(true);
        }
        else
        {
            //show prev button
            prevButton.SetActive(true);
            //remove next button
            nextButton.SetActive(false);
        }
    }

    public void NewGame()
    {
        //set selected weapon
        SaveData.Save(playerData.GetHighScore(), selectedWeaponIndex);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void QuitApplication()
    {
        Application.Quit(0);
    }
}
