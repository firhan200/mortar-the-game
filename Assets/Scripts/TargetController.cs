using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    //inputs
    [SerializeField]
    GameObject desctrucableVersion;

    [SerializeField]
    int targetPoint = 1; //default point if hit

    [SerializeField]
    float rotationAnimationSpeed = 3f; //target rotaion speed

    private void FixedUpdate()
    {
        //rotate target
        transform.Rotate(0f, rotationAnimationSpeed, 0f);
    }

    public void Destroy()
    {
        //change with destrucable one
        Instantiate(desctrucableVersion, transform.position, transform.rotation);

        //destroy this real object
        Destroy(gameObject);
    }

    public int GetTargetPoint()
    {
        return targetPoint;
    }
}
