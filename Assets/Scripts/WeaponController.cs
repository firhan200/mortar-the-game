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

    //dependency game object
    //joystick
    private GameObject joyStick;
    private Joystick joyStickInput;
    private JoystickController joystickController;

    //fire button controller
    private PowerController powerController;

    //weapon children
    private GameObject weapon;

    private void Start()
    {
        /* initialize */

        InitJoystick();

        GetPowerButton();

        GetWeaponObject();
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
        Fire();
        Aim();
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

            //Get Rigid Body
            Rigidbody canonBallRB = ammo.GetComponent<Rigidbody>();

            //calculate fire power
            float totalFirePower = powerController.powerSlider.value * firePower;

            //unfreeze ball
            canonBallRB.constraints = RigidbodyConstraints.None;

            //add force to ball
            canonBallRB.AddForce(ammo.transform.forward * totalFirePower, ForceMode.Impulse);

            //set camera to follow ammo


            //to prevent multiple shot
            isFire = false;
            powerController.isShot = false;
        }
    }

    void Aim()
    {
        //check if joystick is pressed
        bool isJoystickPressed = joystickController.isPressed;

        Debug.Log(joyStickInput.Vertical + ", " + isJoystickPressed);

        //user aim horizontal
        if (joyStickInput.Horizontal != 0 && isJoystickPressed)
        {
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
            float angle = joyStickInput.Vertical * rotationSpeed * Time.fixedDeltaTime;
            angle = angle * -1; //inverse rotate

            //rotate validation
            float rotateResult = (360 - weapon.transform.eulerAngles.x) - angle;
            Debug.Log(rotateResult);
            if (rotateResult >= maximumBottomRotationAngle && rotateResult <= maximumUpRotationAngle || rotateResult > 360 /* bug? */)
            {
                //rotate
                weapon.transform.Rotate(angle, 0f, 0f);
            }
        }
    }
}
