using System.Collections;
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

    [SerializeField]
    public GameObject gotAmmoPrefab;

    [SerializeField]
    public GameObject hitAnimPrefab;

    //local
    int currentAmmo = 3;
    TMPro.TextMeshProUGUI scorePointText;
    TMPro.TextMeshProUGUI scorePointGameOverText;
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
        scorePointText = GameObject.Find("Score Point").GetComponent<TMPro.TextMeshProUGUI>();
        scorePointGameOverText = GameObject.Find("Your Score Text").GetComponent<TMPro.TextMeshProUGUI>();
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

    public void IncreaseAmmo(int increaseNumber)
    {
        //show got ammo animation
        GotAmmo();

        //check if already max
        if(currentAmmo < totalAmmo)
        {
            currentAmmo = currentAmmo + increaseNumber;

            //redraw
            canvasAmmoController.DrawAmmoImages(currentAmmo);
        }
    }

    public void GotAmmo()
    {
        //get canvas
        GameObject canvas = GameObject.Find("Canvas");

        GameObject gotAmmo = Instantiate(gotAmmoPrefab, canvas.transform) as GameObject;

        Destroy(gotAmmo, 1f);
    }

    public void HitAnimation()
    {
        //get canvas
        GameObject canvas = GameObject.Find("Canvas");

        GameObject hitAnim = Instantiate(hitAnimPrefab, canvas.transform) as GameObject;

        Destroy(hitAnim, 1f);
    }

    public void DropBox()
    {
        //increment drop count
        dropCount++;

        //set is box dropping to prevent double drop
        isBoxDropping = true;

        Vector3 randomBoxPosition = new Vector3(Random.Range(-5.4f, 5.9f), 12f, Random.Range(12f, 30f));

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
