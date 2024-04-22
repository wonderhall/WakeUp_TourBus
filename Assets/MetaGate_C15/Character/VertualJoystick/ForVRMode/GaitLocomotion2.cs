using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class GaitLocomotion2 : MonoBehaviour
{
    #region nested classes

    [System.Serializable]
    public class GaitControls
    {
        [Tooltip("위일때 게이트 델타에 등록되어진다.")]
        public float m_upThreshold = 0.005f;
        [Tooltip("아래일때 게이트 델타에 등록되어진다.")]
        public float m_downThreshold = 0.003f;
        [Range(0f, 20f)]
        public float m_deltaMultiplier = 10f;
        [Range(0f, 1000f)]
        [Tooltip("최대 속도가 얼마나 빠른지 부르럽게 결정합니다.")]
        public float m_runSpeedMultiplier = 250f;
        [Tooltip("플레이어가 움직임을 멈춘 후 게임에서 움직임이 멈추기 전까지 얼마나 많은 시간이 지나야 하는지 결정한다")]
        public float m_stoppingTime = 0.5f;
        [Tooltip("최대 속도가 얼마나 빠른지 결정합니다.")]
        public float m_speed = 2f;

        [HideInInspector]
        public float m_actualSpeed;
    }

    public enum GaitAlignment { Camera, Direction, Forward }

    #endregion

    #region member variables

    //general params
    public Transform m_camera;
    public Transform[] m_directionsToFollow;
    public bool m_isMovingHMD = true;
    public GaitAlignment m_gaitAlignment = GaitAlignment.Camera;
    public GaitControls m_gaitControls;

    public int m_totalSteps;
    [Tooltip("발걸음 간격")]
    public float m_stepDistance = 1f;

    //보행인식과 속도
    private float m_delta;
    private float m_previousDelta;
    private float m_stepsDeltaTime;
    private float m_previousTime;

    //운동 트리거
    private bool m_tookStep;
    private bool m_moving;
    private bool m_Stopped;
    private float m_stoppingCounter;
    private bool canMove;
    private float m_stepCheckCounter = 0;

    public bool gettingInputButton = false;
    //플레이어가 움직일 캐릭터컨트롤러
    private CharacterController m_cc;

    private Vector3 m_oldPos;
    private Vector3 m_newPos;

    //public Text text;
    //public InputActionProperty LeftMoveJoyistic;
    //public InputActionProperty RightTeleportJoyistic;

    #endregion

    #region MonoBehaviours

    void Awake()
    {
        m_cc = GetComponent<CharacterController>();
        StartCoroutine(GetMovingCO()); //초기화 시 이동을 방지.


    }

    void FixedUpdate()
    {
        //text.text = gettingInputButton.ToString();
        if (!canMove) //초기화 시 이동을 방지.
            return;

        //실수로 움직이지 않도록 한다
        if (m_tookStep)
        {
            m_stepCheckCounter += Time.deltaTime;
            if (m_stepCheckCounter > .3f)
                m_tookStep = false;
        }

        if (m_Stopped)
        {
            m_stoppingCounter += Time.deltaTime;
            m_stoppingCounter = Mathf.Clamp(m_stoppingCounter, 0, m_gaitControls.m_stoppingTime + .1f);
            if (m_stoppingCounter > m_gaitControls.m_stoppingTime)
                m_moving = false;
        }
        else
        {
            m_stoppingCounter = 0;
        }
        m_Stopped = true;

        //델타는 가속도의 다양한 점프/충격을 아는 데 사용되는 값
        m_delta = GetDelta();

        //머리가 올라가고 있는지 확인 (머리 흔들기)
        if (m_delta > m_gaitControls.m_upThreshold && !m_tookStep)
        {
            //print("UP");

            StartCoroutine(StopGaitCO()); //웅크리고 서거나 주위를 둘러보면서 실수로 움직이지 않도록 한다
            m_tookStep = true;
            m_stepCheckCounter = 0;
            m_totalSteps++; // count step when comp goes high 
        }
        //머리가 내려가고 있는지 확인(머리 흔들기)
        else if (m_delta < -m_gaitControls.m_downThreshold && m_tookStep)
        {
            //print("DOWN");

            m_stepsDeltaTime = Mathf.Clamp(m_stepsDeltaTime, 0, Time.fixedTime - m_previousTime);
            m_previousTime = Time.fixedTime;

            m_tookStep = false; //발걸음을 반복하지 않도록 합니다.
            m_Stopped = false;
            m_moving = true;
        }

        // 스피드를 받아서 움직인다
        if (m_camera != null && m_cc != null)
        {
            m_delta = Mathf.Abs(m_delta);
            m_delta = Mathf.Lerp(m_previousDelta, m_delta, .05f);
            m_previousDelta = m_delta;

            //움직이지 않는 HMD에서 증폭
            if (m_isMovingHMD)
            {
                m_gaitControls.m_actualSpeed = Mathf.Lerp(m_gaitControls.m_actualSpeed, m_moving ? m_gaitControls.m_speed * m_delta * m_gaitControls.m_runSpeedMultiplier : 0f, .05f);
            }
            else
            {
                m_gaitControls.m_actualSpeed = Mathf.Lerp(m_gaitControls.m_actualSpeed, m_moving ? m_gaitControls.m_speed * m_delta * m_gaitControls.m_runSpeedMultiplier * 4 : 0f, .01f);
            }
            if (!gettingInputButton)//컨트롤러 인풋을 받고 있지 않을때만 실행
            {
                //방향을 평균화하고 평평하게 만듭니다.
                Vector3 dirToFollow = Vector3.zero;
                foreach (var d in m_directionsToFollow)
                {
                    dirToFollow += d.forward;
                }
                dirToFollow /= m_directionsToFollow.Length;
                dirToFollow = Vector3.ProjectOnPlane(dirToFollow, Vector3.up);

                Vector3 planarForward = Vector3.ProjectOnPlane((transform.forward * 1000).normalized, Vector3.up);
                Vector3 planarCamForward = Vector3.ProjectOnPlane((m_camera.forward * 1000).normalized, Vector3.up);

                Vector3 dir = m_gaitAlignment == GaitAlignment.Camera ? planarCamForward : m_gaitAlignment == GaitAlignment.Direction ? dirToFollow : planarForward;
                if (canMove) m_cc.Move(dir * m_gaitControls.m_actualSpeed * Time.deltaTime);

                ////캐릭터컨트롤러에 중력 적용
                //m_cc.Move(Physics.gravity * Time.deltaTime);
            }

        }
    }

    #endregion

    #region utility

    //getters
    public bool IsMoving()
    {
        return m_moving;
    }

    public float GetDistance()
    {
        return m_totalSteps * m_stepDistance;
    }

    public int GetTotalSteps()
    {
        return m_totalSteps;
    }

    public void ResetSteps()
    {
        m_totalSteps = 0;
    }

    public float GetSpeed()
    {
        return m_gaitControls.m_actualSpeed * Time.deltaTime;
    }

    //utils
    private IEnumerator GetMovingCO()
    {
        yield return new WaitForSeconds(1.5f);
        m_oldPos = m_camera.localPosition;
        m_newPos = m_oldPos;
        canMove = true;
    }

    private IEnumerator StopGaitCO()
    {
        yield return new WaitForSeconds(.2f);
        m_tookStep = false;
    }

    private float GetDelta()
    {
        m_newPos = m_camera.localPosition;
        var velocity = (m_newPos - m_oldPos);
        m_oldPos = m_newPos;
        m_newPos = m_camera.localPosition;
        return velocity.y;
    }

    #endregion
}