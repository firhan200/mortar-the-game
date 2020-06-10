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
    public float rotationSpeed = 50f;

    [SerializeField]
    public GameObject ammoPrefab;

    //effect prefabs
    [SerializeField]
    public GameObject fireEffect;

    [SerializeField]
    public bool previewMode = false;

    //dependency game object
    //joystick
    private GameObject joyStick;
    private Joystick joyStickInput;
    private JoystickController joystickController;

    //fire button controller
    private PowerController powerController;

    //weapon children
    private GameObject weapon;

    //audio source
    AudioSource fireSFX;
    AudioSource movementSFX;

    private Animator weaponAnimator;

    //particle effect
    GameObject sparks;

    private void Start()
    {
        /* initialize */
        if (!previewMode)
        {
            InitJoystick();

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

    void InitJoystick()
    {
        joyStick = GameObject.Find("Fixed Joystick");
        joyStickInput = joyStick.GetComponent<Joystick>();
        joystickController = joyStick.GetComponent<JoystickController>();
    }

    private void FixedUpdate()
    {
        if (!previewMode)
        {
            Fire();
            Aim();
        }
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

    void Aim()
    {
        //check if joystick is pressed
        bool isJoystickPressed = joystickController.isPressed;

        //user aim horizontal
        if (joyStickInput.Horizontal != 0 && isJoystickPressed)
        {
            //play audio sfx
            PlayMovementSFX();

            //user aim horizontal
            float angle = joyStickInput.Horizontal * rotationSpeed * Time.fixedDeltaTime;

            //rotate validation
            float rotateResult = Mathf.Abs(transform.eulerAngles.y + angle);
            if (
                //between 0' till -70'
                (rotateResult >= (360 - maximumSideRotationAngle) && rotateResult <= 360) ||
                //between 0' till 70'
                (rotateResult >= 0 && rotateResult < maximumSideRotationAngle) ||
                (rotateResult >= 360) //i dont know bug?
                )
            {
                transform.Rotate(0f, angle, 0f);
            }
        }

        //user aim vertical
        if (joyStickInput.Vertical != 0 && isJoystickPressed)
        {
            //play audio sfx
            PlayMovementSFX();

            float angle = joyStickInput.Vertical * rotationSpeed * Time.fixedDeltaTime;
            angle = angle * -1; //inverse rotate

            //rotate validation
            float rotateResult = (360 - weapon.transform.eulerAngles.x) - angle;
            if (rotateResult >= maximumBottomRotationAngle && rotateResult <= maximumUpRotationAngle || rotateResult > 360 /* bug? */)
            {
                //rotate
                weapon.transform.Rotate(angle, 0f, 0f);
            }
        }
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
