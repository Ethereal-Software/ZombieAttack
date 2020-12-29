using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class LightFlicker : MonoBehaviour
{
    // Start is called before the first frame update
    public Light light;

    public float flickerTimeMin;
    public float flickerTimeMax;
    public float flickerTime;

    public bool flickerFromOff = true;
    
    void Start()
    {
    }

    // Update is called once per frame
    int lightState = 0;//0=default, 1=darkening, 2=brightening

    float nextFlickerTime = 0;
    void Update()
    {

        if (Time.time > nextFlickerTime) {
            lightState = 1;
            if(flickerFromOff)
                lightState = 2;
            nextFlickerTime = Time.time + Random.Range(flickerTimeMin, flickerTimeMax);
        }

        if (lightState == 1) {
            if (light.intensity > 0)
            { ///darken
                light.intensity -= Time.deltaTime / flickerTime;
            }
            else {
                light.intensity = 0;
                lightState = 2;
                if (flickerFromOff)
                    lightState = 0;
            }
        }
        else if(lightState == 2) {
            if (light.intensity < 1)
            { ///darken
                light.intensity += Time.deltaTime / flickerTime;
            }
            else
            {
                light.intensity = 1;
                lightState = 0;
                if (flickerFromOff)
                    lightState = 1;
            }
        }


    }
}
