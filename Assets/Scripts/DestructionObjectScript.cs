using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionObjectScript : MonoBehaviour
{
    public float lifeTimeSeconds = 3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Dissappear());
    }

    //for amount x second dessapear
    IEnumerator Dissappear()
    {
        yield return new WaitForSeconds(lifeTimeSeconds);

        Destroy(gameObject);
    }
}
