using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookYtoMainCam : MonoBehaviour
{
    private Camera cam;

    private void Update()
    {
        cam = Camera.main;
        //this.transform.LookAt(cam.transform);
        this.transform.LookAt(cam.transform);
        transform.localRotation *= Quaternion.Euler(0,-180,0);//로컬 X에서 -90°, 로컬 Z축에서 -90°를 추가로 회전하여 포워드 방향을 뒤집는다
        this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 1, 0);
    }
}
