using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillBar;
    public Canvas canvas;

    private Transform target;

    public void Setup(Transform followTarget)
    {
        target = followTarget;
        canvas.enabled = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + Vector3.up * 2f;

        transform.LookAt(Camera.main.transform);
    }

    public void UpdateHealth(float percent)
    {
        fillBar.fillAmount = percent;
    }

    public void Show()  => canvas.enabled = true;
    public void Hide()  => canvas.enabled = false;
}
