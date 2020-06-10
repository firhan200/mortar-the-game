using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestrucableScript : MonoBehaviour
{
    public GameObject desctrucableVersion;

    public void Destroy()
    {
        //change with destrucable one
        Instantiate(desctrucableVersion, transform.position, transform.rotation);

        //destroy this real object
        Destroy(gameObject);
    }
}
