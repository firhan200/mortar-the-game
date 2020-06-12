﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //inputs
    [SerializeField]
    public int totalAmmo = 3;

    [SerializeField]
    public GameObject defaultTargetPrefab;

    [SerializeField]
    public GameObject specialTargetPrefab;

    //local
    int currentAmmo = 3;
    UnityEngine.UI.Text scorePointText;
    UnityEngine.UI.Text scorePointGameOverText;
    UnityEngine.UI.RawImage[] ammoImages;
    CanvasAmmoController canvasAmmoController;
    PowerController powerController;
    CameraController cameraController;
    int point = 0; //set no point, so first increment will be 0
    PlayerData playerData;
    bool isBoxDropping = false;
    int dropCount = 0;

    public void Awake()
    {
        //init all controller
        InitController();

        InitScorePoint();

        //set ammo
        currentAmmo = totalAmmo;

        //hide gameover canvas
        GameObject.Find("Game Over Canvas").GetComponent<Canvas>().enabled = false;

        //draw
        canvasAmmoController.DrawAmmoImages(currentAmmo);

        //drop random box
        DropBox();
    }

    private void Start()
    {
        //load saved game
        playerData = SaveData.LoadPlayerData();

        SelectWeapon(playerData.GetSelectedWeaponIndex());
    }

    void InitScorePoint()
    {
        scorePointText = GameObject.Find("Score Point").GetComponent<UnityEngine.UI.Text>();
        scorePointGameOverText = GameObject.Find("Your Score Text").GetComponent<UnityEngine.UI.Text>();
    }

    void InitController()
    {
        canvasAmmoController = GameObject.Find("Canon Ammo Panel").GetComponent<CanvasAmmoController>();
        powerController = GameObject.Find("Fire Button").GetComponent<PowerController>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    public void ResetAmmo()
    {
        currentAmmo = totalAmmo;
    }

    public void DecreaseAmmo()
    {
        currentAmmo--;

        //redraw
        canvasAmmoController.DrawAmmoImages(currentAmmo);
    }

    public void DropBox()
    {
        //increment drop count
        dropCount++;

        //set is box dropping to prevent double drop
        isBoxDropping = true;

        Vector3 randomBoxPosition = new Vector3(Random.Range(-5.4f, 5.9f), 12f, Random.Range(20, 58));

        //select which box shoul be dropped
        GameObject targetPrefab;
        if (dropCount % 5 == 0)
        {
            //every x box, drop 1 special
            targetPrefab = specialTargetPrefab;
        }
        else
        {
            //default
            targetPrefab = defaultTargetPrefab;
        }

        //clone prefab
        Instantiate(targetPrefab, randomBoxPosition, Quaternion.identity);

        isBoxDropping = false;
    }

    public void IncrementPoint(int targetPoint)
    {
        //increment point
        point = point + targetPoint;

        //draw to score board
        scorePointText.text = point.ToString();
        scorePointGameOverText.text = "Your Score: "+point.ToString();
    }

    public void AmmoHitSomething(bool isHitTarget)
    {
        if (!isHitTarget)
        {
            //decrease ammo
            DecreaseAmmo();
        }
       
        if (currentAmmo > 0)
        {
            //game continue
            //back to first position
            powerController.ResetPower();
            cameraController.ResetCamera();
        }
        else
        {
            //save high score
            SaveHighScore(point);

            //lose
            //hide game canvas
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

            //show game over
            GameObject.Find("Game Over Canvas").GetComponent<Canvas>().enabled = true;
        }

        if (isHitTarget)
        {
            //drop another box
            DropBox();
        }
    }

    void SelectWeapon(int weaponIndex)
    {
        //get weapons holder
        int counter = 0;
        GameObject weapons = GameObject.Find("Weapons") as GameObject;

        foreach(Transform transform in weapons.transform)
        {
            if(counter == weaponIndex)
            {
                transform.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }

            counter++;
        }
    }

    public void SaveHighScore(int newHighScore)
    {
        //check if high score greater than before
        if(newHighScore > playerData.GetHighScore())
        {
            scorePointGameOverText.text = "New High Score: " + point.ToString();

            SaveData.Save(newHighScore, playerData.GetSelectedWeaponIndex());
        }
    }
}
