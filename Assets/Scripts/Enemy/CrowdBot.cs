using UnityEngine;
using UnityEngine.AI;

public class CrowdBot : MonoBehaviour
{
    public GameObject[] goalLocations;
    public float detectionRadius = 25f;
    public float fleeRadius = 15f;
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ResetAgent();
    }

    void Update()
    {
        // If within the detection range of the player
        if (Vector3.Distance(transform.position, player.transform.position) < 10)
        {
            anim.SetBool("walk", false);
            GetComponent<LineOfSight>().enabled = true;
            this.enabled = false;
        }
        else
        {
            anim.SetBool("walk", true);
            GetComponent<LineOfSight>().enabled = false;
        }

        // Continue moving to random goal locations if not close to the player
        if (agent.remainingDistance < 1)
        {
            ResetAgent();
            agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        }
    }

    public void ResetAgent()
    {
        agent.speed = Random.Range(0.35f, 1.5f);
        anim.SetBool("walk", true);
    }

    public void DetectNewObstacle(Vector3 position)
    {
        if (Vector3.Distance(position, transform.position) < detectionRadius)
        {
            Vector3 fleeDirection = (transform.position - position).normalized;
            Vector3 newGoal = transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newGoal, path);

            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                agent.speed = 10;
            }
        }
    }
}
