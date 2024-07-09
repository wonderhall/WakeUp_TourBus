using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangSkyBox : MonoBehaviour
{
    public Transform sunMesh;
    public SkyboxBlender sb;

    //public Material skybox;
    public float gradientTime = 2;
    public float SunFallDownTime = 2;
    [Range(0, 1)]
    public float changeV = 0;

    private float nowFadeAlpha;
    private float Sun_posY;
    private float Sun_rotX;
    private Quaternion lightRot;
    public Camera cam;
    private Vector3 scaleChange;
    public float scaleChangeValue = 0.01f;
    private void OnEnable()
    {
        cam = Camera.main;
        StartCoroutine(ScreenFade(0, 1));
        StartCoroutine(FallDown());

    }


    // Update is called once per frame

    public IEnumerator ScreenFade(float start, float end)
    {
        Debug.Log("gotkeys");
        float nowTime = 0.0f;
        while (nowTime < gradientTime)
        {
            nowTime += Time.deltaTime;
            //nowFadeAlpha = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / gradientTime));
            //SetAlpha();
            sb.blend = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / gradientTime));
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator FallDown()
    {
        sunMesh.gameObject.SetActive(true);
        Debug.Log("gotkeys");
        float nowTime = 0.0f;
        while (nowTime < SunFallDownTime)
        {
            nowTime += Time.deltaTime;
            Sun_posY = Mathf.Lerp(sunMesh.position.y, -6f, Mathf.Clamp01(nowTime / SunFallDownTime));
            sunMesh.position = new Vector3(sunMesh.position.x, Sun_posY, sunMesh.position.z);
            scaleChange = new Vector3(scaleChangeValue, scaleChangeValue, scaleChangeValue);
            sunMesh.localScale += scaleChange;
            sunMesh.transform.LookAt(cam.transform);
            Debug.Log(Sun_posY);
            if (Sun_posY < -5.3f)
                break;
            yield return new WaitForEndOfFrame();
        }
    }

}
