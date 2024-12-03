using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChageFromSelectObj : MonoBehaviour
{
    [Header("스크린페이드인자값")]
    public string LoadSceneName;
    public UnityEngine.UI.Image img;
    private bool IsDone;

    private new Renderer renderer = null;
    public float gradientTime = 2;
    private float currentAlpha;
    private float nowFadeAlpha;

    //[Tooltip("FadeIn color.")]
    //public Color fadeInColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    [Tooltip("FadeOut color.")]
    public Color fadeOutColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    public GameObject[] deviceType = new GameObject[2];

    private void Awake()
    {
#if ForAndroid
        Debug.Log("에디터_윈도우");
        deviceType[0].SetActive(true);
        deviceType[1].SetActive(false);
        return;
#endif
#if ForVR
        deviceType[0].SetActive(false);
        deviceType[1].SetActive(true);

#endif
    }
    public void ScChange()
    {
        if (deviceType[0].activeSelf)//안드로이드일경우{
        {
            Debug.Log("안드로이드용");
            img = GameObject.Find("screenColor").GetComponent<UnityEngine.UI.Image>();
            StartCoroutine(imgFade(0, 1, fadeOutColor, true));
            StartCoroutine(loadSc(LoadSceneName));
        }
        else//vr일경우
        {
            Debug.Log("VR용");
            StartCoroutine(ScreenFade(0, 1, fadeOutColor));
            StartCoroutine(loadSc(LoadSceneName));
        }
    }

    #region 스크린페이드
    public IEnumerator imgFade(float start, float end, Color FadeColor, bool isEndEvent)
    {

        img.enabled = true;
        float nowTime = 0.0f;
        while (nowTime < gradientTime)
        {
            nowTime += Time.deltaTime;
            nowFadeAlpha = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / gradientTime));
            SetImgAlpha(FadeColor);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        if (isEndEvent) IsDone = true;

    }

    public IEnumerator ScreenFade(float start, float end, Color FadeColor)
    {
        Camera cam = Camera.main;
        renderer = cam.GetComponent<Renderer>();
        renderer.enabled = true;
        float nowTime = 0.0f;
        while (nowTime < gradientTime)
        {
            //Debug.Log(nowFadeAlpha);
            nowTime += Time.deltaTime;
            nowFadeAlpha = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / gradientTime));
            SetAlpha(FadeColor);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        IsDone = true;
    }
    private void SetImgAlpha(Color fadeColor)
    {
        Color color = fadeColor;
        color.a = Mathf.Max(currentAlpha, nowFadeAlpha);
        img.color = color;
    }

    private void SetAlpha(Color fadeColor)
    {
        Color color = fadeColor;
        color.a = Mathf.Max(currentAlpha, nowFadeAlpha);
        renderer.material.color = color;
    }
    #endregion

    IEnumerator loadSc(string scName) //씬 에이씽크로드
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scName);
        asyncOperation.allowSceneActivation = false;
        //PhotonManager photonManager = GetComponent<PhotonManager>();
        PhotonManager photonManager = GameObject.FindObjectOfType<PhotonManager>();

        StartCoroutine(photonManager.Disconnect());
        while (!asyncOperation.isDone)
        {
            //if (Input.GetKeyDown(KeyCode.L))
            if (IsDone)
                asyncOperation.allowSceneActivation = true;
            yield return null;
        }

        SceneManager.LoadSceneAsync(scName, LoadSceneMode.Single);
    }

}
