using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;
using PN = Photon.Pun.PhotonNetwork;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using Photon.Realtime;

//using UnityEngine.XR.Interaction.Toolkit.Interactors;
//using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Content.Interaction
{
    #region before xr인터렉션 버전"3.0.3"에서만 가능
    //public class Door : MonoBehaviour
    //{
    //    [SerializeField]
    //    HingeJoint m_DoorJoint;

    //    [SerializeField]
    //    [Tooltip("Transform joint that pulls a door to follow an interactor")]
    //    TransformJoint m_DoorPuller;

    //    [SerializeField]
    //    GameObject m_KeyKnob;

    //    [SerializeField]
    //    float m_HandleOpenValue = 0.1f;

    //    [SerializeField]
    //    float m_HandleCloseValue = 0.5f;

    //    [SerializeField]
    //    float m_HingeCloseAngle = 5.0f;

    //    [SerializeField]
    //    float m_KeyLockValue = 0.9f;

    //    [SerializeField]
    //    float m_KeyUnlockValue = 0.1f;

    //    [SerializeField]
    //    float m_KeyPullDistance = 0.1f;

    //    [SerializeField]
    //    [Tooltip("Events to fire when the door is locked.")]
    //    UnityEvent m_OnLock = new UnityEvent();

    //    [SerializeField]
    //    [Tooltip("Events to fire when the door is unlocked.")]
    //    UnityEvent m_OnUnlock = new UnityEvent();

    //    JointLimits m_OpenDoorLimits;
    //    JointLimits m_ClosedDoorLimits;
    //    bool m_Closed = false;
    //    float m_LastHandleValue = 1.0f;

    //    bool m_Locked = false;

    //    GameObject m_KeySocket;
    //    IXRSelectInteractable m_Key;

    //    XRBaseInteractor m_KnobInteractor;
    //    Transform m_KnobInteractorAttachTransform;

    //    /// <summary>
    //    /// Events to fire when the door is locked.
    //    /// </summary>
    //    public UnityEvent onLock => m_OnLock;

    //    /// <summary>
    //    /// Events to fire when the door is unlocked.
    //    /// </summary>
    //    public UnityEvent onUnlock => m_OnUnlock;

    //    void Start()
    //    {
    //        m_OpenDoorLimits = m_DoorJoint.limits;
    //        m_ClosedDoorLimits = m_OpenDoorLimits;
    //        m_ClosedDoorLimits.min = 0.0f;
    //        m_ClosedDoorLimits.max = 0.0f;
    //        m_DoorJoint.limits = m_ClosedDoorLimits;
    //        m_KeyKnob.SetActive(false);
    //        m_Closed = true;
    //    }

    //    void Update()
    //    {
    //        // If the door is open, keep track of the hinge joint and see if it enters a state where it should close again
    //        if (!m_Closed)
    //        {
    //            if (m_LastHandleValue < m_HandleCloseValue)
    //                return;

    //            if (Mathf.Abs(m_DoorJoint.angle) < m_HingeCloseAngle)
    //            {
    //                m_DoorJoint.limits = m_ClosedDoorLimits;
    //                m_Closed = true;
    //            }
    //        }

    //        if (m_KnobInteractor != null && m_KnobInteractorAttachTransform != null)
    //        {
    //            var distance = (m_KnobInteractorAttachTransform.position - m_KeyKnob.transform.position).magnitude;

    //            // If over threshold, break and grant the key back to the interactor
    //            if (distance > m_KeyPullDistance)
    //            {
    //                var newKeyInteractor = m_KnobInteractor;
    //                m_KeySocket.SetActive(true);
    //                m_Key.transform.gameObject.SetActive(true);
    //                newKeyInteractor.interactionManager.SelectEnter(newKeyInteractor, m_Key);
    //                m_KeyKnob.SetActive(false);
    //            }
    //        }
    //    }

    //    public void BeginDoorPulling(SelectEnterEventArgs args)
    //    {
    //        m_DoorPuller.connectedBody = args.interactorObject.GetAttachTransform(args.interactableObject);
    //        m_DoorPuller.enabled = true;
    //    }

    //    public void EndDoorPulling()
    //    {
    //        m_DoorPuller.enabled = false;
    //        m_DoorPuller.connectedBody = null;
    //    }

    //    public void DoorHandleUpdate(float handleValue)
    //    {
    //        m_LastHandleValue = handleValue;

    //        if (!m_Closed || m_Locked)
    //            return;

    //        if (handleValue < m_HandleOpenValue)
    //        {
    //            m_DoorJoint.limits = m_OpenDoorLimits;
    //            m_Closed = false;
    //        }
    //    }

    //    public void KeyDropUpdate(SelectEnterEventArgs args)
    //    {
    //        m_KeySocket = args.interactorObject.transform.gameObject;
    //        m_Key = args.interactableObject;
    //        m_KeySocket.SetActive(false);
    //        m_Key.transform.gameObject.SetActive(false);
    //        m_KeyKnob.SetActive(true);
    //    }

    //    public void KeyUpdate(float keyValue)
    //    {
    //        if (!m_Locked && keyValue > m_KeyLockValue)
    //        {
    //            m_Locked = true;
    //            m_OnLock.Invoke();
    //        }

    //        if (m_Locked && keyValue < m_KeyUnlockValue)
    //        {
    //            m_Locked = false;
    //            m_OnUnlock.Invoke();
    //        }
    //    }

    //    public void KeyLockSelect(SelectEnterEventArgs args)
    //    {
    //        m_KnobInteractor = args.interactorObject as XRBaseInteractor;
    //        m_KnobInteractorAttachTransform = args.interactorObject.GetAttachTransform(args.interactableObject);
    //    }

    //    public void KeyLockDeselect(SelectExitEventArgs args)
    //    {
    //        m_KnobInteractor = null;
    //        m_KnobInteractorAttachTransform = null;
    //    }
    //} 
    #endregion
    public class Door : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        HingeJoint m_DoorJoint;

        [SerializeField]
        [Tooltip("Transform joint that pulls a door to follow an interactor")]
        TransformJoint m_DoorPuller;

        [SerializeField]
        float m_HandleOpenValue = 0.1f;

        [SerializeField]
        float m_HandleCloseValue = 0.5f;

        [SerializeField]
        float m_HingeCloseAngle = 5.0f;

        [SerializeField]
        [Tooltip("Events to fire when the door is locked.")]
        UnityEvent m_OnLock = new UnityEvent();

        [SerializeField]
        [Tooltip("Events to fire when the door is unlocked.")]
        UnityEvent m_OnUnlock = new UnityEvent();

        JointLimits m_OpenDoorLimits;
        JointLimits m_ClosedDoorLimits;
        bool m_Closed = false;
        float m_LastHandleValue = 1.0f;

        bool m_Locked = false;


        [Header("네트워크 및 이벤트설정")]
        public string SceneName;
        private bool IsDone;
        public bool IsBreakNet;
        private bool isActive;
        private Volume Vol;
        private ColorAdjustments ColorAdj;
        public GameObject OpenLightVolume;
        public GameObject FocusEffect;
        public ParticleSystem[] psArray;
        public int RoomIdx = 0;
        public bool DoorLocked;
        Collider colliderForUserCheck;
        private DoorUnLockCheck doorUnLockCheck;

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

        /// <summary>
        /// Events to fire when the door is locked.
        /// </summary>
        public UnityEvent onLock => m_OnLock;

        /// <summary>
        /// Events to fire when the door is unlocked.
        /// </summary>
        public UnityEvent onUnlock => m_OnUnlock;



        void Start()
        {
            for (int i = 0; i < UserInfo.UnlockRoom.Length; i++)
            {
                Debug.Log($"{i}번 잠김상태는 {UserInfo.UnlockRoom[i]}이다");
            }
            m_OpenDoorLimits = m_DoorJoint.limits;
            m_ClosedDoorLimits = m_OpenDoorLimits;
            m_ClosedDoorLimits.min = 0.0f;
            m_ClosedDoorLimits.max = 0.0f;
            m_DoorJoint.limits = m_ClosedDoorLimits;
            //m_KeyKnob.SetActive(false);
            m_Closed = true;

            #region 네트워크 및 이벤트-->
            doorUnLockCheck = GetComponent<DoorUnLockCheck>();
            playerInfo = GameObject.FindAnyObjectByType<PlayerInfo>();
            p_manager = GameObject.FindAnyObjectByType<PhotonManager>();
            colliderForUserCheck = GetComponent<Collider>();
            Vol = FindAnyObjectByType<Volume>();
#if ForVR
            colliderForUserCheck.enabled = false;
#endif
            OpenLightVolume.SetActive(false);//문 뒤편 화이트
            UserInfo.UnlockRoom[0] = true;
            DoorLocked = !UserInfo.UnlockRoom[RoomIdx];
            
            psArray = FocusEffect.transform.GetComponentsInChildren<ParticleSystem>();

            if (!DoorLocked)//문이 열려있으면
            {
                foreach (var item in psArray) StartCoroutine(particleColorChange(item, Color.cyan));

            }
            else//잠겨있으면
            {
                foreach (var item in psArray) StartCoroutine(particleColorChange(item, Color.red));
            }

            ColorAdjustments adj;

            if (Vol.profile.TryGet<ColorAdjustments>(out adj))

            {

                ColorAdj = adj;

            }
            ColorAdj.postExposure.overrideState = true;
#endregion <--
        }

        void Update()
        {
            // If the door is open, keep track of the hinge joint and see if it enters a state where it should close again
            if (!m_Closed)
            {
                if (m_LastHandleValue < m_HandleCloseValue)
                    return;

                if (Mathf.Abs(m_DoorJoint.angle) < m_HingeCloseAngle)
                {
                    m_DoorJoint.limits = m_ClosedDoorLimits;
                    m_Closed = true;
                }
            }

        }



        public void BeginDoorPulling(SelectEnterEventArgs args)
        {
            if (!DoorLocked)
            {
                m_DoorPuller.connectedBody = args.interactorObject.GetAttachTransform(args.interactableObject);
                m_DoorPuller.enabled = true;
                colliderForUserCheck.enabled = true;//방이동을 위해 유저와 충돌할 컬리전 킴.
                OpenLightVolume.SetActive(true);//문 뒤편 화이트
                FocusEffect.SetActive(false);
                doorUnLockCheck.SetRoomUnLock();
                Debug.Log("밀기시작");
            }
        }

        public void EndDoorPulling()
        {
            if (!DoorLocked)
            {
                m_DoorPuller.enabled = false;
                m_DoorPuller.connectedBody = null;
            }

        }

        public void DoorHandleUpdate(float handleValue)//문 회전 업데이트
        {
            if (!DoorLocked)
            {
                m_LastHandleValue = handleValue;

                if (!m_Closed || m_Locked)
                    return;

                if (handleValue < m_HandleOpenValue)
                {
                    m_DoorJoint.limits = m_OpenDoorLimits;
                    m_Closed = false;
                }
            }
        }
        /// <summary>
        /// 네트워크 및 이벤트처리

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
                    StartCoroutine(ScreenFade(0.3f, 7f));
                }

                else //오프라인 룸에서  오프라인 룸으로 실행
                {
                    Debug.Log("오프라인 룸에서  오프라인 룸으로 실행");
                    StartCoroutine(loadSc(SceneName));
                    StartCoroutine(ScreenFade(0.3f, 7f));
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
                        Debug.Log(" 온라인 룸으로 실행");
                        StartCoroutine(ScreenFade(0.3f, 7f));
                    }

                    else //오프라인 룸에서  오프라인 룸으로 실행
                    {
                        Debug.Log(" 오프라인 룸으로 실행");
                        StartCoroutine(ScreenFade(0.3f, 7f));
                        StartCoroutine(loadSc(SceneName));
                    }
                }
            }
#endif

        }


        IEnumerator loadSc(string scName)
        {
            Debug.Log($"{scName} 에이싱크로드로 들어옴");
            yield return null;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scName);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                Debug.Log($"{scName} 에이싱크 대기중");
                if (IsDone)
                {
                    asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }

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

        public override void OnLeftRoom()
        {
            if (isActive && !IsBreakNet)
            {
                Debug.Log($"{SceneName} 씬 로드");
                SceneManager.LoadScene(SceneName);
            }

        }
#region 스크린페이드
        private IEnumerator ScreenFade(float start, float end)
        {
            float nowTime = 0.0f;
            while (nowTime < gradientTime)
            {
                nowTime += Time.deltaTime;
                ColorAdj.postExposure.value = Mathf.Lerp(start, end, Mathf.Clamp01(nowTime / gradientTime));
                Debug.Log($"현재 포스트이펙트 컬러노출값은 '{ColorAdj.postExposure.value}' 이다");
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1);
            IsDone = true;
            if (isChangingSc == false && !IsBreakNet && PN.OfflineMode == false) //���� �������� �ƴ϶��
            {
                Debug.Log("방떠남 코루틴 실행"); 
                StartCoroutine(Leave());
            }
           
        }
#endregion
        private IEnumerator particleColorChange(ParticleSystem particle, Color color)
        {
            yield return new WaitForSeconds(1);
            ParticleSystem.MainModule main = particle.main;
            main.startColor = color;
            //Debug.Log($"파티클 색깔을{color}로 변경");
        }


    }
}
