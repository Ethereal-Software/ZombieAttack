using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceBasic : MonoBehaviour
{
    public GameObject instanceProto;
    public List<Vector3> points;
    public float spacing;
    public bool update = false;
    public bool clear = false;

    void ClearInstances() {
        //while (transform.childCount > 0) {
        foreach (Transform t in transform) {
            DestroyImmediate(t.gameObject);
        }
        foreach (Transform t in transform)
        {
            DestroyImmediate(t.gameObject);
        }
        //}
    }

    void Populate() {
        for (int p = 1; p < points.Count; p++) {
            var from = points[p - 1];
            var to = points[p];
            var delta = to - from;
            float count = delta.magnitude / spacing;//auto overflow
            var spacingDelta = delta.normalized * spacing;
            var curPoint = from;
            for (int i = 0; i < count; i++) {
                var inst = Instantiate(instanceProto, transform);

                

                inst.transform.localPosition = curPoint;
                inst.transform.localRotation = Quaternion.LookRotation(delta, Vector3.up);

                curPoint += spacingDelta;
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        if (update) {
            update = false;
            ClearInstances();
            Populate();
        }
        if (clear) {
            clear = false;
            ClearInstances();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
