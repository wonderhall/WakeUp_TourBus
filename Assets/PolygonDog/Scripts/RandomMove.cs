using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //important

//if you use this code you are contractually obligated to like the YT video
public class RandomMove : MonoBehaviour //don't forget to change the script name if you haven't
{
    public NavMeshAgent agent;
    public bool IsRandomWalk;

    [SerializeField] Transform centrePoint; //centre of the area the agent wants to move around in
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float range; //radius of sphere


    private Animator animator;
    private float tempStoppongDistance;
    float tempAgentSpeed;
    [SerializeField] float TimeForRun = 4;
    private Vector3 destination;
    [SerializeField]private float runSpeed;
    private MyDog mydog;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        mydog = GetComponent<MyDog>();
        tempAgentSpeed = agent.speed;
        animator.SetFloat("Movement_f", 0);
        tempStoppongDistance = agent.stoppingDistance;

    }


    void Update()
    {
        if (mydog != null && !mydog.IsCall)
        {
            if (IsRandomWalk)
            {
                //agent.speed = agentSpeed;
                agent.stoppingDistance = tempStoppongDistance;
                if (agent.remainingDistance <= agent.stoppingDistance) //done with path
                {
                    Vector3 point;
                    if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
                    {
                        Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                        agent.SetDestination(point);
                    }
                }

                ///--------------에니메이션 및 에이전트 스피드 조절
                if (animator != null && agent.remainingDistance > TimeForRun)
                {
                    if (agent.speed < maxSpeed)//달리기 속도 업
                        agent.speed += Time.deltaTime * 2f;


                    float v = Mathf.Clamp((animator.GetFloat("Movement_f") + Time.deltaTime * 0.5f), 0.5f, 1);

                    animator.SetFloat("Movement_f", v);//에니메이션 스피드 업
                }
                else if (animator != null && agent.remainingDistance < TimeForRun)
                {
                    if (agent.speed > tempAgentSpeed)//달리기 속도 업
                        agent.speed -= Time.deltaTime;

                    float v = Mathf.Clamp((animator.GetFloat("Movement_f") - Time.deltaTime * 0.5f), 0.5f, 1);

                    animator.SetFloat("Movement_f", v);//에니메이션 스피드 업
                }


                //---------------- ///에니메이션 및 에이전트 스피드 조절


            }
            else if (!IsRandomWalk && animator.GetFloat("Movement_f") > 0)//걷기가 꺼지면 보간
            {
                agent.stoppingDistance = 0f;
                agent.ResetPath();
                float speed = animator.GetFloat("Movement_f");
                speed -= Time.deltaTime;
                animator.SetFloat("Movement_f", speed);
            }
            else animator.SetFloat("Movement_f", 0);//움직임이 멈추면
        }

    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


}
