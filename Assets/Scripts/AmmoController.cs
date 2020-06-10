using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    //input
    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private float explosionRadius = 5f;

    [SerializeField]
    private float explosionCastSeconds = 2f;

    [SerializeField]
    private float explosionPower = 5f;

    //local
    AudioSource explosionSoundEffect;
    GameController gameController;
    GameObject currentExplosion;
    bool isLose = false;
    bool isHitTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        InitController();

        explosionSoundEffect = GetComponent<AudioSource>();
    }

    void InitController()
    {
        gameController = GameObject.Find("World").GetComponent<GameController>();
    }

    private void FixedUpdate()
    {
        IsLose();
    }

    /* on ammo hit something */
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        //make ammo explosion
        Explode();

        CheckHitTarget();

        //wait for amount of time
        yield return new WaitForSeconds(explosionCastSeconds);

        //hide explosion
        Destroy(currentExplosion);

        //decide continue or game over
        gameController.AmmoHitSomething(isHitTarget);

        //destroy object
        Destroy(gameObject);
    }

    void Explode()
    {
        //show explosion
        currentExplosion = Instantiate(explosionEffect, transform.position, transform.rotation);

        //play sfx
        try
        {
            explosionSoundEffect.Play();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        //stop ball
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        //hide ball
        GetComponent<Renderer>().enabled = false;
    }

    void CheckHitTarget()
    {
        //check hit target
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObjectCollider in collidersToDestroy)
        {
            GameObject nearByGameObject = nearbyObjectCollider.gameObject;
            Rigidbody rigidbody = nearbyObjectCollider.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                //check if any box
                if (nearbyObjectCollider.tag == "Target")
                {
                    isHitTarget = true;

                    //destroy this box
                    nearByGameObject.GetComponent<DestrucableScript>().Destroy();
                }
            }
        }

        //add force to nearby collider destroy
        Collider[] collidersToRemove = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObjectCollider in collidersToRemove)
        {
            Rigidbody rigidbody = nearbyObjectCollider.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                //object has rigid body (destrucion object)
                rigidbody.AddExplosionForce(explosionPower, transform.position, explosionRadius);
            }
        }
    }

    void IsLose()
    {
        //check if position below ground
        if (transform.localPosition.y < -10 && !isLose)
        {
            isLose = true;
            StartCoroutine(Hit());
        }
    }
}
