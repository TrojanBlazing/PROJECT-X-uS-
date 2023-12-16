using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class PlayerMovement : MonoBehaviour
{
    //  Movement control Variables
    private float SmoothMove = 0.1f; // smoothness of movement 
    private float WalkingSpeed = 3.5f; 
    private float RunningSpeed = 7f; 
    private float GravityPower = 10f; // Strength of gravity affecting the player.
    private float JumpPower = 20f; 
    private float DoubleJumpPower = 25f; 
    private int JumpsRemaining = 1; 

    // Storing Current Velocity Variables.
    private Vector3 CurrentVelocity; // Current velocity of the player.
    private Vector3 SDampVelocity; // Velocity used for smoothing movement transitions.
    private Vector3 CurrentForceVelocity; // Current force-based velocity.

   
    CharacterController controller;

   
    void Start()
    {
       
        controller = GetComponent<CharacterController>();
    }

   
    void Update()
    {
        
        Move();
    }

   
    private void Move()
    {
        //Player input for movement.
        Vector3 PlayerInput = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        };

        // To stop faster diagonal movement and for to store its existing movements.
        if (PlayerInput.magnitude > 1f)
        {
            PlayerInput.Normalize();
        }

        // Transform input direction into world space.
        Vector3 MoveDir = transform.TransformDirection(PlayerInput);

        // Checks the current movement speed based if the player is holding the "Left Shift" key.
       // ?(operator) its basically like if else 
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunningSpeed : WalkingSpeed;

        // For smoothness of players Velocity.
        CurrentVelocity = Vector3.SmoothDamp(CurrentVelocity, MoveDir * currentSpeed, ref SDampVelocity, SmoothMove);
        controller.Move(CurrentVelocity * Time.deltaTime);

        //Raycast to check if its grounded
        Ray GroundCheck = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(GroundCheck, 1.1f))
        {
            // Reset jumps when grounded and apply a downward force.
            JumpsRemaining = 1;
            CurrentForceVelocity.y = -1f;

            //Checking for Jump press or input
            if (Input.GetKey(KeyCode.Space))
            {
                CurrentVelocity.y = JumpPower;
            }
        }
        else
        {
            // Apply gravity when the player is not grounded.
            CurrentForceVelocity.y -= GravityPower * Time.deltaTime;

            // Check for double jump input 
            if (JumpsRemaining > 0 && Input.GetKeyDown(KeyCode.Space))
            {
                CurrentVelocity.y = DoubleJumpPower;
                JumpsRemaining--;
            }
        }

        // Moves the player based on the force velocity.
        controller.Move(CurrentForceVelocity * Time.deltaTime);
    }
}

