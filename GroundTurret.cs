using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GroundTurret : MonoBehaviour
{
    private const bool V = true;
    [Header("References")]
    public Transform turretHead;
    public Transform firePoint;
    public Transform player;

    [Header("Settings")]
    public float rotationSpeed = 5f;
    public float detectionRange = 25f;
    public float fireRate = 0.5f;

    [Header("Bullet")]
    public GameObject bulletPrefab;    
    public float bulletDamage = 10f;
    public float bulletSpeed = 60f;

    [Header("Effects")]
    public GameObject muzzleFlash;

    private float nextFireTime;

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(turretHead.position, player.position);
        if (dist > detectionRange) return;

        AimAtPlayer();
        TryShoot();
    }

    public void AimAtPlayer()
    {
        Vector3 dir = player.position - turretHead.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        lookRot *= Quaternion.Euler(90f, 90f, 0);

        turretHead.rotation = Quaternion.Slerp(
            turretHead.rotation,
            lookRot,
            rotationSpeed * Time.deltaTime
        );
    }
    
    void TryShoot()
    {   
          if (Time.time < nextFireTime) return;
          nextFireTime = Time.time + fireRate;
          StartCoroutine(ShootWithDelay());
    }

    IEnumerator ShootWithDelay()
    {
      yield return new WaitForSeconds(1.5f);
      Shoot();
    }

    void Shoot()
    {
        if (muzzleFlash != null)
        {
            GameObject flash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
            Destroy(flash, 2f);
        }

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        BulletProjectile bullet = bulletObj.GetComponent<BulletProjectile>();
        bullet.damage = bulletDamage;
        bullet.speed = bulletSpeed;
    }
}
