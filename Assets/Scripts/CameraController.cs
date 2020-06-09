using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float followSpeed = 10;
    public float followRange = 10;
    PowerController powerController;
    Vector3 cameraOffset;
    CanonController canonController;

    // Start is called before the first frame update
    void Start()
    {
        powerController = GameObject.Find("Fire Button").GetComponent<PowerController>();
        canonController = GameObject.Find("Canon Base").GetComponent<CanonController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowBall();
    }

    void FollowBall()
    {
        if(canonController.currentBall != null)
        {
            //check if ball is shot
            if (powerController.isShot)
            {
                Debug.Log("Shot");

                //calculate camera offset
                cameraOffset = canonController.currentBall.transform.position - transform.position;
            }

            bool isBallReleased = powerController.isBallReleased;
            if (isBallReleased)
            {
                //get ball position
                transform.position = Vector3.Lerp(transform.position, canonController.currentBall.transform.position - new Vector3(0f,-2f,followRange), Time.fixedDeltaTime * followSpeed);
            }
        }
        
    }

    public void ResetCamera()
    {
        transform.position = new Vector3(0f, 7.64f, -7.27f);
        transform.rotation = Quaternion.Euler(20f,0f,0f);
    }
}
