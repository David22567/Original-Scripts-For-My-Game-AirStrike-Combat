using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    public float radius = 5f;
    public float force = 200f;
    public float upwardModifier = 1f;

    void Start()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider c in cols)
        {
            Rigidbody rb = c.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius, upwardModifier, ForceMode.Impulse);
            }
        }
    }
}
