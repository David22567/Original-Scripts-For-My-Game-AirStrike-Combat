using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject smoke;
    public GameObject fire;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Death & Respawn")]
    public GameObject destroyedHelicopterPrefab;
    public Transform respawnPoint;
    public float respawnDelay = 3f;
    public Transform helicopterModel;

    private bool isDead = false;


    void Start()
    {
        smoke.SetActive(false);
        fire.SetActive(false);
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0f)
            Die();      
        
        if (currentHealth <= 50f)
            smoke.SetActive(true);
        
        if (currentHealth <= 20f)
            fire.SetActive(true);
    }

    void Die()
    {
        if (isDead) return;

        Debug.Log("Player Dead!");
        isDead = true;

        gameObject.SetActive(false);
        helicopterModel.gameObject.SetActive(false);

        if (destroyedHelicopterPrefab != null)
        {
            GameObject wreck = Instantiate(destroyedHelicopterPrefab,transform.position,transform.rotation);

            Destroy(wreck, respawnDelay + 10f);
        }

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
{
    Debug.Log("Player Respawn!");

    HelicopterShooting gun = GetComponent<HelicopterShooting>();
    if (gun != null)
        gun.ResetGunState();

    helicopterModel.localRotation = Quaternion.identity;
    helicopterModel.gameObject.SetActive(true);

    currentHealth = maxHealth;
    if (healthSlider != null)
        healthSlider.value = maxHealth;

    if (respawnPoint != null)
    {
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
    }

    gameObject.SetActive(true);
    smoke.SetActive(false);
    fire.SetActive(false);

    isDead = false;
}

}
