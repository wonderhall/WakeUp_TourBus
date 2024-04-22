using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;
using PN = Photon.Pun.PhotonNetwork;
using Photon.Realtime;

public class DoorOpen : MonoBehaviourPunCallbacks
{
    public string SceneName;
    private bool IsDone;
    public bool IsBreakNet;
    private bool isActive;


    private new Renderer renderer;
    public float gradientTime = 2;
    [Header("temp")]
    private float currentAlpha;
    private float nowFadeAlpha;
    [Tooltip("Basic color.")]
    public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);


    private PhotonView PV;
    private PhotonManager p_manager;
    private PlayerInfo playerInfo;

    private bool isChangingSc = false;//씬 변경중인지 체크
    private void Start()
    {
        playerInfo = GameObject.FindAnyObjectByType<PlayerInfo>();
        p_manager = GameObject.FindAnyObjectByType<PhotonManager>();
    }
    void OnTriggerEnter(Collider other)
    {
#if ForAndroid
        if (other.tag == "Player")//안에 들어온 오브젝트의 태그가 "Player"라면
        {
            Debug.Log(other);
            if (isActive == false)
            {
                isActive = true; //도어를 활성화해서 이 문에 연결된  "OnLeftRoom()"만 실행한다
                if (PN.OfflineMode == false && IsBreakNet == false)//온라인 룸에서 온라인 룸으로 실행
                {
                    Debug.Log("온라인 룸에서 온라인 룸으로 실행");
                    StartCoroutine(ScreenFade(0, 1));
                }

                else //오프라인 룸에서  오프라인 룸으로 실행
                {
                    Debug.Log("오프라인 룸에서  오프라인 룸으로 실행");
                    StartCoroutine(loadSc(SceneName));
                    StartCoroutine(ScreenFade(0, 1));
                }
            }

        }
#endif
#if ForVR
        if (other.tag == "Player" || other.tag == "MainCamera")//안에 들어온 오브젝트의 태그가 "Player"라면
        {
            Debug.Log(other);
            if (isActive == false)
            {
            isActive = true; //도어를 활성화해서 이 문에 연결된  "OnLeftRoom()"만 실행한다
            if (PN.OfflineMode == false && IsBreakNet == false)//온라인 룸에서 온라인 룸으로 실행
            {
                Debug.Log("온라인 룸에서 온라인 룸으로 실행");
                StartCoroutine(ScreenFade(0, 1));
                //PV = other.GetComponent<PhotonView>();
                //if (PV.IsMine) StartCoroutine(ScreenFade(0, 1));

            }

            else //오프라인 룸에서  오프라인 룸으로 실행
            {
                Debug.Log("오프라인 룸에서  오프라인 룸으로 실행");
                StartCoroutine(loadSc(SceneName));
                StartCoroutine(ScreenFade(0, 1));
            }
            }
        }

        //if (other.tag == "Player" || other.tag == "MainCamera")
        //{
        //    PV = other.GetComponent<PhotonView>();
        //    Debug.Log(other.name);
        //    if (PV.IsMine)StartCoroutine(ScreenFade(0, 1));
        //    if (IsBreakNet)
        //    {
        //        playerInfo = other.transform.parent.parent.GetComponent<PlayerInfo>();
        //        //PN.Disconnect();
        //    }
        //}
#endif
        #region defore
        //if (other.tag == "Player"|| other.tag == "MainCamera")
        //{
        //    Debug.Log("OnTriggerEnter : Playertag : " + other.tag);
        //    Debug.Log("OnTriggerEnter : PlayerIsBreakNet : " + IsBreakNet);
        //    StartCoroutine(loadSc(SceneName));
        //    StartCoroutine(ScreenFade(0, 1));
        //    if (IsBreakNet)
        //    {

        //        ////여기다 디스커넥트
        //        //Debug.Log("OnTriggerEnter. my SID Player: " + NetworkManager.cInfo.id + "---------------------------");
        //        //NetworkManager.Instance.OnDisconnected();
        //        //NetworkManager.Instance.OnProDisconnected(NetworkManager.cInfo.id);
        //    }

        //}
        //if (other.tag == "User")
        //{
        //    //string newID = other.GetComponent<getUserInfo>().ID;//포톤전환시 주석
        //    //Debug.Log("new id is = " + newID);//포톤전환시 주석
        //    Debug.Log("OnTriggerEnter : Usertag : " + other.tag);
        //    Debug.Log("OnTriggerEnter : UserIsBreakNet : " + IsBreakNet);

        //    if (IsBreakNet)
        //    {
        //        ////여기다 디스커넥트
        //        //Debug.Log("OnTriggerEnter. my SID User: " + NetworkManager.cInfo.id + "---------------------------");
        //        //NetworkManager.Instance.OnProDisconnected(newID);
        //    }
        //}
        #endregion
    }

    public IEnumerator Leave()
    {
        Debug.Log("leving");
        isChangingSc = true;
        playerInfo.UseNetWork = false;
        p_manager.roomName = SceneName;
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom && IsDone) ;
        yield return null;
        // load previpous scene
    }

    IEnumerator loadSc(string scName)
    {
        Debug.Log($"{scName} 씬 로드");
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scName);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            //if (Input.GetKeyDown(KeyCode.L))
            if (IsDone)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        //SceneManager.LoadSceneAsync(scName, LoadSceneMode.Single);
    }
    public override void OnLeftRoom()
    {
        if (isActive && !IsBreakNet)
        {
            Debug.Log($"{SceneName} 씬 로드");
            SceneManager.LoadScene(SceneName);
        }

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    StartCoroutine(Leave());
    }
    #region 스크린페이드
    public IEnumerator ScreenFade(float start, float end)
    {
        Camera cam = Camera.main;
        renderer = cam.GetComponent<Renderer>();
        renderer.enabled = true;
        float nowTime = 0.0f;
        while (nowTime < gradientTime)
        {
            nowTime += Time.deltaTime;
            nowFadeAlpha = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / gradientTime));
            SetAlpha();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        IsDone = true;
        if (isChangingSc == false && !IsBreakNet && PN.OfflineMode == false) //씬이 변경중이 아니라면
        { Debug.Log("방떠남 코루틴 실행"); StartCoroutine(Leave()); }

    }

    private void SetAlpha()
    {
        Color color = fadeColor;
        color.a = Mathf.Max(currentAlpha, nowFadeAlpha);
        renderer.material.color = color;
    }
    #endregion

    private void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    GameObject collider = GameObject.Find("Player");
        //    playerInfo = collider.transform.parent.parent.GetComponent<PlayerInfo>();
        //}
    }
}