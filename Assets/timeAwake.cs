using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.PlayerLoop.PreLateUpdate;

public class timeAwake : MonoBehaviour
{
    public int startTime=0;
    public int endTime=1000;
    public GameObject before;
    public GameObject active;
    private bool isActive = true;
    private float currentTime;
    private int nextUpdate;

    // Start is called before the first frame update
    void Start()
    {
        before.gameObject.SetActive(isActive);
        active.gameObject.SetActive(!isActive);
    }

    // Update is called once per frame
    void Update()
    {


        //if (Time.time > startTime) 
        //{
        //    active.gameObject.SetActive(isActive);
        //    before.gameObject.SetActive(!isActive);
        //}
        //if (Time.time > endTime)
        //{
        //    before.gameObject.SetActive(isActive);
        //    active.gameObject.SetActive(!isActive);
        //}
    }
    private void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        // If the next update is reached
        if (currentTime >= startTime)
        {
            active.gameObject.SetActive(isActive);
            before.gameObject.SetActive(!isActive);
        }
        if (currentTime >= startTime)
        {
            active.gameObject.SetActive(isActive);
            before.gameObject.SetActive(!isActive);
        }
    }
}
