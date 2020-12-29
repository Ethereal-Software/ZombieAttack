using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class SSSMaterial : MonoBehaviour
{

    public Transform directionalLight;
    public bool update;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetLightPosition() {
        var mat = GetComponent<Renderer>().sharedMaterial;
        var r = directionalLight.rotation;
        mat.SetVector("_lightPosition", new Vector4(r.x, r.y, r.z, r.w));
        //mat.SetFloatArray("_lightPosition", new float[] { r.x, r.y, r.z, r.w });
    }
    private void OnValidate()
    {
        SetLightPosition();
    }
    private void OnDrawGizmosSelected()
    {
        update = false;
        SetLightPosition();
    }
}
