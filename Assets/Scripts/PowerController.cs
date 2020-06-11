using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //slider
    public UnityEngine.UI.Slider powerSlider;

    //last recorded power
    public GameObject powerSliderLast;

    //slider increment
    public float sliderIncrement = 0.5f;

    //sfx
    AudioSource sfx;

    //is button on hold
    public bool isHold = false;
    public bool isShot = false;
    public bool isBallReleased = false;

    private void Awake()
    {
        //hide because user has not shot yet
        powerSliderLast.SetActive(false);
    }

    void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isBallReleased)
        {
            isHold = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isBallReleased)
        {
            isShot = true; //ball is now shooting
            isBallReleased = true; //tell camera to follow ball
            isHold = false;

            //record last power
            if (!powerSliderLast.activeSelf)
            {
                powerSliderLast.SetActive(true);
            }
            powerSliderLast.GetComponent<UnityEngine.UI.Slider>().value = powerSlider.value;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        IncreasePower();
    }

    void IncreasePower()
    {
        if (isHold)
        {
            if(powerSlider.value <= 1)
            {
                //play sfx
                if (!sfx.isPlaying)
                {
                    sfx.Play();
                }

                //increase bar
                powerSlider.value += Time.fixedDeltaTime * sliderIncrement;
            }
        }
    }

    public void ResetPower()
    {
        powerSlider.value = 0;
        isShot = false;
        isBallReleased = false;
    }

    public void DeactiveShotStatus()
    {
        isShot = false;
    }
}
