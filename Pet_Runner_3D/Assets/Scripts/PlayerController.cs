using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float laneDistance = 2.5f;
    public float jumpForce = 7.0f;

    private int desiredLane = 1; // 0:left, 1:middle, 2:right
    private Rigidbody rb;
    private Animator animator;
    public bool isGrounded;
    public Vector2 startTouchPosition, endTouchPosition;
    public bool isTv;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        //isTv = AndroidTV.IsAndroidOrFireTv();
    }

    void Update()
    {
        //if (PlayerManager.gameOver)
        //    return;

        Vector3 targetPosition = transform.position;

        if (isTv)
        {
            HandleTvInputs();
        }
        else
        {
            HandleMobileInputs();
        }

        MoveToLane(targetPosition);
    }

    private void HandleTvInputs()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) desiredLane = Mathf.Min(desiredLane + 1, 2);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) desiredLane = Mathf.Max(desiredLane - 1, 0);
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow)) Jump();
        if (isGrounded && Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(Slide());
    }

    private void HandleMobileInputs()
    {
        if (SwipeRight()) desiredLane = Mathf.Min(desiredLane + 1, 2);
        if (SwipeLeft()) desiredLane = Mathf.Max(desiredLane - 1, 0);
        if (isGrounded && SwipeUp()) Jump();
        if (isGrounded && SwipeDown()) StartCoroutine(Slide());
    }

    private void MoveToLane(Vector3 targetPosition)
    {
        if (desiredLane == 0) targetPosition.x = -laneDistance;
        else if (desiredLane == 1) targetPosition.x = 0;
        else if (desiredLane == 2) targetPosition.x = laneDistance;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        animator.SetTrigger("jump");
        isGrounded = false;
    }

    private IEnumerator Slide()
    {
        animator.SetBool("isSliding", true);
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("isSliding", false);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    //    //if (collision.gameObject.CompareTag("Obstacle")) PlayerManager.gameOver = true;
    //}

    private bool SwipeLeft() => DetectSwipe(Vector2.left);
    private bool SwipeRight() => DetectSwipe(Vector2.right);
    private bool SwipeUp() => DetectSwipe(Vector2.up);
    private bool SwipeDown() => DetectSwipe(Vector2.down);

    private bool DetectSwipe(Vector2 direction)
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) startTouchPosition = touch.position;
            if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                Vector2 swipeDelta = endTouchPosition - startTouchPosition;
                swipeDelta = new Vector2(swipeDelta.x / Screen.width, swipeDelta.y / Screen.height); // Normalize for portrait mode
                if (Vector2.Dot(swipeDelta.normalized, direction) > 0.8f) return true;
            }
        }
        return false;
    }
}
