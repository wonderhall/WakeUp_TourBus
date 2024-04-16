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
        transform.localRotation *= Quaternion.Euler(0,-180,0);//���� X���� -90��, ���� Z�࿡�� -90�Ƹ� �߰��� ȸ���Ͽ� ������ ������ �����´�
        this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 1, 0);
    }
}
