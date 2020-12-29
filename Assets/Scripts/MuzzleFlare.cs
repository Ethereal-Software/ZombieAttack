using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlare : MonoBehaviour
{
    public Light muzzleFlashLight;
    public SpriteRenderer muzzleFlare;
    public float alphaMax;

    //public float muzzleRangeMax = 10f;
    public float muzzleIntensityMax = 1.5f;
    public float flashDecayTime = 0.5f;


    int flashState = 0;
    public void Flash() {
        flashState = 1;
    }
    
    void Start()
    {
        muzzleFlare.color = new Color(1, 1, 1, 0);
        muzzleFlashLight.intensity = 0;
        flashState = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (muzzleFlashLight != null && flashState != 0)
        {
            if (flashState == 1)
            { //getting brighter

                muzzleFlare.color = new Color(1, 1, 1, alphaMax);
                muzzleFlashLight.intensity = muzzleIntensityMax;
                flashState = 2;
            }
            else if (flashState == 2)
            {  //getting darker
                if (muzzleFlashLight.range > 0)
                { //fade out
                    var colTp = muzzleFlare.color;
                    colTp.a -= Time.deltaTime / flashDecayTime;
                    muzzleFlare.color = colTp;

                    muzzleFlashLight.intensity -= muzzleIntensityMax * Time.deltaTime / flashDecayTime;
                }
                else
                {//off
                    muzzleFlare.color = new Color(1, 1, 1, 0);
                    muzzleFlashLight.intensity = 0;
                    flashState = 0;
                }
            }

        }
    }
}
