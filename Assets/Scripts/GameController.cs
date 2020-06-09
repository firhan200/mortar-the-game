using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int totalAmmo = 3;
    public GameObject boxPrefab;
    public int currentAmmo = 3;
    public UnityEngine.UI.Text scorePointText;
    public UnityEngine.UI.Text scorePointGameOverText;
    UnityEngine.UI.RawImage[] ammoImages;
    CanvasAmmoController canvasAmmoController;
    public int point = 0; //set no point, so first increment will be 0

    public void Awake()
    {
        currentAmmo = totalAmmo;
        //hide gameover canvas
        GameObject.Find("Game Over Canvas").GetComponent<Canvas>().enabled = false;

        canvasAmmoController = GameObject.Find("Canon Ammo Panel").GetComponent<CanvasAmmoController>();

        //draw
        canvasAmmoController.DrawAmmoImages(currentAmmo);

        DropBox();
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
        Debug.Log("Got Point!");

        //increment point
        point = point + 1;

        //draw to score board
        scorePointText.text = point.ToString();
        scorePointGameOverText.text = "Your Score: "+point.ToString();
    }
}
