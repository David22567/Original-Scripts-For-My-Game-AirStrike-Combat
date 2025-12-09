using UnityEngine;

public class LocalScaleOnStart : MonoBehaviour
{
    public Vector3 scl;

    public void Start()
    {
     transform.localScale = scl;
    }

}
