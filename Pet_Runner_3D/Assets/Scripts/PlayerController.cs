using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour // Changed class name from RunnerController to PlayerController
{
    private CharacterController controller; // Player character controller
    public Vector3 moveVector; // Movement vector for the player

    public float baseSpeed = 5.0f; // Base speed of the player
    public float laneDistance = 3.0f; // Distance between lanes
    private int desiredLane = 1; // Current lane (0=Left, 1=Middle, 2=Right)

    private float jumpForce = 8.0f; // Jump force value
    private float gravity = 12.0f; // Gravity value
    private float verticalVelocity; // Current vertical velocity

    private bool isSliding = false; // Flag for sliding state

    // Power-up flags
    private bool isMagnetActive = false;
    private bool isJetActive = false;
    private bool isHoverboardActive = false;

    private float jetHeight = 5.0f; // Height when jet is active

    void Start()
    {
        controller = GetComponent<CharacterController>(); // Initialize character controller
    }

    void Update()
    {
        moveVector = Vector3.zero; // Reset movement vector
        baseSpeed += 0.1f * Time.deltaTime; // Gradually increase speed

        if (isJetActive)
        {
            verticalVelocity = 0;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, jetHeight, Time.deltaTime * 2), transform.position.z);
        }
        else
        {
            if (controller.isGrounded)
            {
                verticalVelocity = -0.1f;
                if (Input.GetKeyDown(KeyCode.UpArrow)) verticalVelocity = jumpForce;
            }
            else verticalVelocity -= gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSliding) StartCoroutine(Slide());

        HandleLaneSwitching();

        moveVector.y = verticalVelocity;
        moveVector.z = baseSpeed;

        controller.Move(moveVector * Time.deltaTime);
        HandlePowerUps();
    }

    private void HandleLaneSwitching()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) desiredLane--;
        if (Input.GetKeyDown(KeyCode.RightArrow)) desiredLane++;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0) targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2) targetPosition += Vector3.right * laneDistance;

        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        controller.height = 1.0f;
        yield return new WaitForSeconds(1.0f);
        controller.height = 2.0f;
        isSliding = false;
    }

    private void HandlePowerUps()
    {
        if (Input.GetKeyDown(KeyCode.M)) ActivateMagnet();
        if (Input.GetKeyDown(KeyCode.J)) ActivateJet();
        if (Input.GetKeyDown(KeyCode.H)) ActivateHoverboard();
    }

    private void ActivateMagnet() { isMagnetActive = true; StartCoroutine(DeactivatePowerUp(() => isMagnetActive = false, 10f)); }
    private void ActivateJet() { isJetActive = true; StartCoroutine(DeactivatePowerUp(() => isJetActive = false, 10f)); }
    private void ActivateHoverboard() { isHoverboardActive = true; StartCoroutine(DeactivatePowerUp(() => isHoverboardActive = false, 10f)); }

    private IEnumerator DeactivatePowerUp(System.Action deactivate, float time)
    {
        yield return new WaitForSeconds(time);
        deactivate();
    }
}
