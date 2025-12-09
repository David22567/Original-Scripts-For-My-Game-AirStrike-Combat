using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Weapon
{
    public string weaponName;

    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public float spread = 5f;
    public int damage = 10;
    public Transform firePoint;
    public GameObject hitEffectDefault;
    public GameObject hitEffectGround;

    [Header("Bullet Settings")]
    public GameObject bulletTrail;
    public float bulletSpeed = 100f;
    public float maxDistance = 200f;

    [Header("Heat Settings")]
    public float heatPerShot = 6f;

    [Header("Effects")]
    public GameObject muzzleFlash;
    public AudioClip shootSound;
    public float soundVolume = 1f;
}

public class HelicopterShooting : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public AudioSource audioSource;

    [Header("Impact Effects")]
    public LayerMask groundLayer;

    [Header("Heat UI")]
    public Slider heatSlider;
    public GameObject GunheatUI;
    public float coolRate = 10f;
    public float overheatLimit = 100f;
    public float overheatCooldown = 3f;

    [Header("Weapons")]
    public Weapon[] weapons;
    private Weapon currentWeapon;
    private int currentWeaponIndex = 0;

    private float currentHeat = 0f;
    private bool isOverheated = false;

    private PlayerControls controls;
    private Vector2 pointerScreenPos;
    private bool isFiring = false;
    private float nextFireTime = 0f;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Point.performed += ctx =>
        {
            pointerScreenPos = ctx.ReadValue<Vector2>();
        };

        controls.Player.Attack.started += ctx => isFiring = true;
        controls.Player.Attack.canceled += ctx => isFiring = false;

        controls.Player.SwitchWeapon.performed += ctx =>
        {
            int dir = (int)Mathf.Sign(ctx.ReadValue<float>());
            SwitchWeapon(currentWeaponIndex + dir);
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        if (cam == null) cam = Camera.main;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        GunheatUI.SetActive(false);
        currentWeapon = weapons[0];

        if (heatSlider)
        {
            heatSlider.minValue = 0f;
            heatSlider.maxValue = overheatLimit;
            heatSlider.value = 0f;
        }
    }

    void Update()
    {
        HandleHeatSystem();

        if (!isOverheated && isFiring && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + currentWeapon.fireRate;
            Shoot();
            AddHeat(currentWeapon.heatPerShot);
        }
    }

    public void SwitchWeapon(int index)
    {
        if (weapons.Length == 0) return;

        index = (index + weapons.Length) % weapons.Length;
        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];

        Debug.Log("Switched to: " + currentWeapon.weaponName);
    }

    void AddHeat(float amount)
    {
        currentHeat += amount;
        currentHeat = Mathf.Clamp(currentHeat, 0, overheatLimit);

        if (heatSlider) heatSlider.value = currentHeat;

        if (currentHeat >= overheatLimit)
            StartCoroutine(OverheatCooldownRoutine());
    }

    void HandleHeatSystem()
    {
        if (!isFiring && !isOverheated)
        {
            currentHeat -= coolRate * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0, overheatLimit);

            if (heatSlider) heatSlider.value = currentHeat;
        }
    }

    IEnumerator OverheatCooldownRoutine()
    {
        isOverheated = true;
        GunheatUI.SetActive(true);

        Debug.Log("GUN OVERHEATED!");
        yield return new WaitForSeconds(3f);

        GunheatUI.SetActive(false);

        float startHeat = currentHeat;
        float t = 0f;

        while (t < overheatCooldown)
        {
            t += Time.deltaTime;
            currentHeat = Mathf.Lerp(startHeat, 0f, t / overheatCooldown);
            if (heatSlider) heatSlider.value = currentHeat;
            yield return null;
        }

        currentHeat = 0f;
        heatSlider.value = 0f;
        isOverheated = false;
    }

    void Shoot()
    {
        PlayMuzzleFlash();
        PlayShootSound();

        Ray pointerRay = cam.ScreenPointToRay(pointerScreenPos);

        Vector3 targetPoint;
        if (Physics.Raycast(pointerRay, out RaycastHit mouseHit, 1000f))
            targetPoint = mouseHit.point;
        else
            targetPoint = pointerRay.GetPoint(200f);

        Vector3 shootDir = (targetPoint - currentWeapon.firePoint.position).normalized;
        shootDir = AddSpread(shootDir);

        Vector3 endPoint = currentWeapon.firePoint.position + shootDir * currentWeapon.maxDistance;

        bool hitSomething = false;
        RaycastHit hit;

        if (Physics.Raycast(currentWeapon.firePoint.position, shootDir, out hit, currentWeapon.maxDistance))
        {
            endPoint = hit.point;
            hitSomething = true;
        }

        GameObject trailObj = Instantiate(currentWeapon.bulletTrail, currentWeapon.firePoint.position, Quaternion.identity);
        TrailRenderer trail = trailObj.GetComponent<TrailRenderer>();

        StartCoroutine(SpawnTrail(trail, currentWeapon.firePoint.position, endPoint, hitSomething, hit));
    }

    void PlayMuzzleFlash()
    {
        if (currentWeapon.muzzleFlash != null)
        {
            GameObject flash = Instantiate(currentWeapon.muzzleFlash, currentWeapon.firePoint.position, currentWeapon.firePoint.rotation);
            flash.transform.SetParent(currentWeapon.firePoint);
            Destroy(flash, 0.3f);
        }
    }

    public void ResetGunState()
     {
      StopAllCoroutines();
      isFiring = false;
      isOverheated = false;
      nextFireTime = 0f;
      currentHeat = 0f;
      if (heatSlider) heatSlider.value = 0f;
      GunheatUI.SetActive(false);
     }

    void PlayShootSound()
    {
        if (currentWeapon.shootSound != null)
        {
            audioSource.PlayOneShot(currentWeapon.shootSound, currentWeapon.soundVolume);
        }
    }

    Vector3 AddSpread(Vector3 dir)
    {
        float x = Random.Range(-currentWeapon.spread, currentWeapon.spread);
        float y = Random.Range(-currentWeapon.spread, currentWeapon.spread);
        return Quaternion.Euler(x, y, 0) * dir;
    }

    IEnumerator SpawnTrail(
        TrailRenderer trail,
        Vector3 start,
        Vector3 end,
        bool hitSomething,
        RaycastHit hit)
    {
        float distance = Vector3.Distance(start, end);
        float travelTime = distance / currentWeapon.bulletSpeed;

        float elapsed = 0f;

        while (elapsed < travelTime)
        {
            trail.transform.position = Vector3.Lerp(start, end, elapsed / travelTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        trail.transform.position = end;

        if (hitSomething)
        {
            GameObject fxPrefab = currentWeapon.hitEffectDefault;

            if (((1 << hit.collider.gameObject.layer) & groundLayer) != 0)
                fxPrefab = currentWeapon.hitEffectGround;

            if (fxPrefab)
            {
                GameObject fx = Instantiate(fxPrefab, hit.point, Quaternion.identity);
                Destroy(fx, 2f);
            }

            BreakableObjectHealth hp = hit.collider.GetComponent<BreakableObjectHealth>();
            if (hp != null)
                hp.TakeDamage(currentWeapon.damage);

            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb)
            {
                Vector3 hitDir = (end - start).normalized;
                rb.AddForce(hitDir * 30f, ForceMode.Impulse);
            }
        }
    }
}
