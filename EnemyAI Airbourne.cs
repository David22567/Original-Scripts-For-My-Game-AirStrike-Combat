using Unity;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAIAirbourne : MonoBehaviour
{
    public Transform enemy;
    public float value = 2f;
    public float stop = 2f;
    public Transform player;
    public Animator Heli;

    public float detectionRange = 30f;

    void Update()
    {
     transform.Translate (Vector3.back * Time.deltaTime * value);

      float dist = Vector3.Distance(enemy.position, player.position);
      if (dist > detectionRange) return;
       AimAtPlayer();
    }

    void AimAtPlayer()
    {
        transform.Translate (Vector3.forward * Time.deltaTime * stop);
        Debug.Log("Player In Range");
    }

}
