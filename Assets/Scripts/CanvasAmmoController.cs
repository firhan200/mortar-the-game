using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAmmoController : MonoBehaviour
{
    //addmo prefabs
    public GameObject ammoImage;

    public void DrawAmmoImages(int totalAmmo)
    {
        //reset all
        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int ammo = 1; ammo <= totalAmmo; ammo++)
        {
            //clone
            var ammoImageObj = Instantiate(ammoImage, transform) as GameObject;
            var ammoRect = ammoImageObj.GetComponent<RectTransform>();

            //translate
            ammoRect.anchoredPosition = new Vector3((25 * (ammo - 1) * -1) - ((ammoRect.rect.width/2) * ammo),0f,0f);
        }
    }
}
