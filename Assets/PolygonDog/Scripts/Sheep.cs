using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject player;
    public float EnemyDistanceRun = 4.0f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log("distance = " + distance);

        //�÷��̾�� ���� ����
        if (distance < EnemyDistanceRun)
        {
            //�÷��̾�� ������ ����
            Vector3 dirToPlayer = transform.position - player.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            agent.SetDestination(newPos);

        }
    }
}
