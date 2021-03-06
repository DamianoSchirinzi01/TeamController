using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;    
    private CharacterState states;
    private Camera mainCam;   

    [Header("Inverse Kinematics")]
    [SerializeField] private Vector3 lookAtPos;
    [SerializeField] private float lookAtPointDistance;
    [SerializeField] private LayerMask lookAtIgnoreMask;

    [Header("Movement")]
    [SerializeField] float currentSpeed;
    [SerializeField] float turnSpeed;
    [SerializeField] private float leaderDistThreshold;
    public float targetAngle;
    private Vector3 lastStoppedPos;
    private float moveDifferenceAmount; //stores total move amount from last input
    public bool movedBeyondThreshold { get; private set; }
    public Vector2 direction { get; set; }
    public bool isSprinting { get; set; }
    public bool isMoving { get; private set; }
    [SerializeField] private bool isFalling;
    private float targetSpeed;
    private float turnSpeedVelocity;

    [Header("GroundCheck/Jumping")]
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Transform groundCheckObj;
    [SerializeField] private float gravityScale;
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask whatIsWalkable;
    public bool jumped { get;  set; }

    private Vector3 playerVelocity;

    void Awake()
    {
        states = GetComponent<CharacterState>();
    }

    void Start()
    {
        mainCam = cam.GetComponent<Camera>();        
    }

    // Update is called once per frame
    void Update()
    {       
        handleCameraRotation();
        groundCheck();
        Move();
        moveTeam();
        updateLookAt();
    }

    private void Move() //Moves character control;er
    {      
        if (isSprinting) 
        {
            targetSpeed = states.thisCharacterStats.sprintSpeed;
        }
        else
        {
            targetSpeed = states.thisCharacterStats.walkSpeed;
        }       

        if (direction.magnitude >= .1f) //Checks if any input is being recieved from WASD
        {
            isMoving = true;

            float newSpeed = Utils.lerpValue(currentSpeed, targetSpeed, states.thisCharacterStats.speedGain * Time.deltaTime, true);
            currentSpeed = newSpeed;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; //Always move forward relative to camera
            states.thisController.Move(moveDir.normalized * currentSpeed * Time.deltaTime); //Move player
        }
        else
        {
            isMoving = false;
            float newSpeed = Utils.lerpValue(currentSpeed, 0, states.thisCharacterStats.speedGain * Time.deltaTime, false);  //Lerps new speed between whatever the current speed is and the target speed we want to achieve
            currentSpeed = newSpeed;
        }

        if (jumped && isGrounded) //If the palyer pressed Jump and they are grounded, Jump
        {
            states.thisAnimHook.setBool(states.thisAnimHook.jumped, true);
            playerVelocity.y += Mathf.Sqrt(states.thisCharacterStats.jumpHeight * -3.0f * gravityScale);
        }
        else if (jumped && !isGrounded)//Whilst player is in the air, reset jumped value so they can jump as soon as they land
        {
            jumped = false;
        }
        
        if (!isGrounded)
        {
            playerVelocity.y += gravityScale * Time.deltaTime; //Apply gravity to playerVelocity
        }

        states.thisAnimHook.setSpeed(currentSpeed); //Calls functions that sets blend tree speed variable on this characters animator
        states.thisController.Move(playerVelocity * Time.deltaTime); //Move player on the Y.
    }  

    private void groundCheck()
    {
        isGrounded = sphereCastGround(); //Sets isGrounded to the bool return value of the CheckForGround method;

        if (isGrounded) //If the player is grounded, velocity should be 0
        {
            playerVelocity.y = 0f;
            states.thisAnimHook.setBool(states.thisAnimHook.isGrounded, true);
        }
        else
        {
            states.thisAnimHook.setBool(states.thisAnimHook.isGrounded, false);
        }

        if (playerVelocity.y < -0.1f) //If the player is grounded, velocity should be 0
        {
            isFalling = true;
            states.thisAnimHook.setBool(states.thisAnimHook.jumped, false);
        }
        else
        {
            isFalling = false;
        }

        states.thisAnimHook.setBool(states.thisAnimHook.isFalling, isFalling);
    }

    private bool sphereCastGround() //Uses a CheckSphere method to check if the player is grounded
    {
        bool grounded = Physics.CheckSphere(groundCheckObj.position, groundCheckRadius, whatIsWalkable);

        if (grounded) //If they are, return true
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void handleCameraRotation()
    {
        //If player is blocking face the look at position
        if (states.isThisCharacterUsingAltAction() == true)
        {
            Debug.Log("Using alternate rotation");

            Quaternion targetRotation = getFacingAngle(lookAtPos, transform.position);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2 * Time.deltaTime);

            isSprinting = false;
        }

        //When player is locking on, face target.
        if (TargetManager.instance.isLockingOn && states.thisCamManger.CurrentTarget != null)
        {
            Debug.Log("Locked on rotation");

            Quaternion targetRotation = getFacingAngle(states.thisCamManger.CurrentTarget.transform.position, transform.position);
            targetRotation.x = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }

        if (direction.magnitude >= .1f) //Checks if any input is being recieved from WASD
        {
            targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cam.eulerAngles.y; //Facing forward vector of the camera when moving
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeedVelocity, turnSpeed);

            if (states.isThisCharacterUsingAltAction() == false && !TargetManager.instance.isLockingOn)
            {
                Debug.Log("Standard rotation");
                transform.rotation = Quaternion.Euler(0f, angle, 0f); //rotates transform using angle calculated from camera
            }
        }
    }

    private void moveTeam() //Moves team if player has gone past a certain threshold.
    {
        if (direction == Vector2.zero)
        {
            lastStoppedPos = transform.position; //Stores the current position when stopped
        }
        else
        {
            moveDifferenceAmount = Vector3.Distance(transform.position, lastStoppedPos); //Checks the distance between this update position and the position when the player was last stopped

            if (moveDifferenceAmount > leaderDistThreshold)
            {
                movedBeyondThreshold = true; //If leader is too far from the team, team will move towards positions
            }
            else
            {
                movedBeyondThreshold = false; //If the leader has not moved past threshold, no need for the team to move
            }
        }
    }

    private void updateLookAt() //get a lookAt position by casting a ray and pass that vector3 to the animHook class
    {
        RaycastHit hit;
        Ray cameraAim = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Debug.DrawLine(cameraAim.origin, cameraAim.GetPoint(20f), Color.red);
        
        if (Physics.Raycast(cameraAim, out hit, 200f, lookAtIgnoreMask))
        {
            lookAtPos = cameraAim.GetPoint(lookAtPointDistance);
        }

        states.thisAnimHook.PlayerLookAtPos = lookAtPos;
    }
    private Quaternion getFacingAngle(Vector3 _pointA, Vector3 _pointB)
    {
        Vector3 relativePos = _pointA - _pointB;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        toRotation.x = 0;
        return toRotation;
    }
  
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(lookAtPos, .2f);
    }
}
