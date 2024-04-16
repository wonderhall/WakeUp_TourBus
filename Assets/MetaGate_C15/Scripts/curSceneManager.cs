using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;

public class curSceneManager : MonoBehaviour
{
    #region backup
    //public GameObject sf;
    //public Renderer renderer = new Renderer();
    //public float gradientTime;
    //public bool isFade;
    //private Material gradientMaterial = null;
    //private float currentAlpha;
    //private float nowFadeAlpha;
    //[Tooltip("Basic color.")]
    //public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    //public IEnumerator ScreenFade(float start, float end)
    //{
    //    Debug.Log("gotkeys");

    //    float nowTime = 0.0f;
    //    while (nowTime < gradientTime)
    //    {
    //        Debug.Log(nowFadeAlpha);
    //        nowTime += Time.deltaTime;
    //        nowFadeAlpha = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / gradientTime));
    //        SetAlpha();
    //        yield return new WaitForEndOfFrame();
    //    }
    //}

    //private void SetAlpha()
    //{
    //    Color color = fadeColor;
    //    color.a = Mathf.Max(currentAlpha, nowFadeAlpha);
    //    renderer.material.color = color;

    //}
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        renderer = sf.GetComponent<Renderer>();
    //        renderer.enabled = true;
    //        StartCoroutine(ScreenFade(0, 1));
    //        fadeColor = renderer.material.color;
    //    }

    //} 
    #endregion

    public List<EventInfo> list;
    public float speed = 1;
    public int startSecond = 0;
    [Header("종료설정")]
    public int endSc_sec;
    public string SceneName;
    public float EndGradientTime;
    private bool IsDone;
    private new Renderer renderer = null;
    private bool isDoing;


    // Next update in second
    private int nextUpdate = 1;
    private float nowFadeAlpha;
    private Color fadeColor = Color.black;
    private float currentAlpha;

    private float currentTime = 0.0f;

    private bool updatingColor;

    private void Awake()
    {
        currentTime = 0.0f;
        foreach (var item in list)
        {
            item.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        currentTime += Time.deltaTime * speed;
        // If the next update is reached
        if (currentTime >= nextUpdate)
        {
            UpdateEverySecond(nextUpdate);
            //Debug.Log(Time.time + ">=" + nextUpdate);
            // Change the next update (current second+1)
            nextUpdate = Mathf.FloorToInt(currentTime) + 1;
            // Call your fonction
        }
        
    }
    // Update is called once per second
    void UpdateEverySecond(int t)
    {
        t += startSecond;


        if (t >= endSc_sec && !isDoing && !IsDone)
        {
            Debug.Log("끝");
            StartCoroutine(loadSc(SceneName));
            StartCoroutine(ScreenFade(0, 1));
        }
        foreach (var item in list)
        {
            Debug.Log("do it . time is " + (t - 1));
            Debug.Log(item.name);
            if (!item.isDone)
            {
                if (t > item.startSecond && !item.isStart)
                {
                    item.gameObject.SetActive(true);
                    if (item.isFadeIn)
                    {

                        if (!updatingColor) StartCoroutine(ScreenFade(0, 1, item));
                        else continue;
                    }
                    else item.isStart = true;

                }
                else if (item.isStart)
                {
                    if (t > item.EndSecond && !item.isFadeOut)
                    {
                        item.gameObject.SetActive(false);
                        item.isDone = true;
                    }
                    else if (t > item.EndSecond && item.isFadeOut)
                    {
                        if (!updatingColor) StartCoroutine(ScreenFade(1, 0, item));
                    }

                }
                else continue;

            }
        }
        // ...

    }

    //페이드인아웃
    public IEnumerator ScreenFade(float start, float end, EventInfo item)
    {
        Debug.Log("여기");
        updatingColor = true;
        if (item.isStart && item.isDone) yield break;
        Color col = item.renderer.material.color;
        Color startColor = col;
        Color endColor = col;
        startColor.a = start;
        endColor.a = end;

        float t = 0;
        while (t < item.gradientTime)
        {
            t += Time.deltaTime;
            col = Color.Lerp(startColor, endColor, t / item.gradientTime);
            item.renderer.material.color = col;
            yield return null;
        }
        yield return null;

        if (start == 0) item.isStart = true;
        else
        {
            item.isDone = true;
            item.gameObject.SetActive(false);
        }
        yield return null;
        updatingColor = false;
    }



    //종료 화면 어두워짐
    IEnumerator loadSc(string scName)
    {
        yield return null;
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scName);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            //if (Input.GetKeyDown(KeyCode.L))
            if (IsDone)
                asyncOperation.allowSceneActivation = true;
            yield return null;
        }

        //SceneManager.LoadSceneAsync(scName, LoadSceneMode.Single);
    }

    public IEnumerator ScreenFade(float start, float end)
    {
        isDoing = true;
        Camera cam = Camera.main;
        renderer = cam.GetComponent<Renderer>();
        renderer.enabled = true;
        float nowTime = 0.0f;
        while (nowTime < EndGradientTime)
        {
            Debug.Log(nowFadeAlpha);
            nowTime += Time.deltaTime;
            nowFadeAlpha = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / EndGradientTime));
            SetAlpha();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        IsDone = true;
        isDoing = false;


    }

    private void SetAlpha()
    {
        Color color = fadeColor;
        color.a = Mathf.Max(currentAlpha, nowFadeAlpha);
        renderer.material.color = color;

    }

}
