using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public float followSpeed = 10;

    [SerializeField]
    public float followRange = 10;

    PowerController powerController;
    Vector3 cameraOffset;
    Vector3 originalCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalCameraPosition = transform.position;
        powerController = GameObject.Find("Fire Button").GetComponent<PowerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowBall();
    }

    void FollowBall()
    {
        if (powerController.isBallReleased)
        {
            try
            {
                //get ammo
                GameObject ammo = GameObject.FindGameObjectWithTag("Ammo");

                //get ball position
                transform.position = Vector3.Lerp(transform.position, ammo.transform.position - new Vector3(0f, -2f, followRange), Time.fixedDeltaTime * followSpeed);
            }catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }

    public void ResetCamera()
    {
        transform.position = originalCameraPosition;
    }
}
