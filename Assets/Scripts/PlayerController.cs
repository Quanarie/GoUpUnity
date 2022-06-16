using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float gravity = 10f;

    private bool isJumping;

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
        moveDirection.x = input.x * walkSpeed;
        if (characterController.IsOnGround())
        {
            moveDirection.y = 0f;
            isJumping = false;
            if (startJump)
            {
                startJump = false;
                moveDirection.y = jumpSpeed;
                characterController.DisableGroundCheck();
                isJumping = true;
            }
        }
        else
        {
            if (releasedJump)
            {
                releasedJump = false;
                if (moveDirection.y > 0)
                {
                    moveDirection /= 2f;
                }
            }
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    //Input system methods
    public void OnMovement(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (characterController.IsOnGround())
                startJump = true;
        }
        else if (context.canceled)
        {
            if (!characterController.IsOnGround())
                releasedJump = true;
        }
    }
}
