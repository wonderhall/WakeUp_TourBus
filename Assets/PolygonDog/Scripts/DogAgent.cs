using UnityEngine;
using UnityEngine.AI;

public class DogAgent : MonoBehaviour
{
    NavMeshAgent nav;
    [SerializeField] Transform target;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
