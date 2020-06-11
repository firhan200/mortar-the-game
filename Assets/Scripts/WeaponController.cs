using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public float maximumSideRotationAngle = 70f;

    [SerializeField]
    public float maximumUpRotationAngle = 70f;

    [SerializeField]
    public float maximumBottomRotationAngle = 0f;

    [SerializeField]
    public bool isFire = false;

    [SerializeField]
    [Range(10, 100)]
    public float firePower = 50f;

    [SerializeField]
    public float rotationSpeed = 100f;

    [SerializeField]
    public GameObject ammoPrefab;

    //effect prefabs
    [SerializeField]
    public GameObject fireEffect;

    [SerializeField]
    public bool previewMode = false;

    //dependency game object
    //fire button controller
    private PowerController powerController;

    //weapon children
    private GameObject weapon;

    //audio source
    AudioSource fireSFX;
    AudioSource movementSFX;

    //weapon animator
    private Animator weaponAnimator;

    //particle effect
    GameObject sparks;

    //touch screen range
    float touchRangePercent = 60f;
    float minBottomTouchPosition, maxTopTouchPosition, minLeftTouchPosition, maxRightTouchPosition;

    //touch aim
    bool isSwiping = false;
    Touch initialTouch;

    private void Start()
    {
        /* initialize */
        if (!previewMode)
        {
            //get screen width and height
            CalculateAvailableScreenTouch();

            GetPowerButton();

            GetWeaponObject();

            GetAudioSource();

            weaponAnimator = GetComponent<Animator>();
        }
    }

    void GetAudioSource()
    {
        //get audio source
        AudioSource[] audios = GetComponents<AudioSource>();
        if (audios != null && audios.Length > 1)
        {
            fireSFX = audios[0];
            movementSFX = audios[1];
        }

        if(audios == null)
        {
            Debug.Log("Please insert fire SFX and movement SFX Audio Source");
        }
    }

    void GetWeaponObject()
    {
        //get chidlrens
        var childrensTransform = GetComponentsInChildren<Transform>();
        foreach (var childrenTransform in childrensTransform)
        {
            if (childrenTransform.gameObject.tag == "Weapon")
            {
                weapon = childrenTransform.gameObject;
            }
        }
    }

    void GetPowerButton()
    {
        powerController = GameObject.Find("Fire Button").GetComponent<PowerController>();
    }

    private void FixedUpdate()
    {
        if (!previewMode)
        {
            Fire();
            AimWithTouch();
        }
        else
        {
            PreviewMode();
        }
    }

    void PreviewMode()
    {
        //rotate 
        transform.Rotate(0f, 50f * Time.fixedDeltaTime, 0f);
    }

    void Fire()
    {
        if (powerController.isShot)
        {
            isFire = true;
        }

        if (powerController.isShot)
        {
            //duplicate
            GameObject ammo = Instantiate(ammoPrefab, new Vector3(transform.localPosition.x, transform.localPosition.y + weapon.transform.localPosition.y, transform.localPosition.z), Quaternion.Euler(
                    weapon.transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    0f
                )) as GameObject;

            //start fire effect
            StartCoroutine(PlayFireEffect());

            //start fire animation
            StartCoroutine(PlayWeaponFireAnimation());

            //play audio sfx
            PlayFireSFX();

            //Get Rigid Body
            Rigidbody canonBallRB = ammo.GetComponent<Rigidbody>();

            //calculate fire power
            float totalFirePower = powerController.powerSlider.value * firePower;

            //unfreeze ball
            canonBallRB.constraints = RigidbodyConstraints.None;

            //add force to ball
            canonBallRB.AddForce(ammo.transform.forward * totalFirePower, ForceMode.Impulse);

            //to prevent multiple shot
            isFire = false;
            powerController.isShot = false;
        }
    }

    void AimWithTouch()
    {
        bool isTouch = Input.touchCount > 0 ? true : false;
        if (isTouch)
        {
            var touch = Input.GetTouch(0);

            //get touch position
            Vector3 touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                //user start touch, store in var
                initialTouch = touch;
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                //user moved touch
                isSwiping = true;

                //check swipe direction
                var deltaX = touchPosition.x - initialTouch.position.x; //greater than 0 is right and less than zero is left
                var deltaY = touchPosition.y - initialTouch.position.y; //greater than 0 is up and less than zero is down

                //check if only on square available position
                if (IsOnAvailableMoveScreen(initialTouch.position))
                {
                    //check horizontal normalized
                    float horizontalInput = deltaX / Screen.width * 3f;
                    //check vertical normalized
                    float verticalInput = deltaY / Screen.height * 3f;

                    //play audio sfx
                    PlayMovementSFX();

                    //user aim horizontal
                    float horizontalAngle = horizontalInput * rotationSpeed * Time.fixedDeltaTime;
                    //rotate validation
                    float horizontalRotateResult = Mathf.Abs(transform.eulerAngles.y + horizontalAngle);
                    if (
                        //between 0' till -70'
                        (horizontalRotateResult >= (360 - maximumSideRotationAngle) && horizontalRotateResult <= 360) ||
                        //between 0' till 70'
                        (horizontalRotateResult >= 0 && horizontalRotateResult < maximumSideRotationAngle) ||
                        (horizontalRotateResult >= 360) //i dont know bug?
                        )
                    {
                        transform.Rotate(0f, horizontalAngle, 0f);
                    }

                    //user aim vertical, 2f to fasting aim vertical
                    float verticalAngle = verticalInput * rotationSpeed * Time.fixedDeltaTime;
                    verticalAngle = verticalAngle * -1; //inverse
                                                        //rotate validation
                    float verticalRotateResult = (360 - weapon.transform.eulerAngles.x) - verticalAngle;
                    if (verticalRotateResult >= maximumBottomRotationAngle && verticalRotateResult <= maximumUpRotationAngle || verticalRotateResult > 360 /* bug? */)
                    {
                        //rotate
                        weapon.transform.Rotate(verticalAngle, 0f, 0f);
                    }
                }
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                isSwiping = false;
                initialTouch = new Touch();
            }
        }
    }

    bool IsOnAvailableMoveScreen(Vector3 touchPosition)
    {
        if(
            touchPosition.x >= minLeftTouchPosition && 
            touchPosition.x <= maxRightTouchPosition &&
            touchPosition.y >= minBottomTouchPosition &&
            touchPosition.y <= maxTopTouchPosition
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DebugText(string msg)
    {
        try
        {
            GameObject.Find("Debug Text").GetComponent<UnityEngine.UI.Text>().text = msg;
        }
        catch (Exception ex)
        {

        }
    }

    void CalculateAvailableScreenTouch()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        //calculate horizontal,, can all the way
        //float availableWidth = screenWidth * (touchRangePercent / 100);
        //float marginX = (screenWidth - availableWidth) / 2;
        minLeftTouchPosition = 0;
        maxRightTouchPosition = screenWidth;

        //calculate vertical
        float availableHeight = screenHeight * (touchRangePercent / 100);
        float marginY = (screenHeight - availableHeight) / 2;
        minBottomTouchPosition = screenHeight - (marginY + availableHeight);
        maxTopTouchPosition = screenHeight - marginY;
    }

    void PlayMovementSFX()
    {
        try
        {
            if (!movementSFX.isPlaying)
            {
                movementSFX.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("cannot play movement sfx");
        }
    }

    void PlayFireSFX()
    {
        try
        {
            if (!fireSFX.isPlaying)
            {
                fireSFX.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("cannot play fire sfx");
        }
    }

    IEnumerator PlayWeaponFireAnimation()
    {
        if (weaponAnimator != null)
        {
            try
            {
                weaponAnimator.SetBool("isFire", true);
            }
            catch (Exception ex)
            {
                Debug.Log("cannot play animation");
            }
        }

        yield return new WaitForSeconds(1);

        if (weaponAnimator != null)
        {
            try
            {
                weaponAnimator.SetBool("isFire", false);
            }
            catch (Exception ex)
            {
                Debug.Log("cannot play animation");
            }
        }
    }

    IEnumerator PlayFireEffect()
    {
        GameObject smoke = Instantiate(fireEffect, weapon.transform) as GameObject;
        smoke.transform.localPosition = new Vector3(weapon.transform.localPosition.x, weapon.transform.localPosition.y, weapon.transform.localPosition.z + 4f);

        yield return new WaitForSeconds(2);

        Destroy(smoke);
    }
}
