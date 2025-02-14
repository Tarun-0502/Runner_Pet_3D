using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance; // Singleton instance
    [SerializeField] private float speed = 10.0f; // Player speed

    private Rigidbody playerRB; // Rigidbody for physics-based movement
    private CapsuleCollider playerCollider; // Player collider component
    public Vector3 currentposition = Vector3.zero; // Current player position
    public Animator animator; // Animator for character animations

    public float currentHeight = 1.0f; // Player height for jumps and crouches
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Power-up timers
    public float jetTime = 15.0f;
    public float hoverBoardTime = 15.0f;
    public float powerShoesTime = 15.0f;
    public float MagnetTime = 15.0f;
    public float shiledTime = 5.0f;
    public float jumpForce = 7.0f;

    public enum lane { left, middle, right } // Player lanes
    public enum mode { run, jet, hoverBoard, powerJump, over } // Game modes
    public enum status { Playing, Pause, GameOver } // Game status

    public lane currentLane; // Current lane
    public mode gameMode; // Current mode
    private mode previousGameMode; // For restoring mode
    public status gameStatus; // Game status
    public bool isTv = false; // TV mode detection

    private float currentSpeed; // Current speed

    private void Awake() { Instance = this; }

    private void Start()
    {
        LoadPlayerPrefs();
        playerCollider = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        currentLane = lane.middle;
        gameMode = mode.run;
        currentSpeed = speed;
        currentposition = transform.position;
    }

    private void LoadPlayerPrefs() { /* Load power-up timers from preferences */ }

    private void Update()
    {
        if (gameStatus != status.Playing) return;
        currentposition = transform.position + transform.forward * currentSpeed * Time.deltaTime;
        currentposition.y = Mathf.Clamp(currentposition.y, 0.0f, 10.0f);

        HandleInputs();
        AdjustPositionAccordingToLane();
        transform.position = currentposition;
    }

    private void HandleInputs()
    {
        if (isTv)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) ShiftLane(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) ShiftLane(1);
            if (Input.GetKeyDown(KeyCode.UpArrow) && IsGrounded()) Jump();
            if (Input.GetKeyDown(KeyCode.DownArrow) && IsGrounded()) Slide();
        }
        else
        {
            HandleTouchInputs();
        }
    }

    private void HandleTouchInputs() { /* Handle swipe inputs for mobile */ }

    private void ShiftLane(int direction)
    {
        if (direction == -1 && currentLane != lane.left) currentLane--;
        else if (direction == 1 && currentLane != lane.right) currentLane++;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            playerRB.velocity = new Vector3(playerRB.velocity.x, jumpForce, playerRB.velocity.z);
            animator.SetTrigger("jump");
        }
    }

    private void Slide()
    {
        animator.SetBool("isSliding", true);
        StartCoroutine(EndSlide());
    }

    private IEnumerator EndSlide()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("isSliding", false);
    }

    private void AdjustPositionAccordingToLane()
    {
        float targetX = 0.0f;
        if (currentLane == lane.left) targetX = -2.5f;
        else if (currentLane == lane.right) targetX = 2.5f;
        currentposition.x = Mathf.Lerp(currentposition.x, targetX, Time.deltaTime * 10);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.17f, groundLayer);
    }

}
