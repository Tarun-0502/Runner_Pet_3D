using Cinemachine;
using UnityEngine;

 #region USER-DEFINED-CLASS

    [System.Serializable]
    public enum Lane { Left, Mid, Right }
    public enum HitX { Left, Mid, Right, None }
    public enum HitY { Up, Mid, Down, None }
    public enum HitZ { Forward, Mid, Backward, None }
    public enum Mode { Run, Jet, HoverBoard, PowerJump, Over, Jump, Slide }
    public enum Status { Playing, Pause, GameOver }

#endregion

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
    [SerializeField] internal Collider playerCollider;
    [SerializeField] Transform SkyBox_;
    [SerializeField] CollectablesManager collectablesManager_;
    [SerializeField] CinemachineVirtualCamera Cam_Vir;

    #endregion

    #region ENUMS

    [SerializeField] private Lane m_Lane = Lane.Mid;
    [SerializeField] private HitX m_HitX = HitX.None;
    [SerializeField] private HitY m_HitY = HitY.None;
    [SerializeField] private HitZ m_HitZ = HitZ.None;
    [SerializeField] private Mode playerMode = Mode.Run;
    [SerializeField] private Status gameStatus = Status.Playing;

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
        if (gameStatus != Status.Playing) return;

        SwipeLeft = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        SwipeRight = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        SwipeUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        SwipeDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

        forwardSpeed += speedIncreaseRate * Time.deltaTime;
        AdjustPlayerPosition();
    }

    void AdjustPlayerPosition()
    {
        if (SwipeLeft && !Inslide)
        {
            if (m_Lane == Lane.Mid) { m_Lane = Lane.Left; targetX = -laneDistance; }
            else if (m_Lane == Lane.Right) { m_Lane = Lane.Mid; targetX = 0; }
        }
        else if (SwipeRight && !Inslide)
        {
            if (m_Lane == Lane.Mid) { m_Lane = Lane.Right; targetX = laneDistance; }
            else if (m_Lane == Lane.Left) { m_Lane = Lane.Mid; targetX = 0; }
        }

        Vector3 moveDirection = new Vector3(Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * moveSpeed) - transform.position.x, targetY * Time.deltaTime, forwardSpeed * Time.deltaTime);
        characterController.Move(moveDirection);
        SkyBox_.transform.position = new Vector3(SkyBox_.position.x,SkyBox_.position.y, transform.position.z);

        Jump();
        Slide();
    }

    void Jump()
    {
        if (characterController.isGrounded)
        {
            if (playerMode == Mode.Jump) playerMode = Mode.Run;
            if (SwipeUp)
            {
                targetY = jumpForce;
                Injump = true;
                playerMode = Mode.Jump;
                Anim.CrossFadeInFixedTime("Jump", 0.1f);
            }
        }
        else targetY -= jumpForce * 2 * Time.deltaTime;
    }

    void Slide()
    {
        SlideCounter -= Time.deltaTime;
        if (SlideCounter <= 0f)
        {
            SlideCounter = 0f;
            AdjustCollider(false);
            Inslide = false;
            if (playerMode != Mode.HoverBoard) playerMode = Mode.Run;
        }

        if (SwipeDown)
        {
            SlideCounter = 0.5f;
            targetY -= 10f;
            Anim.CrossFadeInFixedTime("Slide", 0.1f);
            AdjustCollider(true);
            //Debug.LogError("Slide");
            Inslide = true;
            Injump = false;
            playerMode = Mode.Slide;
        }
    }

    void AdjustCollider(bool adjust)
    {
        if (adjust)
        {
            characterController.center = new Vector3(0, ColCenterY / 2f, 0.0f);
            characterController.height = ColHeight / 2f;
        }
        else
        {
            characterController.center = new Vector3(0, ColCenterY, 0.0f);
            characterController.height = ColHeight;
        }
    }

    public void HandleCollision(Collider obstacle)
    {
        m_HitX = GetHitX(obstacle);
        m_HitY = GetHitY(obstacle);
        m_HitZ = GetHitZ(obstacle);

        if (characterController.isGrounded)
        {
            if (m_HitX == HitX.Mid && m_HitY == HitY.Mid)
            {
                TriggerGameOver();
            }
            else
            {
                if (m_HitX == HitX.Left || m_HitX == HitX.Right)
                {
                    Debug.LogError("Hitted on X");
                }
            }
        }
    }

    void TriggerGameOver()
    {
        Anim.CrossFadeInFixedTime("Dead", 0.1f);
        gameStatus = Status.GameOver;
        //Debug.LogError("Game Over!");
    }

    //private HitX GetHitX(Collider col)
    //{
    //    float relativeX = transform.position.x - col.transform.position.x;
    //    return relativeX < -0.5f ? HitX.Left : relativeX > 0.5f ? HitX.Right : HitX.Mid;
    //}

    //private HitY GetHitY(Collider col)
    //{
    //    float relativeY = transform.position.y - col.transform.position.y;
    //    return relativeY < -0.5f ? HitY.Down : relativeY > 0.5f ? HitY.Up : HitY.Mid;
    //}

    //private HitZ GetHitZ(Collider col)
    //{
    //    float relativeZ = transform.position.z - col.transform.position.z;
    //    return relativeZ < -0.5f ? HitZ.Forward : relativeZ > 0.5f ? HitZ.Backward : HitZ.Mid;
    //}

    private HitX GetHitX(Collider col)
    {
        Bounds colBounds = col.bounds;
        Bounds charBounds = characterController.bounds;
        float minX = Mathf.Max(colBounds.min.x, charBounds.min.x);
        float maxX = Mathf.Min(colBounds.max.x, charBounds.max.x);
        float average = (minX + maxX) / 2f - colBounds.min.x;
        return average > colBounds.size.x - 0.33f ? HitX.Right : average < 0.33f ? HitX.Left : HitX.Mid;
    }

    private HitY GetHitY(Collider col)
    {
        Bounds colBounds = col.bounds;
        Bounds charBounds = characterController.bounds;
        float minY = Mathf.Max(colBounds.min.y, charBounds.min.y);
        float maxY = Mathf.Min(colBounds.max.y, charBounds.max.y);
        float average = ((minY + maxY) / 2f - colBounds.min.y) / colBounds.size.y;
        return average < 0.33f ? HitY.Down : average < 0.66f ? HitY.Mid : HitY.Up;
    }

    private HitZ GetHitZ(Collider col)
    {
        Bounds colBounds = col.bounds;
        Bounds charBounds = characterController.bounds;
        float minZ = Mathf.Max(colBounds.min.z, charBounds.min.z);
        float maxZ = Mathf.Min(colBounds.max.z, charBounds.max.z);
        float average = ((minZ + maxZ) / 2f - colBounds.min.z) / colBounds.size.z;
        return average > colBounds.size.z - 0.33f ? HitZ.Backward : average < 0.33f ? HitZ.Forward : HitZ.Mid;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Coin")
        {
            collectablesManager_.Coins++;
            other.gameObject.SetActive(false);
        }
    }

}
