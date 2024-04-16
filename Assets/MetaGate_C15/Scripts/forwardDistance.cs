using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forwardDistance : MonoBehaviour
{
    public GameObject target;
    private Vector3 offset;
    public Vector3 transOffset= new Vector3(1,0.5f,0);
    public float distance;
    public float YLimit;

    // Use this for initialization
    void Start()
    {
        offset = transform.position - target.transform.position;

        distance = offset.magnitude;
    }

    void LateUpdate()
    {
        transform.position = target.transform.position +  target.transform.forward * distance;

       
        if (transform.position.y < YLimit)
        { 
            transform.position = new Vector3(transform.position.x, YLimit, transform.position.z);
        }// 오브젝트가 너무 낮아서 안보이는걸 막는다

    }
}
