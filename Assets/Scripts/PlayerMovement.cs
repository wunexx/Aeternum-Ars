using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    //[SerializeField] private float accelerationSpeed = 10f;
    //[SerializeField] private float decelerationSpeed = 15f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkHeightOffset = 0.3f;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool drawGizmos = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private float walkSoundInterval = 0.4f;
    private bool wasGrounded;
    private float walkSoundTimer = 0f;


    bool canMove = true;


    private InputSystem_Actions inputActions;

    private CharacterController characterController;

    private float verticalVelocity;

    private bool isGrounded;
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    void Update()
    {
        isGrounded = Physics.CheckCapsule(
            groundCheck.position + new Vector3(0, checkHeightOffset, 0), 
            groundCheck.position - new Vector3(0, checkHeightOffset, 0), 
            checkRadius, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            if (landSound != null && audioSource != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(landSound);
            }
        }
        wasGrounded = isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -4f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (canMove && inputActions.Player.Jump.IsPressed() && isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        Vector3 horizontal = Vector3.zero;
        if (canMove)
        {
            Vector2 move = inputActions.Player.Move.ReadValue<Vector2>();
            Vector3 inputDir = new Vector3(move.x, 0f, move.y).normalized;

            if (inputDir.magnitude > 0.1f && walkSound != null && audioSource != null && isGrounded)
            {
                walkSoundTimer -= Time.deltaTime;
                if (walkSoundTimer <= 0f)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(walkSound);
                    walkSoundTimer = walkSoundInterval;
                }
            }

            horizontal = transform.TransformDirection(inputDir) * moveSpeed;
        }

        Vector3 velocity = horizontal;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (groundCheck == null || !drawGizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position + new Vector3(0, checkHeightOffset, 0), checkRadius);
        Gizmos.DrawWireSphere(groundCheck.position - new Vector3(0, checkHeightOffset, 0), checkRadius);
    }
#endif
}
