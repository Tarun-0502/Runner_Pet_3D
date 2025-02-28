using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public static PlayerMovements instance;

    [Header("Movement Settings")]
    public float speed = 10f;
    public float laneDistance = 2.5f; // Distance between lanes
    public float jumpForce = 15f;
    public bool isPlaying = false;

    [Header("References")]
    public Animator animator;
    public Rigidbody rb;

    private int currentLane = 1; // 0 = Left, 1 = Middle, 2 = Right
    private bool isJumping = false;
    private bool isSliding = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isPlaying) return;

        // Move Forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Handle Input
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(1);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(Slide());
        }
    }

    private void MoveLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane < 0 || targetLane > 2) return; // Stay within bounds

        currentLane = targetLane;
        Vector3 targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
        StartCoroutine(SmoothLaneChange(targetPosition));
    }

    private IEnumerator SmoothLaneChange(Vector3 targetPosition)
    {
        float duration = 0.2f;
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private void Jump()
    {
        if (isJumping) return;

        isJumping = true;
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        //animator.SetTrigger("Jump");
    }

    private IEnumerator Slide()
    {
        if (isSliding) yield break;

        isSliding = true;
        //animator.SetTrigger("Slide");

        // Reduce collider height
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        float originalHeight = collider.height;
        collider.height = originalHeight / 2;

        yield return new WaitForSeconds(0.8f);

        // Reset collider height
        collider.height = originalHeight;
        isSliding = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void Die()
    {
        isPlaying = false;
        //animator.SetTrigger("Die");
        Debug.Log("Game Over!");
        // Implement game over logic here
    }
}
