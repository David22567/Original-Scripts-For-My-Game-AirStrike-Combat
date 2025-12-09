using UnityEngine;
using UnityEngine.UI;

public class BreakableObjectHealth : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;

    public GameObject brokenPrefab;
    public GameObject explosionEffect;

    [Header("Points")]
    public int points = 50; 

    [Header("UI")]
    public Canvas healthCanvas;
    public Slider healthSlider;

    private Camera cam;

    private float hideTimer = 0f;
    private float hideDelay = 3f;

    void Start()
    {
        currentHealth = maxHealth;
        cam = Camera.main;

        if (healthCanvas == null)
            healthCanvas = GetComponentInChildren<Canvas>();

        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        if (healthCanvas != null)
            healthCanvas.enabled = false;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    void LateUpdate()
    {
        if (healthCanvas == null || cam == null)
            return;

        healthCanvas.transform.LookAt(cam.transform);
        healthCanvas.transform.Rotate(0, 180f, 0);

        Vector3 viewPos = cam.WorldToViewportPoint(healthCanvas.transform.position);

        bool isVisibleOnScreen =
            viewPos.z > 0 &&
            viewPos.x > 0 && viewPos.x < 1 &&
            viewPos.y > 0 && viewPos.y < 1;

        if (isVisibleOnScreen)
        {
            if (hideTimer > 0)
            {
                hideTimer -= Time.deltaTime;

                if (hideTimer <= 0)
                    healthCanvas.enabled = false;
            }
        }
        else
        {
            healthCanvas.enabled = false;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthCanvas != null)
            healthCanvas.enabled = true;

        hideTimer = hideDelay;

        if (currentHealth <= 0)
            BreakObject();
    }

    public void BreakObject()
    {
        if (brokenPrefab != null)
            Instantiate(brokenPrefab, transform.position, transform.rotation);

        if (explosionEffect != null)
        {
            GameObject fx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(fx, 20f);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.AddPoints(points);

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterDestroyedObject();

        Destroy(gameObject);
    }
}
