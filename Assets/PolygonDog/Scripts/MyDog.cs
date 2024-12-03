using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class MyDog : MonoBehaviour
{
    //���º���
    public bool isAction;//�ൿ��
    public bool isWalking;//�ȴ��� �Ǻ�

    [Range(0.0f, 1.0f)] public float strength = 1f;
    [SerializeField] private float walkTime;//�ȱ� �ð�
    [SerializeField] private float waitTime;//��� �ð�
    public float currentTime;

    private bool dogActionEnabled;
    private int countDown = 1;
    public bool Sleep_b = false;//���ڱ� �Ǻ�
    public bool Sit_b = false;// �ɱ� �Ǻ�
    [SerializeField] bool isActionDone;
    //�ʿ����۳�Ʈ
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private BoxCollider boxCol;
    [SerializeField] private RandomMove randomMove;


    //�������� �ʿ� ������Ʈ
    [SerializeField] private float EnemyDistanceRun = 3.0f;
    [SerializeField] private float EnemyMaxDistanceRun = 10f;
    private float TempEnemyDistanceRun;
    [Range(0.0f, 1.0f)] public float Favorability = 0f;

    private GameObject player;
    public bool IsCall;

    //��ư ����//


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


        //if(!PhotonNetwork.IsMasterClient)return;//���ϴ� ������ Ŭ���̾�Ʈ������ ����
        //Debug.Log("���� ������ Ŭ���̾�Ʈ");

    }


    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;//���ϴ� ������ Ŭ���̾�Ʈ������ ����
        Debug.Log("���� ������ Ŭ���̾�Ʈ");

        if (!IsCall)//�θ��� �������� ���� �ൿ ����
            ElapsedTime();

        if (isWalking) MinusStrength(); else if (Sit_b || Sleep_b) PlusStrength();// �ɰų� ���ﶧ ü��ȸ�� �ƴҶ� ü�� ����

        ComOrRun();//���ų� ���ų�

        EnemyDistanceRun = Mathf.Lerp(EnemyMaxDistanceRun, TempEnemyDistanceRun, Favorability);

       
    }
    private void ElapsedTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ResetAction();//���� ���� �ൿ ����
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

        isWalking = false; isAction = true;//����
        randomMove.IsRandomWalk = false; //�ȱ� ���߱�
        if (strength < 0.5f)//ü���� �ȵǸ� ���� ������ �����δ�
            RandomAction();//����������
        else if (Sit_b || Sleep_b)//ü���� ������ �ʰų� ���������� ����Ų��.
        {
            Sit_b = false; //�ɱ� ���� ����
            Sleep_b = false;
            anim.SetBool("Sit_b", Sit_b);
            anim.SetBool("Sleep_b", Sleep_b);
        }
        else RandomActionRun();//ü���� �Ǹ� �ڴ�.
    }

    private void RandomAction()//���� �׼� ������
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
    private void RandomActionRun()//���� �׼� ������
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
        Debug.Log("���");
        currentTime = waitTime;
        if (isWalking) randomMove.agent.SetDestination(this.transform.position);//�ȴ� ���̸� ����.
    }

    private void Sit()
    {
        if (Probability(Favorability)) // ȣ������ �������� ����� Ȯ���� �ö󰣴�
        {

            if (anim.GetFloat("Movement_f") == 0 && !randomMove.IsRandomWalk)
            {
                currentTime = waitTime;
                if (Sleep_b) Sleep_b = !Sleep_b;
                Sit_b = !Sit_b; //�ɱ� ���� ����
                anim.SetBool("Sit_b", Sit_b);
                anim.SetBool("Sleep_b", Sleep_b);
                Debug.Log("�ɱ�");
            }
        }
        else if (Sit_b)// ȣ������ ���� �ɾ������� ����� Ȯ���� �ö󰣴�
        {
            Sit_b = false; //�Ͼ
            anim.SetBool("Sit_b", Sit_b);
            Debug.Log("ȣ���� ���Ƽ� �Ͼ");
        }
        else // ȣ������ �������� ����� Ȯ���� �ö󰣴�
        {
            Debug.Log("ȣ���� ���Ƽ� ���� �ʰ� ¢��");
            BowWow();
        }

    }

    private void Sleep()
    {
        if (Probability(Favorability)) // ȣ������ �������� ����� Ȯ���� �ö󰣴�
        {
            if (anim.GetFloat("Movement_f") == 0 && !randomMove.IsRandomWalk)
            {
                currentTime = waitTime;

                if (Sit_b) Sit_b = !Sit_b;
                Sleep_b = !Sleep_b;
                anim.SetBool("Sit_b", Sit_b);
                anim.SetBool("Sleep_b", Sleep_b);

                Debug.Log("���ڱ�");
            }
        }
        else if (Sleep_b)// ȣ������ ���� �ɾ������� ����� Ȯ���� �ö󰣴�
        {
            Sleep_b = false; //�Ͼ
            anim.SetBool("Sleep_b", Sleep_b);
            Debug.Log("ȣ���� ���Ƽ� �Ͼ");
        }
        else
        {
            Debug.Log("ȣ���� ���Ƽ� ���� ���� �ʰ� ¢��");
            BowWow();
        }
    }

    private void BowWow()
    {
        if (anim.GetFloat("Movement_f") == 0)
        {
            Debug.Log("��!��!");
            StartCoroutine(DogActions(1));
        }

    }
    private void TryWalk()
    {
        if (isActionDone && !dogActionEnabled)
        {
            isWalking = true;
            currentTime = walkTime;
            randomMove.IsRandomWalk = !randomMove.IsRandomWalk;//�ȶٰ� ������ �ٰ� �ٰ������� ����
            Debug.Log("�ȱ�");
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



    ///����///
    public AudioClip bark;
    public AudioClip whistle;
    public Transform jawBone;

    void Bark() // fbx ���ϸ��̼� �̺�Ʈ���� �ҷ� ��
    {
        // �̺�Ʈ�� �߻��ϸ� �߼Ҹ� ���� ���
        AudioSource.PlayClipAtPoint(bark, jawBone.position, 10);
    }

    void Whistle()
    {
        //// �̺�Ʈ�� �߻��ϸ� �߼Ҹ� ���� ���
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
    //����//

    //---> ���ų� ���ų�
    float distanceFromPlayer = 2f;
    void ComOrRun()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        float calRemainDistance = randomMove.agent.remainingDistance - randomMove.agent.stoppingDistance;

        if (IsCall && Favorability > 0.7)// ���� �θ� ���
        {
            if (Sit_b || Sleep_b)
            {
                Sit_b = false;
                Sleep_b = false;
                anim.SetBool("Sit_b", Sit_b);
                anim.SetBool("Sleep_b", Sleep_b);
            }

            randomMove.agent.stoppingDistance = distanceFromPlayer;
            strength = 0f;//ü���� ������ ���� �� �־ Ǯ�� ä��

            randomMove.agent.SetDestination(player.transform.position);
            randomMove.agent.speed = Mathf.Lerp(0.0f, 4f, calRemainDistance);
            anim.SetFloat("hurryUpSpeed", Mathf.Lerp(1, 3, calRemainDistance));
            anim.SetFloat("Movement_f", Mathf.Lerp(0.0f, 4, calRemainDistance));

            //Debug.Log(calRemainDistance.ToString("N1"));

            if (calRemainDistance <= 0  ) //�Ÿ��� ����� ������ ���º�ȭ
            {
                StopCoroutine("CancleIsCall");
                StartCoroutine("CancleIsCall");
            }

        }
        else //����--->
        {

            //Debug.Log("distance = " + distance);

            //�÷��̾�� ���� ����
            if (distance < EnemyDistanceRun)
            {
                Favorability = Mathf.Clamp((Favorability - Time.deltaTime * 0.1f), 0, 1);
                // ---->�ɰų� �������� �������� ���� ����Ű��
                anim.SetFloat("hurryUpSpeed", 3);//�� ����Ű��
                strength = 1;//ü���� ������ ���� �� �־ Ǯ�� ä��
                anim.SetFloat("Movement_f", 1);//���ϸ��̼� ��� �޸���
                randomMove.agent.speed = 4;// �׺�����Ʈ ���ǵ� ��
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
                    //�÷��̾�� ������ ����
                    Vector3 dirToPlayer = transform.position - player.transform.position;
                    Vector3 newPos = transform.position + dirToPlayer;
                    randomMove.agent.SetDestination(newPos);
                }
            }
            else
            {
                anim.SetFloat("hurryUpSpeed", 1);
                Favorability = Mathf.Clamp((Favorability + Time.deltaTime * 0.1f), 0, 1);//ȣ���� ȸ��
            }//<------����

        }

    }

    public void CallDog()
    {
        IsCall = true;
        Debug.Log("���� �ҷ����");
        Whistle();
    }

    IEnumerator enumerator = null;
    IEnumerator CancleIsCall()
    {
        yield return new WaitForSeconds(5.0f);
        IsCall = false;
    }
    //<-------���ų� ���ų�
    bool Probability(float chance)  //Ȯ��
    {
        return UnityEngine.Random.value <= chance;
    }
}
