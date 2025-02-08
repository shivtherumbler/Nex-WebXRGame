using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // First position
    public Transform pointB; // Second position
    public float speed = 3f; // Movement speed
    public float waitTime = 3f; // Time to wait at each stop

    private Vector3 target; // Current movement target
    private bool isWaiting = false; // If platform is waiting

    void Start()
    {
        target = pointB.position; // Start moving towards B
    }

    void Update()
    {
        if (!isWaiting) // Move only if not waiting
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // If platform reaches the target, start waiting
            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                StartCoroutine(WaitBeforeMoving());
            }
        }
    }

    IEnumerator WaitBeforeMoving()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime); // Wait for 3 seconds
        target = (target == pointA.position) ? pointB.position : pointA.position; // Swap target
        isWaiting = false;
    }
}
