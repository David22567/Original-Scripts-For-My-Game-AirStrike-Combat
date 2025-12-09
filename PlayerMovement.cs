using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;

    [Header("Animations")]
    public Animator animator;
    public float animSmoothTime = 0.1f;

    [Header("Movement Bounds")]
    public BoxCollider movementBounds;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 animBlendVelocity;

    private Vector3 minBounds;
    private Vector3 maxBounds;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {}

    void Update()
    {
        Bounds b = movementBounds.bounds;
        minBounds = b.min;
        maxBounds = b.max;
        MovePlayer();
        ClampInsideBounds();
        UpdateAnimations();
    }

    void MovePlayer()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    void ClampInsideBounds()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);

        transform.position = pos;
    }

    void UpdateAnimations()
    {
        Vector2 smoothAnim = Vector2.SmoothDamp(
            new Vector3(animator.GetFloat("MoveX"), animator.GetFloat("MoveY")),
            moveInput,
            ref animBlendVelocity,
            animSmoothTime
        );

        animator.SetFloat("MoveX", smoothAnim.x);
        animator.SetFloat("MoveY", smoothAnim.y);
    }
}
