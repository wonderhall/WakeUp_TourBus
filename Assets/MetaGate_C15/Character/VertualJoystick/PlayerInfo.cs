//using System.Collections;
//using System.Collections.Generic;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using System;
//using UnityEngine.PlayerLoop;
//using Unity.XR.CoreUtils.GUI;
//using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;

public enum deviceType//포톤수정
{
    /*public string */
    vr, android, ios
}

public class PlayerInfo : MonoBehaviour
{
    //캐릭터 정보

    //public string roomName;
    //public new string name = "user00";
    //public int chType;
    public bool IsWalk;
    public int emoEvent;
    public int addEvent;


    public Transform cam;
    public GameObject thePlayer;
    [SerializeField]
    public deviceType _dvType;


    //network
    public GameObject userPrefab;
    public bool UseNetWork;
    //[SerializeField]
    //private bool MoveUpdate = false;

    [Header("vr모드 전용")]//vr모드 전용
    public showVrUI _showVrUI;

    public Sprite[] EmoSprites;

    [Header("안드로이드 전용")]//안드로이드 전용
    public GameObject emoUI;
    public Image ChImage;
    public Sprite[] ChImages;
    public GameObject emoTextBox;
    public bool[] RoomUnLock= new bool[6];
    public GaitLocomotion2 WalkSimulation;
    //[Header("이벤트시스템 0은 vr 1은 안드로이드")]
    //public GameObject[] eventSystems = new GameObject[2];

    // Start is called before the first frame update
    //디버깅
    private GUIStyle guiStyle = new GUIStyle();
    public bool isGuiDebug;
    //public Text debugText;
    public bool IsRotate = false;

    public int ccu = 0;
    public int ccuRoom = 0;
    private bool changingEmoticon = false;

    public bool IsJoindMasterServer = false;

    private void Awake()
    {
        emoTextBox.SetActive(false);

        //name = UserInfo.userName;

        //chType = UserInfo.chType;



#if ForAndroid
        _dvType = deviceType.android;
        if (!UseNetWork) emoUI.transform.parent.gameObject.SetActive(false);// 네트워크가 필요없을때 이모티콘 하이드

        //유저싱글톤에  저장된 캐릭터타입
        if (UserInfo.chType == 0) { ChImage.sprite = ChImages[0]; } //0은남자1은여자 
        else { ChImage.sprite = ChImages[1]; }//여자이면 ui 얼굴 여자로 교체
        savedSprite = ChImage.sprite;
        //if (GameObject.FindAnyObjectByType<EventSystem>())//이벤트 시스템 중복 방지
        //{
        //    Destroy(GameObject.FindAnyObjectByType<EventSystem>().gameObject);
        //}
        //Instantiate(eventSystems[1]);
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(true);
        GameObject.Find("Player").transform.tag = "Player";
#endif
#if ForVR
        _dvType = deviceType.vr;
        //if (GameObject.FindAnyObjectByType<EventSystem>())//이벤트 시스템 중복 방지
        //{
        //    Destroy(GameObject.FindAnyObjectByType<EventSystem>().gameObject);
        //}
        //Instantiate(eventSystems[0]);
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(false);

#endif

    }

    private void OnEnable()
    {

        deviceCheck();
        thePlayer = new GameObject("thePlayer");
        cam = Camera.main.transform;
        if (_dvType == deviceType.vr && UserInfo.UseWalkSimulation == true) WalkSimulation.enabled=true;
        else WalkSimulation.enabled = false;
    }
    void Start()
    {


    }

    // Update is called once per frame

    float deltaTime = 0.0f;
    float EmoTime = 0.0f;
    private Sprite savedSprite;

    void Update()
    {
        if (isGuiDebug) deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        ccu = PhotonNetwork.CountOfPlayers; //현재 접속자수
        ccuRoom = PhotonNetwork.PlayerList.Length;
        if (UseNetWork) //가상의 플레이어 더미의 이동회전값 주기
        {
            float yRotation = cam.eulerAngles.y;
            Quaternion rot = cam.rotation * Quaternion.Euler(0, 1, 0);
            Vector3 newPos = new Vector3(cam.position.x, cam.parent.parent.position.y, cam.position.z);
            if (thePlayer != null)
            {
            thePlayer.transform.position = newPos;
            thePlayer.transform.eulerAngles = new Vector3(0, yRotation, 0);
            }
        }


        if (emoEvent != 0 && UseNetWork)
        {
            EmoTime += Time.deltaTime;
            //Debug.Log("여기");
            if (_dvType == deviceType.android)//안드로이드일때 이모티콘 변경
            {
                if (emoEvent > 8)//이모티콘이 텍스트인지 이미지인지 체크
                {
                    Debug.Log("이모티콘 2" + emoEvent + " 으로 변경");
                    emoTextBox.SetActive(true);
                    emoTextBox.transform.GetChild(0).gameObject.SetActive(true);
                    ChImage.sprite = savedSprite;
                    emoTextBox.transform.GetChild(0).GetComponent<Image>().sprite = EmoSprites[emoEvent - 1];
                    if (EmoTime > 3)
                    {
                        emoEvent = 0;
                        EmoTime = 0;
                        emoTextBox.SetActive(false);
                        emoTextBox.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("이모티콘 3" + emoEvent + " 으로 변경");
                    ChImage.sprite = EmoSprites[emoEvent - 1];
                    if (EmoTime > 3)
                    {
                        Debug.Log("이모타임 3초 지남");
                        emoEvent = 0;
                        EmoTime = 0;
                        ChImage.sprite = savedSprite;
                    }
                }
            }

            else//vr일때 이모티콘 변경
            {
                if (emoEvent < 9)
                {
                    EmoTime += Time.deltaTime;
                    _showVrUI.ChImage.sprite = EmoSprites[emoEvent - 1];
                    if (EmoTime > 3)
                    {
                        emoEvent = 0;
                        EmoTime = 0;
                        _showVrUI.ChImage.sprite = _showVrUI.savedSprite;
                    }
                }
                else//추가텍스트
                {
                    EmoTime += Time.deltaTime;
                    _showVrUI.emoText.gameObject.SetActive(true);
                    _showVrUI.emoText.sprite = EmoSprites[emoEvent - 1];
                    if (EmoTime > 3)
                    {
                        emoEvent = 0;
                        EmoTime = 0;
                        _showVrUI.emoText.gameObject.SetActive(false);
                    }
                }
            }
            if (changingEmoticon)//이모티콘 변경중
            {
                Debug.Log("수상한 곳1");
                thePlayer.GetComponent<getUserInfo>().emoIdx = emoEvent;
                changingEmoticon = !changingEmoticon;
            }




        }

    }

    public void changeEmo(int num)
    {
        changingEmoticon = true;
        emoEvent = num;
        EmoTime = 0;
        Debug.Log("이모티콘 " + num + " 으로 변경");
#if ForAndroid
        if (num > 8) { emoTextBox.SetActive(true); emoTextBox.transform.GetChild(0).GetComponent<Image>().sprite = EmoSprites[num - 1]; }
        //ChImage.sprite = EmoSprites[num - 1];
#endif
    }//이모티콘 바꾸기.에디터 버튼에서 변경



    private void deviceCheck()
    {
        switch (_dvType)
        {
            case deviceType.vr:
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    if (i == 0)
                        transform.GetChild(i).gameObject.SetActive(true);
                    else
                        transform.GetChild(i).gameObject.SetActive(false);
                }
                break;
            case deviceType.android://vr
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    if (i == 1)
                        transform.GetChild(i).gameObject.SetActive(true);
                    else
                        transform.GetChild(i).gameObject.SetActive(false);
                }
                break;
            case deviceType.ios://vr
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    if (i == 2)
                        transform.GetChild(i).gameObject.SetActive(true);
                    else
                        transform.GetChild(i).gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }//타입선택 스위치문

    public void EndGame()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }//게임종료



    //디버깅//
    #region 디버깅유아이
    void OnGUI()
    {
        guiStyle.fontSize = 8;
        guiStyle.normal.textColor = Color.white;


        if (isGuiDebug)
        {
            GUI.Label(new Rect(20, 30, 200, 10), "현재해상도 : " + Screen.currentResolution, guiStyle);

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(new Rect(20, 40, 200, 10), text, guiStyle);
            GUI.Label(new Rect(20, 50, 200, 10), "현재 접속자수 : " + ccu, guiStyle);
            GUI.Label(new Rect(20, 60, 200, 10), "현재 룸 접속자수 : " + ccuRoom, guiStyle);
            GUI.Label(new Rect(20, 70, 200, 10), "접속 이름 : " + UserInfo.userName, guiStyle);
            GUI.Label(new Rect(20, 80, 200, 10), "ch type : " + UserInfo.chType, guiStyle);
            GUI.Label(new Rect(20, 90, 200, 10), "포톤네트워크 OfflineMode 는  : " + PhotonNetwork.OfflineMode, guiStyle);
            GUI.Label(new Rect(20, 100, 200, 10), "포톤네트워크 IsConnected 는  : " + PhotonNetwork.IsConnected, guiStyle);
        }
    }
    #endregion
}
