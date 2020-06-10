using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //input
    [SerializeField]
    public int totalAmmo = 3;

    [SerializeField]
    public GameObject boxPrefab;

    //local
    int currentAmmo = 3;
    UnityEngine.UI.Text scorePointText;
    UnityEngine.UI.Text scorePointGameOverText;
    UnityEngine.UI.RawImage[] ammoImages;
    CanvasAmmoController canvasAmmoController;
    PowerController powerController;
    CameraController cameraController;
    int point = 0; //set no point, so first increment will be 0

    public void Awake()
    {
        //init all controller
        InitController();

        InitScorePoint();

        SelectWeapon(1);

        //set ammo
        currentAmmo = totalAmmo;

        //hide gameover canvas
        GameObject.Find("Game Over Canvas").GetComponent<Canvas>().enabled = false;

        //draw
        canvasAmmoController.DrawAmmoImages(currentAmmo);

        //drop random box
        DropBox();
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
        //draw
        canvasAmmoController.DrawAmmoImages(currentAmmo);
    }

    public void DropBox()
    {
        Vector3 randomBoxPosition = new Vector3(Random.Range(-20, 25), 12f, Random.Range(20, 60));
        GameObject box = Instantiate(boxPrefab, randomBoxPosition, Quaternion.identity) as GameObject;
    }

    public void IncrementPoint()
    {
        //increment point
        point = point + 1;

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
            //lose
            //hide game canvas
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

            //show game over
            GameObject.Find("Game Over Canvas").GetComponent<Canvas>().enabled = true;
        }

        if (isHitTarget)
        {
            //increment point
            IncrementPoint();

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
}
