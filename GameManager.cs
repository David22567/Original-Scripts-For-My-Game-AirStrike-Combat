using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalScore = 0;
    public int destroyedCount = 0;

    public TMP_Text scoreText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddPoints(int p)
    {
        totalScore += p;
        UpdateUI();
    }

    public void RegisterDestroyedObject()
    {
        destroyedCount++;
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + totalScore;
    }
}
