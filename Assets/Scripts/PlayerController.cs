using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float friction = 0.5f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashTime = 0.2f;

    //Ability toggles
    public bool CanDash;

    public bool IsJumping { get; set; }
    public bool IsDashing { get; set; }

    //Input system flags
    private bool startJump;
    private bool releasedJump;

    private Vector2 input;
    private Vector2 moveDirection;
    private CharacterController2D characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController2D>();
    }

    private void Update()
    {
        if (moveDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (moveDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (IsDashing)
        {
            if (characterController.Above)
            {
                applyGravity();
            }

            if (CanDash)
            {
                CanDash = false;
                if (input == Vector2.zero)
                {
                    moveDirection = new Vector2(0f, 1f) * dashSpeed;
                }
                else
                {
                    moveDirection = input * dashSpeed;
                }
            }
        }
        else if (!IsDashing)
        {
            if (input.x == 0f)
            {
                moveDirection.x = Mathf.Lerp(moveDirection.x, 0, friction);
            }
            else
            {
                moveDirection.x += input.x * acceleration;
                moveDirection.x = Mathf.Clamp(moveDirection.x, -walkSpeed, walkSpeed);
            }
           
        }

        if (!IsDashing)
        {
            if (characterController.IsOnGround)
            {
                moveDirection.y = 0f;
                IsJumping = false;
                CanDash = true;

                if (startJump)
                {
                    startJump = false;
                    moveDirection.y = jumpSpeed;
                    characterController.DisableGroundCheck();
                    IsJumping = true;
                }
            }
            else
            {
                if (releasedJump)
                {
                    releasedJump = false;
                    if (moveDirection.y > 0)
                    {
                        moveDirection.y /= 2f;
                    }
                }

                applyGravity();
            }
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void applyGravity()
    {
        if (moveDirection.y > 0f && characterController.Above)
        {
            moveDirection.y = 0f;
        }

        moveDirection.y -= gravity * Time.deltaTime;
    }

    //Input system methods
    public void OnMovement(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        normalizeXandYofInput();
    }

    private void normalizeXandYofInput()
    {
        if (input.x != 0f) input.x = input.x / Mathf.Abs(input.x);
        if (input.y != 0f) input.y = input.y / Mathf.Abs(input.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Jump part
            startJump = true;
            releasedJump = false;

            //Dash part
            if (IsJumping && CanDash && !characterController.IsOnGround)
            {
                StartCoroutine(Dash());
            }

        }
        else if (context.canceled)
        {
            releasedJump = true;
            startJump = false;
        }
    }

    IEnumerator Dash()
    {
        IsDashing = true;
        yield return new WaitForSeconds(dashTime);
        IsDashing = false;
    }
}
