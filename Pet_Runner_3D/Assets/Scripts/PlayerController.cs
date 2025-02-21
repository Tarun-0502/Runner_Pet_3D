using UnityEngine;

[System.Serializable]
public enum Lane { Left, Mid, Right }
public enum HitX { Left, Mid, Right, None }
public enum HitY { Up, Mid, Down, None }
public enum HitZ { Forward, Mid, Backward, None }
public enum Mode { Run, Jet, HoverBoard, PowerJump, Over, Jump, Slide }
public enum Status { Playing, Pause, GameOver }

public class PlayerController : MonoBehaviour
{
    #region HideInInspector

    [SerializeField, HideInInspector] private float targetX, targetY;
    [SerializeField, HideInInspector] private float ColHeight, ColCenterY;
    [SerializeField, HideInInspector] private bool Injump, Inslide;
    [SerializeField, HideInInspector] bool SwipeLeft, SwipeRight, SwipeUp, SwipeDown;
    [SerializeField, HideInInspector] float SlideCounter;
    [SerializeField, HideInInspector] private CharacterController characterController;
    [SerializeField, HideInInspector] private Animator Anim;

    #endregion

    #region References

    [SerializeField] private float laneDistance = 3.0f;
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float forwardSpeed = 5.0f;
    [SerializeField] private float speedIncreaseRate = 0.1f;

    #endregion

    #region ENUMS

    [SerializeField] private Lane m_Side = Lane.Mid;
    [SerializeField] private HitX m_HitX = HitX.None;
    [SerializeField] private HitY m_HitY = HitY.None;
    [SerializeField] private HitZ m_HitZ = HitZ.None;
    [SerializeField] private Mode playerMode = Mode.Run; // Default mode is running
    [SerializeField] private Status gameStatus = Status.Playing; // Default game status is playing

    #endregion

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Anim = transform.GetChild(0).GetComponent<Animator>();
        ColCenterY = characterController.center.y;
        ColHeight = characterController.height;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        targetX = transform.position.x;
    }

    private void Update()
    {
        if (gameStatus != Status.Playing) return; // Stop updating if game is paused or over

        // Detect input for lane changes, jumping, and sliding
        SwipeLeft = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        SwipeRight = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        SwipeUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        SwipeDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

        // Gradually increase forward speed over time
        forwardSpeed += speedIncreaseRate * Time.deltaTime;

        // Adjust player's position based on input
        AdjustPlayerPosition();
    }

    void AdjustPlayerPosition()
    {
        // Handle left and right lane shifts, preventing movement during slide
        if (SwipeLeft && !Inslide)
        {
            if (m_Side == Lane.Mid)
            {
                m_Side = Lane.Left;
                targetX = -laneDistance;
            }
            else if (m_Side == Lane.Right)
            {
                m_Side = Lane.Mid;
                targetX = 0;
            }
        }
        else if (SwipeRight && !Inslide)
        {
            if (m_Side == Lane.Mid)
            {
                m_Side = Lane.Right;
                targetX = laneDistance;
            }
            else if (m_Side == Lane.Left)
            {
                m_Side = Lane.Mid;
                targetX = 0;
            }
        }

        // Move the player smoothly towards the target position
        Vector3 moveDirection = new Vector3(Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * moveSpeed) - transform.position.x, targetY * Time.deltaTime, forwardSpeed * Time.deltaTime);
        characterController.Move(moveDirection);

        // Handle jumping and sliding
        Jump();
        Slide();
    }

    void Jump()
    {
        if (characterController.isGrounded)
        {
            if (playerMode == Mode.Jump)
            {
                playerMode = Mode.Run; // Reset mode to running when landing
            }
            if (SwipeUp)
            {
                targetY = jumpForce;
                Injump = true;
                playerMode = Mode.Jump; // Change mode to PowerJump
                Anim.SetTrigger("jump");
            }
        }
        else
        {
            targetY -= jumpForce * 2 * Time.deltaTime;
        }
    }

    void Slide()
    {
        SlideCounter -= Time.deltaTime;
        if (SlideCounter <= 0f)
        {
            SlideCounter = 0f;
            characterController.center = new Vector3(0, ColCenterY, 0);
            characterController.height = ColHeight;
            Inslide = false;
            if (playerMode == Mode.HoverBoard) return; // If using hoverboard, stay in that mode
            playerMode = Mode.Run; // Reset mode to running after sliding
        }

        if (SwipeDown)
        {
            SlideCounter = 0.5f;
            targetY -= 10f;
            Anim.CrossFadeInFixedTime("Slide", 0.1f);
            characterController.center = new Vector3(0, ColCenterY / 2f, 0);
            characterController.height = ColHeight / 2f;
            Inslide = true;
            Injump = false;
            playerMode = Mode.Slide;
        }
    }

    #region Collision Detection 

    //public void OnCharacterColliderHit(Collider col)
    //{
    //    m_HitX = GetHitX(col);
    //    m_HitY = GetHitY(col);
    //    m_HitZ = GetHitZ(col);
    //}

    //public void SetGameStatus(Status newStatus)
    //{
    //    gameStatus = newStatus;
    //}

    //public HitX GetHitX(Collider col)
    //{
    //    Bounds char_bounds = characterController.bounds;
    //    Bounds colBounds = col.bounds;
    //    float minX = Mathf.Max(colBounds.min.x, char_bounds.min.x);
    //    float maxX = Mathf.Min(colBounds.max.x, char_bounds.max.x);
    //    float average = (minX + maxX) / 2f - colBounds.min.x;
    //    return average > colBounds.size.x - 0.33f ? HitX.Right : average < 0.33f ? HitX.Left : HitX.Mid;
    //}

    //public HitY GetHitY(Collider col)
    //{
    //    Bounds char_bounds = characterController.bounds;
    //    Bounds colBounds = col.bounds;
    //    float minY = Mathf.Max(colBounds.min.y, char_bounds.min.y);
    //    float maxY = Mathf.Min(colBounds.max.y, char_bounds.max.y);
    //    float average = ((minY + maxY) / 2f - colBounds.min.y) / char_bounds.size.y;
    //    return average < 0.33f ? HitY.Down : average < 0.66f ? HitY.Mid : HitY.Up;
    //}

    //public HitZ GetHitZ(Collider col)
    //{
    //    Bounds char_bounds = characterController.bounds;
    //    Bounds colBounds = col.bounds;
    //    float minZ = Mathf.Max(colBounds.min.z, char_bounds.min.z);
    //    float maxZ = Mathf.Min(colBounds.max.z, char_bounds.max.z);
    //    float average = ((minZ + maxZ) / 2f - colBounds.min.z) / char_bounds.size.z;
    //    return average > colBounds.size.z - 0.33f ? HitZ.Backward : average < 0.33f ? HitZ.Mid : HitZ.Forward;
    //}

    #endregion

    // Collision Handling
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Obstacle"))
        //{
        //    HandleCollision(other);
        //}
    }

    public void HandleCollision(Collider obstacle)
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
            Debug.LogError("Player stumbled but continues.");
        }
    }

    void TriggerGameOver()
    {
        //isGameOver = true;
        //animator.Play("Death");
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
