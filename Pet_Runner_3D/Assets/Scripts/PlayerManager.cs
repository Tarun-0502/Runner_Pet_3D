using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float laneDistance = 3f;

    private Rigidbody playerRB;
    private CapsuleCollider playerCollider;
    public Vector3 currentposition = Vector3.zero;
    public Animator animator;

    public float currentHeight = 1.0f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public float jetTime = 15.0f;
    public float hoverBoardTime = 15.0f;
    public float powerShoesTime = 15.0f;
    public float MagnetTime = 15.0f;
    public float shiledTime = 5.0f;
    public float baseJumpForce = 7.0f;

    public enum lane { left, middle, right }
    public enum mode { run, jet, hoverBoard, powerJump, over }
    public enum status { Playing, Pause, GameOver }

    public lane currentLane;
    public mode gameMode;
    private mode previousGameMode;
    public status gameStatus;
    public bool isTv = false;

    public float currentSpeed;
    private float elapsedTime = 0f;
    [SerializeField] private float speedIncreaseRate = 0.5f; // Increase speed every second

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

    private void LoadPlayerPrefs() { }

    private void Update()
    {
        if (gameStatus != status.Playing) return;

        elapsedTime += Time.deltaTime;
        currentSpeed = speed + (elapsedTime * speedIncreaseRate); // Increase speed over time

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

    private void HandleTouchInputs() { }

    private void ShiftLane(int direction)
    {
        if (direction == -1 && currentLane != lane.left) currentLane--;
        else if (direction == 1 && currentLane != lane.right) currentLane++;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            float jumpForce = baseJumpForce + (currentSpeed / 10f); // Increase jump force with speed
            float jumpDuration = Mathf.Clamp(1f / currentSpeed, 0.3f, 1f); // Adjust jump duration based on speed

            playerRB.velocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("jump");

            StartCoroutine(AdjustGravityDuringJump(jumpDuration));
        }
    }

    private IEnumerator AdjustGravityDuringJump(float jumpDuration)
    {
        Physics.gravity = new Vector3(0, -9.81f / jumpDuration, 0);
        yield return new WaitForSeconds(jumpDuration);
        Physics.gravity = new Vector3(0, -9.81f, 0);
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
        if (currentLane == lane.left) targetX = -laneDistance;
        else if (currentLane == lane.right) targetX = laneDistance;
        currentposition.x = Mathf.Lerp(currentposition.x, targetX, Time.deltaTime * 10);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.17f, groundLayer);
    }
}
