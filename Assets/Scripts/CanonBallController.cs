using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanonBallController : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject desctructionBoxPrefab;
    AudioSource explosionSoundEffect;
    PowerController powerController;
    CanonController canonController;
    CameraController cameraController;
    GameController gameController;
    public float waitSeconds = 2f;
    public float radius = 5f;
    public float explosionForce = 5f;
    GameObject explosion;
    bool isLose = false, isHitTarget = false;

    private void Start()
    {
        powerController = GameObject.Find("Fire Button").GetComponent<PowerController>();
        canonController = GameObject.Find("Canon Base").GetComponent<CanonController>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        gameController = GameObject.Find("World").GetComponent<GameController>();
        explosionSoundEffect = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        IsLose();
    }

    void IsLose()
    {
        if(transform.localPosition.y < -10 && !isLose)
        {
            isLose = true;
            StartCoroutine(Hit());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Hit());
    }

    void Explode()
    {
        //show explosion
        explosion = Instantiate(explosionEffect, transform.position, transform.rotation);

        //play sfx
        try
        {
            explosionSoundEffect.Play();
        }catch(Exception ex)
        {

        }

        //stop ball
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        //hide ball
        GetComponent<Renderer>().enabled = false;
    }

    void CheckHitTarget()
    {
        isHitTarget = false;

        //check hit target
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObjectCollider in collidersToDestroy)
        {
            GameObject nearByGameObject = nearbyObjectCollider.gameObject;
            Rigidbody rigidbody = nearbyObjectCollider.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                //check if any box
                if (nearbyObjectCollider.tag == "Target")
                {
                    //set hit to true
                    isHitTarget = true;

                    //destroy this box
                    nearByGameObject.GetComponent<DestrucableScript>().Destroy();
                }
            }
        }

        //add force to nearby collider destroy
        Collider[] collidersToRemove = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObjectCollider in collidersToRemove)
        {
            Rigidbody rigidbody = nearbyObjectCollider.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                    //object has rigid body (destrucion object)
                rigidbody.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }

        if (!isHitTarget)
        {
            //user failed to hit target, decrease ammo
            gameController.DecreaseAmmo();
        }
        else
        {
            Debug.Log("HIT!!!");
            //increment user point/score
            gameController.IncrementPoint();
        }
    }

    IEnumerator Hit()
    {
        //explotion
        Explode();

        CheckHitTarget();

        //wait for amount of time
        yield return new WaitForSeconds(waitSeconds);

        //hide explosion
        Destroy(explosion);

        //reset camera, canon , etc.
        ResetAll();

        //destroy object
        Destroy(gameObject);
    }

    void ResetAll()
    {
        //check if all ammo out
        if(gameController.currentAmmo > 0)
        {
            //reset canon position
            canonController.ResetCanon();

            //reset power
            powerController.ResetPower();

            //reset camera
            cameraController.ResetCamera();

            if (isHitTarget)
            {
                //drop box again
                gameController.DropBox();
            }
        }
        else
        {
            //lose
            //hide game canvas
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

            //show game over
            GameObject.Find("Game Over Canvas").GetComponent<Canvas>().enabled = true;
        }
    }
}
