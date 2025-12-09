using UnityEngine;
using System.Collections;

public class SlowTimeEverySec : MonoBehaviour
{
    public float Value = 0.2f;
    public Animator anim;

    void Start()
    {
        anim.speed = 1f;
    }
    void Update()
    
    {
        anim.speed -= Value * Time.deltaTime;
    }
}
