using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;
using UnityEngine.InputSystem;

public class SaveNameType : MonoBehaviour
{
    private InputField inputField;//인풋필드 선언
    public string nameText;
    public bool isFemale;
    public GameObject[] deviceType = new GameObject[2];


    [Header("스크린페이드인자값")]
    public string LoadSceneName;
    public UnityEngine.UI.Image img;
    private bool IsDone;

    private new Renderer renderer = null;
    public float gradientTime = 2;
    private float currentAlpha;
    private float nowFadeAlpha;


    [Tooltip("FadeIn color.")]
    public Color fadeInColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    [Tooltip("FadeOut color.")]
    public Color fadeOutColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    public GameObject[] handPrefabs;
    public TouchScreenKeyboard m_keyboard;

    private PlayerInfo playInfo;
    private TouchScreenKeyboard keyboard;
    private void Awake()
    {
        playInfo = GetComponent<PlayerInfo>();
#if ForAndroid
        Debug.Log("에디터_윈도우");
        deviceType[0].SetActive(true);
        deviceType[1].SetActive(false);
        inputField = FindObjectOfType<InputField>(); //인풋필드 찾기.
        StartCoroutine(imgFade(1, 0, fadeInColor, false));
        return;
#endif
#if ForVR
        deviceType[0].SetActive(false);
        deviceType[1].SetActive(true);
        inputField = FindObjectOfType<InputField>(); //인풋필드 찾기.

#endif
    }
    private void Start()
    {



        //m_keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default, false, false, false);


    }


    public void IsFemale(bool isTrue)
    {
        isFemale = isTrue;
    }

    public void EnterRoom()// 플레이프리팹에 정보 저장후 방입장
    {
        if (playInfo.IsJoindMasterServer)
        {
            //성별 저장
            if (isFemale)
                UserInfo.chType = 1;
            else
                UserInfo.chType = 0;
            //이름저장
            if (nameText.Length > 0)
                UserInfo.userName = nameText;
            else
                UserInfo.userName = "이름없슴2";

            if (deviceType[0].activeSelf)//안드로이드일경우{
            {
                Debug.Log("안드로이드용");
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

    }

    //bool IsInputActive;
    private void Update()
    {

        if (inputField.text.Length > 0)
        {
            nameText = inputField.text;
        }

        if (deviceType[1].activeSelf)//vr일경우만 체크
        {
            if (inputField.isFocused)//키보드 활성화 되었을때 컨트롤러 핸드 하이드
            {
                foreach (var item in handPrefabs)
                {
                    item.SetActive(false);
                }
            }
            else
            {
                foreach (var item in handPrefabs)
                {
                    item.SetActive(true);
                }
            }

        }

    }

    IEnumerator loadSc(string scName) //씬 에이씽크로드
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scName);
        asyncOperation.allowSceneActivation = false;
        PhotonManager photonManager = GetComponent<PhotonManager>();
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



}
