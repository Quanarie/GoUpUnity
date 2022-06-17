using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerController playerController;
    private CharacterController2D characterController;
    private Animator animator;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("horizontalMovement", Mathf.Abs(playerController.MoveDirection.x));
        animator.SetFloat("verticalMovement", playerController.MoveDirection.y);
        animator.SetBool("isOnGround", characterController.IsOnGround);
        animator.SetBool("isDashing", playerController.IsDashing);
        animator.SetBool("isJumping", playerController.IsJumping);
    }
}
