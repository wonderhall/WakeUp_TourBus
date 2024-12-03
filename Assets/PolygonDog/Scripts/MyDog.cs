using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class MyDog : MonoBehaviour
{
    //상태변수
    public bool isAction;//행동중
    public bool isWalking;//걷는지 판별

    [Range(0.0f, 1.0f)] public float strength = 1f;
    [SerializeField] private float walkTime;//걷기 시간
    [SerializeField] private float waitTime;//대기 시간
    public float currentTime;

    private bool dogActionEnabled;
    private int countDown = 1;
    public bool Sleep_b = false;//잠자기 판별
    public bool Sit_b = false;// 앉기 판별
    [SerializeField] bool isActionDone;
    //필요컴퍼넌트
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private BoxCollider boxCol;
    [SerializeField] private RandomMove randomMove;


    //추적도망 필요 컴포넌트
    [SerializeField] private float EnemyDistanceRun = 3.0f;
    [SerializeField] private float EnemyMaxDistanceRun = 10f;
    private float TempEnemyDistanceRun;
    [Range(0.0f, 1.0f)] public float Favorability = 0f;

    private GameObject player;
    public bool IsCall;

    //버튼 연결//


    private void Awake()
    {
        
     
    }
    // Start is called before the first frame update
    void Start()
    {

        TempEnemyDistanceRun = EnemyDistanceRun;
        isActionDone = true;
        player = GameObject.FindWithTag("Player");
        randomMove = GetComponent<RandomMove>();
        anim = GetComponent<Animator>();
        currentTime = waitTime;
        isAction = true;
        ResetAction();


        //if(!PhotonNetwork.IsMasterClient)return;//이하는 마스터 클라이언트에서만 실행
        //Debug.Log("포톤 마스터 클라이언트");

    }


    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;//이하는 마스터 클라이언트에서만 실행
        Debug.Log("포톤 마스터 클라이언트");

        if (!IsCall)//부르지 않을때만 랜덤 행동 개시
            ElapsedTime();

        if (isWalking) MinusStrength(); else if (Sit_b || Sleep_b) PlusStrength();// 앉거나 누울때 체력회복 아닐때 체력 방전

        ComOrRun();//오거나 가거나

        EnemyDistanceRun = Mathf.Lerp(EnemyMaxDistanceRun, TempEnemyDistanceRun, Favorability);

       
    }
    private void ElapsedTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ResetAction();//다음 랜덤 행동 개시
        }
    }
    private void MinusStrength()
    {
        if (isAction)
        {
            if (strength > 0)
                strength -= Time.deltaTime * 0.5f;
        }
    }
    private void PlusStrength()
    {
        if (isAction)
        {
            if (strength < 1)
                strength += Time.deltaTime * 0.05f;
        }
    }
    private void ResetAction()
    {

        isWalking = false; isAction = true;//리셋
        randomMove.IsRandomWalk = false; //걷기 멈추기
        if (strength < 0.5f)//체력이 안되면 쉬고 남으면 움직인다
            RandomAction();//램덤돌리기
        else if (Sit_b || Sleep_b)//체력은 되지만 않거나 누워있으면 일으킨다.
        {
            Sit_b = false; //앉기 상태 반전
            Sleep_b = false;
            anim.SetBool("Sit_b", Sit_b);
            anim.SetBool("Sleep_b", Sleep_b);
        }
        else RandomActionRun();//체력이 되면 뛴다.
    }

    private void RandomAction()//랜덤 액션 돌리기
    {
        isAction = true;
        int random = UnityEngine.Random.Range(0, 4);
        if (random == 0)
            Wait();
        if (random == 1)
            Sit();
        if (random == 2)
            Sleep();
        if (random == 3)
            BowWow();
    }
    private void RandomActionRun()//랜덤 액션 돌리기
    {
        isAction = true;
        int random = UnityEngine.Random.Range(0, 3);
        if (random == 0)
            Wait();
        if (random == 1)
            TryWalk();
        if (random == 2)
            TryWalk();
    }
    private void Wait()
    {
        Debug.Log("대기");
        currentTime = waitTime;
        if (isWalking) randomMove.agent.SetDestination(this.transform.position);//걷는 중이면 멈춤.
    }

    private void Sit()
    {
        if (Probability(Favorability)) // 호감도가 높을수록 실행될 확률이 올라간다
        {

            if (anim.GetFloat("Movement_f") == 0 && !randomMove.IsRandomWalk)
            {
                currentTime = waitTime;
                if (Sleep_b) Sleep_b = !Sleep_b;
                Sit_b = !Sit_b; //앉기 상태 반전
                anim.SetBool("Sit_b", Sit_b);
                anim.SetBool("Sleep_b", Sleep_b);
                Debug.Log("앉기");
            }
        }
        else if (Sit_b)// 호감도고 낮고 앉아있으면 실행될 확률이 올라간다
        {
            Sit_b = false; //일어남
            anim.SetBool("Sit_b", Sit_b);
            Debug.Log("호감도 낮아서 일어남");
        }
        else // 호감도가 낮을수록 실행될 확률이 올라간다
        {
            Debug.Log("호감도 낮아서 앉지 않고 짖음");
            BowWow();
        }

    }

    private void Sleep()
    {
        if (Probability(Favorability)) // 호감도가 높을수록 실행될 확률이 올라간다
        {
            if (anim.GetFloat("Movement_f") == 0 && !randomMove.IsRandomWalk)
            {
                currentTime = waitTime;

                if (Sit_b) Sit_b = !Sit_b;
                Sleep_b = !Sleep_b;
                anim.SetBool("Sit_b", Sit_b);
                anim.SetBool("Sleep_b", Sleep_b);

                Debug.Log("잠자기");
            }
        }
        else if (Sleep_b)// 호감도고 낮고 앉아있으면 실행될 확률이 올라간다
        {
            Sleep_b = false; //일어남
            anim.SetBool("Sleep_b", Sleep_b);
            Debug.Log("호감도 낮아서 일어남");
        }
        else
        {
            Debug.Log("호감도 낮아서 앉지 자지 않고 짖음");
            BowWow();
        }
    }

    private void BowWow()
    {
        if (anim.GetFloat("Movement_f") == 0)
        {
            Debug.Log("멍!멍!");
            StartCoroutine(DogActions(1));
        }

    }
    private void TryWalk()
    {
        if (isActionDone && !dogActionEnabled)
        {
            isWalking = true;
            currentTime = walkTime;
            randomMove.IsRandomWalk = !randomMove.IsRandomWalk;//안뛰고 있으면 뛰고 뛰고있으면 스기
            Debug.Log("걷기");
        }

    }


    IEnumerator DogActions(int actionType) // Dog action coroutine
    {
        dogActionEnabled = true; // Enable the dog animation flag
        anim.SetInteger("ActionType_int", actionType); // Enable Animation
        yield return new WaitForSeconds(countDown); // Countdown
        anim.SetInteger("ActionType_int", 0); // Disable animation
        dogActionEnabled = false; // Disable the dog animation flag
    }



    ///사운드///
    public AudioClip bark;
    public AudioClip whistle;
    public Transform jawBone;

    void Bark() // fbx 에니메이션 이벤트에서 불러 옮
    {
        // 이벤트가 발생하면 발소리 사운드 재생
        AudioSource.PlayClipAtPoint(bark, jawBone.position, 10);
    }

    void Whistle()
    {
        //// 이벤트가 발생하면 발소리 사운드 재생
        //AudioSource.PlayClipAtPoint(whistle, player.transform.position, 10);
        
    }
    void ActionDone(string TrueFalse)
    {
        bool isTrue;
        if (TrueFalse == "true") isTrue = true;
        else isTrue = false;

        isActionDone = isTrue;

        Debug.Log("isActionDone = " + isActionDone);
    }
    //사운드//

    //---> 오거나 가거나
    float distanceFromPlayer = 2f;
    void ComOrRun()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        float calRemainDistance = randomMove.agent.remainingDistance - randomMove.agent.stoppingDistance;

        if (IsCall && Favorability > 0.7)// 개를 부를 경우
        {
            if (Sit_b || Sleep_b)
            {
                Sit_b = false;
                Sleep_b = false;
                anim.SetBool("Sit_b", Sit_b);
                anim.SetBool("Sleep_b", Sleep_b);
            }

            randomMove.agent.stoppingDistance = distanceFromPlayer;
            strength = 0f;//체력이 적으면 누울 수 있어서 풀로 채춤

            randomMove.agent.SetDestination(player.transform.position);
            randomMove.agent.speed = Mathf.Lerp(0.0f, 4f, calRemainDistance);
            anim.SetFloat("hurryUpSpeed", Mathf.Lerp(1, 3, calRemainDistance));
            anim.SetFloat("Movement_f", Mathf.Lerp(0.0f, 4, calRemainDistance));

            //Debug.Log(calRemainDistance.ToString("N1"));

            if (calRemainDistance <= 0  ) //거리가 가까워 질수록 상태변화
            {
                StopCoroutine("CancleIsCall");
                StartCoroutine("CancleIsCall");
            }

        }
        else //도망--->
        {

            //Debug.Log("distance = " + distance);

            //플레이어로 부터 도망
            if (distance < EnemyDistanceRun)
            {
                Favorability = Mathf.Clamp((Favorability - Time.deltaTime * 0.1f), 0, 1);
                // ---->앉거나 누웠으면 도망가기 위해 일으키기
                anim.SetFloat("hurryUpSpeed", 3);//얼른 일으키기
                strength = 1;//체력이 적으면 누울 수 있어서 풀로 채춤
                anim.SetFloat("Movement_f", 1);//에니메이션 모션 달리기
                randomMove.agent.speed = 4;// 네비에이전트 스피드 업
                if (Sit_b || Sleep_b)
                {
                    Sit_b = !Sit_b;
                    Sleep_b = !Sleep_b;
                    anim.SetBool("Sit_b", Sit_b);
                    anim.SetBool("Sleep_b", Sleep_b);
                }
                //<-----------

                if (isActionDone && !dogActionEnabled)
                {
                    //플레이어에서 나까지 벡터
                    Vector3 dirToPlayer = transform.position - player.transform.position;
                    Vector3 newPos = transform.position + dirToPlayer;
                    randomMove.agent.SetDestination(newPos);
                }
            }
            else
            {
                anim.SetFloat("hurryUpSpeed", 1);
                Favorability = Mathf.Clamp((Favorability + Time.deltaTime * 0.1f), 0, 1);//호감도 회복
            }//<------도망

        }

    }

    public void CallDog()
    {
        IsCall = true;
        Debug.Log("개를 불렀어요");
        Whistle();
    }

    IEnumerator enumerator = null;
    IEnumerator CancleIsCall()
    {
        yield return new WaitForSeconds(5.0f);
        IsCall = false;
    }
    //<-------오거나 가거나
    bool Probability(float chance)  //확률
    {
        return UnityEngine.Random.value <= chance;
    }
}
