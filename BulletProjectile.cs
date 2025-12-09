using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public float speed = 60f;
    public float lifeTime = 3f;
    public float damage = 10f;

    public GameObject hitEffect;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = -transform.right * speed;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Quaternion hitRot = Quaternion.LookRotation(contact.normal);

        if (hitEffect)
            Instantiate(hitEffect, hitPoint, hitRot);

        PlayerHealth hp = collision.collider.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
