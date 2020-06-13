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

    [SerializeField]
    private GameObject flyingEffect;

    //local
    Rigidbody ammoRigidBody;
    AudioSource explosionSoundEffect;
    GameController gameController;
    GameObject currentExplosion;
    bool isLose = false;
    bool isHitTarget = false;
    private GameObject currentFlyingEffect;
    bool isHitSomething = false;

    // Start is called before the first frame update
    void Start()
    {
        InitController();
        ammoRigidBody = GetComponent<Rigidbody>();
        explosionSoundEffect = GetComponent<AudioSource>();

        //init flying effect
        InitFlyingEffect();
    }

    void InitFlyingEffect()
    {
        if(flyingEffect != null)
        {
            currentFlyingEffect = Instantiate(flyingEffect, transform) as GameObject;
        }
    }

    void InitController()
    {
        gameController = GameObject.Find("World").GetComponent<GameController>();
    }

    private void FixedUpdate()
    {
        IsLose();

        //for ammo to be able to face force
        float rotateX = ammoRigidBody.velocity.normalized.y > 0 ? ammoRigidBody.velocity.normalized.y : ammoRigidBody.velocity.normalized.y * -1;
        transform.Rotate(rotateX * 1.5f, 0f,0f);
    }

    /* on ammo hit something */
    private void OnCollisionEnter(Collision collision)
    {
        //ammo has not hit something yet
        if (!isHitSomething)
        {
            isHitSomething = true;

            //start hit action
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        //make ammo explosion
        Explode();

        CheckHitTarget();

        //destroy flying effect
        if(currentFlyingEffect != null)
        {
            Destroy(currentFlyingEffect);
        }

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
            //Debug.Log(ex.Message);
        }

        //stop ball
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        //hide ball
        GetComponent<Renderer>().enabled = false;
    }

    void CheckHitTarget()
    {
        int targetPoint = 0;

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

                    gameController.HitAnimation();

                    //target controll
                    TargetController targetController = nearByGameObject.GetComponent<TargetController>();

                    //get target point
                    targetPoint = targetController.GetTargetPoint();

                    if(targetPoint > 1)
                    {
                        //special
                        gameController.IncreaseAmmo(1);
                    }

                    //destroy this box
                    targetController.Destroy();
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

        if (isHitTarget)
        {
            //increment user point
            gameController.IncrementPoint(targetPoint);
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
