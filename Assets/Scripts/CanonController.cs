using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanonController : MonoBehaviour
{
    //duplicate for canon ball
    public GameObject canonBallPrefab;

    //canon game object
    public GameObject canonWeapon;

    //animation
    public Animator canonAnimator;

    //audio source
    AudioSource[] audioSources;
    AudioSource explosionEffect;
    AudioSource movementEffect;

    //effect prefabs
    public GameObject smokeEffect;
    public GameObject sparksEffect;

    GameObject sparks;

    // for up and down rotation
    public Transform canonJoint;

    public GameObject currentBall;

    public float firePower = 100;
    public float rotationSpeed = 100;
    public float maximumYAngle = 70;
    public float maximumXTopAngle = 70;
    public float maximumXBottomAngle = 0;
    public GameObject joyStick;
    public PowerController powerController;

    Joystick joyStickControl;

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
        explosionEffect = audioSources[0];
        movementEffect = audioSources[1];
        joyStickControl = joyStick.GetComponent<Joystick>();
        powerController = GameObject.Find("Fire Button").GetComponent<PowerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Aim();
        Fire();
        HoldFire();

        //Debug Cheat
        DebugCheat();
    }

    void DebugCheat()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    void Fire()
    {
        //check if press released
        if (powerController.isShot)
        {
            //add smoke effect
            StartCoroutine(DestroySmokeEffect());

            //start animation
            StartCoroutine(PlayCanonFireAnimation());

            //play sound
            explosionEffect.Play();

            //duplicate
            currentBall = Instantiate(canonBallPrefab, new Vector3(0f, transform.localPosition.y + canonJoint.transform.localPosition.y, 0f), Quaternion.Euler(
                    canonJoint.eulerAngles.x,
                    transform.eulerAngles.y,
                    0f
                )) as GameObject;

            //GameObject canonBallDupe = Instantiate(canonBall) as GameObject;
            Rigidbody canonBallRB = currentBall.GetComponent<Rigidbody>();

            float totalFirePower = powerController.powerSlider.value * firePower;

            Debug.Log(currentBall.transform.localEulerAngles.x);
            Debug.Log(canonJoint.eulerAngles.x+" or "+ canonJoint.localEulerAngles.x);

            //unfreeze ball
            canonBallRB.constraints = RigidbodyConstraints.None;

            //add force to ball
            canonBallRB.AddForce(currentBall.transform.forward * totalFirePower, ForceMode.Impulse);

            //to prevent add force frequently
            powerController.DeactiveShotStatus();

            //start animation
            //canonAnimator.SetBool("isFire", false);
        }
    }

    IEnumerator PlayCanonFireAnimation()
    {
        canonAnimator.SetBool("isFire", true);

        yield return new WaitForSeconds(1);

        canonAnimator.SetBool("isFire", false);
    }

    IEnumerator DestroySmokeEffect()
    {
        GameObject smoke = Instantiate(smokeEffect, canonWeapon.transform) as GameObject;
        smoke.transform.localPosition = new Vector3(canonWeapon.transform.localPosition.x, canonWeapon.transform.localPosition.y, canonWeapon.transform.localPosition.z + 4f);

        yield return new WaitForSeconds(2);

        Destroy(smoke);
    }

    void Aim()
    {
        //validation aim limitation
        AimLimitation();

        //check if joystick is pressed
        bool isJoystickPressed = joyStick.GetComponent<JoystickController>().isPressed;

        if (joyStickControl.Horizontal != 0 && isJoystickPressed)
        {
            //play movement sfx
            if (!movementEffect.isPlaying)
            {
                movementEffect.Play();
            }

            //user aim horizontal
            float angle = joyStickControl.Horizontal * rotationSpeed * Time.fixedDeltaTime;
            
            this.transform.Rotate(0f, angle, 0f);
        }

        if(joyStickControl.Vertical != 0 && isJoystickPressed)
        {
            //play movement sfx
            if (!movementEffect.isPlaying)
            {
                movementEffect.Play();
            }

            //user aim vertical
            float angle = joyStickControl.Vertical * rotationSpeed * Time.fixedDeltaTime;

            //invert
            canonJoint.Rotate(angle * -1, 0f, 0f);
        }
    }

    public void HoldFire()
    {
        if (powerController.isHold)
        {
            if(sparks == null)
            {
                sparks = Instantiate(sparksEffect, canonWeapon.transform) as GameObject;
                sparks.transform.localPosition = new Vector3(canonWeapon.transform.localPosition.x, canonWeapon.transform.localPosition.y + 1.5f, canonWeapon.transform.localPosition.z - 1);
            }
        }
        else
        {
            Destroy(sparks);
        }
    }

    public void ResetCanon()
    {
        //reset canon position
        transform.rotation = Quaternion.Euler(0f,0f,0f);
        canonJoint.transform.rotation = Quaternion.Euler(-30f,0f,0f);
    }

    void AimLimitation()
    {
        //limit horizontal
        //check left or right
        if (this.transform.rotation.y > 0)
        {
            //right
            if (this.transform.eulerAngles.y > maximumYAngle)
            {
                this.transform.rotation = Quaternion.Euler(0f, maximumYAngle, 0f);
            }
        }
        else if(this.transform.rotation.y < 0)
        {
            //left
            if ((360 - this.transform.eulerAngles.y) > maximumYAngle)
            {
                this.transform.rotation = Quaternion.Euler(0f, -1 * maximumYAngle, 0f);
            }
        }

        //limit vertical
        if (canonJoint.rotation.x > 0)
        {
            //down
            if (canonJoint.eulerAngles.x < maximumXBottomAngle)
            {
                //canonJoint.rotation = Quaternion.Euler(maximumXBottomAngle, 0f, 0f);
            }
        }
        else if(canonJoint.rotation.x < 0)
        {
            //up
            if ((360 - canonJoint.eulerAngles.x) > maximumXTopAngle)
            {
                //canonJoint.rotation = Quaternion.Euler(maximumXTopAngle * -1, 0f, 0f);
            }
        }
    }
}
