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
    private InputField inputField;//��ǲ�ʵ� ����
    public string nameText;
    public bool isFemale;
    public GameObject[] deviceType = new GameObject[2];


    [Header("��ũ�����̵����ڰ�")]
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
        Debug.Log("������_������");
        deviceType[0].SetActive(true);
        deviceType[1].SetActive(false);
        inputField = FindObjectOfType<InputField>(); //��ǲ�ʵ� ã��.
        StartCoroutine(imgFade(1, 0, fadeInColor, false));
        return;
#endif
#if ForVR
        deviceType[0].SetActive(false);
        deviceType[1].SetActive(true);
        inputField = FindObjectOfType<InputField>(); //��ǲ�ʵ� ã��.

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

    public void EnterRoom()// �÷��������տ� ���� ������ ������
    {
        if (playInfo.IsJoindMasterServer)
        {
            //���� ����
            if (isFemale)
                UserInfo.chType = 1;
            else
                UserInfo.chType = 0;
            //�̸�����
            if (nameText.Length > 0)
                UserInfo.userName = nameText;
            else
                UserInfo.userName = "�̸�����2";

            if (deviceType[0].activeSelf)//�ȵ���̵��ϰ��{
            {
                Debug.Log("�ȵ���̵��");
                StartCoroutine(imgFade(0, 1, fadeOutColor, true));
                StartCoroutine(loadSc(LoadSceneName));
            }
            else//vr�ϰ��
            {
                Debug.Log("VR��");
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

        if (deviceType[1].activeSelf)//vr�ϰ�츸 üũ
        {
            if (inputField.isFocused)//Ű���� Ȱ��ȭ �Ǿ����� ��Ʈ�ѷ� �ڵ� ���̵�
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

    IEnumerator loadSc(string scName) //�� ���̾�ũ�ε�
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


    #region ��ũ�����̵�
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
