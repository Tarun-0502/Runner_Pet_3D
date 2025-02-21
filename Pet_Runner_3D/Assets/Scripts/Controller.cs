using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    public float speed = 10f;
    public float jumpHeight = 10f;
    public float gravity = 20f;
    public float laneDistance = 3.0f;
    private int laneIndex = 0;

    private float verticalSpeed;
    private bool isRolling = false;
    private bool isGameOver = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        if (isGameOver) return; // Stop movement if game is over

        Vector3 move = Vector3.forward * speed * Time.deltaTime;

        //// Apply Gravity
        //if (characterController.isGrounded)
        //{
        //    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        //        Jump();
        //    else
        //        verticalSpeed = 0;
        //}
        //else
        //{
        //    verticalSpeed -= gravity * Time.deltaTime;
        //}

        // Lane Movement
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeLane(-1);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            ChangeLane(1);

        // Roll/Duck
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Roll();

        // Apply movement
        move.y = verticalSpeed * Time.deltaTime;
        move.x = Mathf.Lerp(transform.position.x, laneIndex * laneDistance, Time.deltaTime * 10);
        characterController.Move(move);
    }

    void Jump()
    {
        if (!isRolling)
        {
            verticalSpeed = Mathf.Sqrt(2 * jumpHeight * gravity);
            animator.Play("Jump");
        }
    }

    void Roll()
    {
        if (!isRolling)
        {
            isRolling = true;
            animator.Play("Slide");
            //Invoke("EndRoll", 1f);
        }
    }

    void EndRoll()
    {
        isRolling = false;
    }

    void ChangeLane(int direction)
    {
        int targetLane = laneIndex + direction;
        if (targetLane >= -1 && targetLane <= 1)
            laneIndex = targetLane;
    }

    // Collision Handling
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleCollision(other);
        }
    }

    void HandleCollision(Collider obstacle)
    {
        // Analyze hit position
        int[] hitResult = AnalyzeHit(obstacle);
        HitLocation xHit = (HitLocation)hitResult[0];
        HitLocation yHit = (HitLocation)hitResult[1];

        if (xHit == HitLocation.XMiddle && yHit == HitLocation.YMiddle)
        {
            TriggerGameOver();
        }
        else
        {
            //animator.Play("Stumble");
            Debug.Log("Player stumbled but continues.");
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        animator.Play("Death");
        Debug.Log("Game Over!");
    }

    // Analyze Hit Function
    private int[] AnalyzeHit(Collider obstacle)
    {
        int[] results = new int[2];
        Bounds playerBounds = characterController.bounds;
        Bounds obstacleBounds = obstacle.bounds;

        // Determine horizontal hit (left, middle, right)
        float centerX = (obstacleBounds.min.x + obstacleBounds.max.x) / 2;
        if (transform.position.x < centerX - 0.5f)
            results[0] = (int)HitLocation.Left;
        else if (transform.position.x > centerX + 0.5f)
            results[0] = (int)HitLocation.Right;
        else
            results[0] = (int)HitLocation.XMiddle;

        // Determine vertical hit (lower, middle, upper)
        float centerY = (obstacleBounds.min.y + obstacleBounds.max.y) / 2;
        if (transform.position.y < centerY - 0.5f)
            results[1] = (int)HitLocation.Lower;
        else if (transform.position.y > centerY + 0.5f)
            results[1] = (int)HitLocation.Upper;
        else
            results[1] = (int)HitLocation.YMiddle;

        return results;
    }

    enum HitLocation
    {
        Left, XMiddle, Right,
        Lower, YMiddle, Upper
    }
}
