using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRandomPosotopm : MonoBehaviour
{
    public Transform[] RandPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = RandPos[Random.Range(0, RandPos.Length - 1)].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
