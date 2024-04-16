using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class GaitLocomotion : MonoBehaviour
{
    #region nested classes

    [System.Serializable]
    public class GaitControls
    {
        [Tooltip("When an UP is registered in the gait delta")]
        public float m_upThreshold = 0.005f;
        [Tooltip("When a DOWN is registered in the gait delta")]
        public float m_downThreshold = 0.003f;
        [Range(0f, 20f)]
        public float m_deltaMultiplier = 10f;
        [Range(0f, 1000f)]
        [Tooltip("Determines how fast the max speed is smoothly")]
        public float m_runSpeedMultiplier = 250f;
        [Tooltip("Determines how much time must pass after the player has stopped moving, before movement is stopped in game")]
        public float m_stoppingTime = 0.5f;
        [Tooltip("Determines how fast the max speed is harshly")]
        public float m_speed = 2f;

        [HideInInspector]
        public float m_actualSpeed;
    }

    public enum GaitAlignment { Camera, Direction, Forward}

    #endregion

    #region member variables

    //general params
    public Transform m_camera;
    public Transform[] m_directionsToFollow;
    public bool m_isMovingHMD = true;
    public GaitControls m_gaitControls;

    public int m_totalSteps;
    [Tooltip("Distance in meters for each step")]
    public float m_stepDistance = 1f;

    //gait recognition and speed
    private float m_delta;
    private float m_previousDelta;
    private float m_stepsDeltaTime;
    private float m_previousTime;

    //locomotion triggers
    private bool m_tookStep;
    private bool m_moving;
    private bool m_Stopped;
    private float m_stoppingCounter;
    private bool canMove;
    private float m_stepCheckCounter = 0;

    //character controller used for moving the player
    private CharacterController m_cc;

    private Vector3 m_oldPos;
    private Vector3 m_newPos;

    #endregion

    #region MonoBehaviours

    void Awake()
    {
        m_cc = GetComponent<CharacterController>();
        StartCoroutine(GetMovingCO()); //prevents moving at initialization


    }

    void FixedUpdate()
    {
        if (!canMove) //prevents moving at initialization
            return;

        //make sure we don't move by accident
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

        //delta is the value used to know the different jumps/ impulses in the acceleration
        m_delta = GetDelta();

        //check if we are going up (head bobbing)
        if (m_delta > m_gaitControls.m_upThreshold && !m_tookStep)
        {
            //print("UP");

            StartCoroutine(StopGaitCO()); //make sure we don't accidentally move with crouching and standing or just looking around
            m_tookStep = true;
            m_stepCheckCounter = 0;
            m_totalSteps++; // count step when comp goes high 
        }
        //check if we are going down (head bobbing)
        else if (m_delta < -m_gaitControls.m_downThreshold && m_tookStep)
        {
            //print("DOWN");

            m_stepsDeltaTime = Mathf.Clamp(m_stepsDeltaTime, 0, Time.fixedTime - m_previousTime);
            m_previousTime = Time.fixedTime;

            m_tookStep = false; //makes sure we don't repeat steps
            m_Stopped = false;
            m_moving = true;
        }

        // Get speed and move
        if (m_camera != null && m_cc != null)
        {
            m_delta = Mathf.Abs(m_delta);
            m_delta = Mathf.Lerp(m_previousDelta, m_delta, .05f);
            m_previousDelta = m_delta;

            //amplify on non moving HMDs
            if (m_isMovingHMD)
            {
                m_gaitControls.m_actualSpeed = Mathf.Lerp(m_gaitControls.m_actualSpeed, m_moving ? m_gaitControls.m_speed * m_delta * m_gaitControls.m_runSpeedMultiplier : 0f, .05f);
            }
            else
            {
                m_gaitControls.m_actualSpeed = Mathf.Lerp(m_gaitControls.m_actualSpeed, m_moving ? m_gaitControls.m_speed * m_delta * m_gaitControls.m_runSpeedMultiplier * 4 : 0f, .01f);
            }

   
            ////apply gravity to Character Controller
            //m_cc.Move(Physics.gravity * Time.deltaTime);
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