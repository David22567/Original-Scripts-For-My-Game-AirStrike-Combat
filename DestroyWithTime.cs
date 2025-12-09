using UnityEngine;

public class DestroyWithTime : MonoBehaviour
{
    public float destroyAfter = 10f;

    void Start()
    {
        Destroy(gameObject, destroyAfter);
    }
}
