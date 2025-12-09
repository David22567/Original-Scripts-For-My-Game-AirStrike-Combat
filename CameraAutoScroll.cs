using UnityEngine;

public class CameraAutoScroll : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public bool loop = false;

    private int currentIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        MoveForward();
    }

    void MoveForward()
    {
        Transform target = waypoints[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                if (loop)
                    currentIndex = 0;
                else
                    enabled = false;
            }
        }
    }
}
