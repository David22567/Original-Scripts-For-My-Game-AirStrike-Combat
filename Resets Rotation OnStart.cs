using UnityEngine;

public class ResetsRotationOnStart : MonoBehaviour
{
    public void Start()
    {
     transform.rotation = Quaternion.Euler(0, 0, 0);
    }

}
