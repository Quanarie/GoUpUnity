using System.Collections;
using UnityEngine;
using GlobalTypes;
using System;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private LayerMask layerMask;
    
    //Contacts with walls/ceiling/ground
    public bool IsOnGround { get; private set; }
    public bool Left { get; private set; }
    public bool Right { get; private set; }
    public bool Above { get; private set; }

    private bool disableGroundCheck;
    private GroundType groundType;
    public bool HitGroundThisFrame { get; private set; }
    private bool inAirLastFrame;

    private Vector2 moveAmount;
    private Vector2 currentPosition;
    private Vector2 lastPosition;

    private new Rigidbody2D rigidbody;
    private new CapsuleCollider2D collider;

    private float slopeAngle;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        inAirLastFrame = !IsOnGround;

        if (IsOnGround)
        {
            if ((moveAmount.x > 0f && slopeAngle < 0f) || (moveAmount.x < 0f && slopeAngle > 0f))
            {
                moveAmount.y = -Mathf.Abs(Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * moveAmount.x);
            }
        }

        lastPosition = rigidbody.position;
        currentPosition = lastPosition + moveAmount;
        rigidbody.MovePosition(currentPosition);
        moveAmount = Vector2.zero;

        if (!disableGroundCheck)
            CheckIfGrounded();

        CheckOtherContacts();

        if (IsOnGround && inAirLastFrame) HitGroundThisFrame = true;
        else HitGroundThisFrame = false;
    }

    public void Move(Vector2 movement)
    {
        moveAmount += movement;
    }

    private void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(collider.bounds.center, collider.size,
                           CapsuleDirection2D.Vertical, 0f, Vector2.down, raycastDistance, layerMask);

        if (hit.collider)
        {
            groundType = DetermineGroundType(hit.collider);
            slopeAngle = slopeAngle = Vector2.SignedAngle(Vector2.up, hit.normal);
            IsOnGround = true;
        }
        else
        {
            groundType = GroundType.none;
            IsOnGround = false;
        }
    }

    private void CheckOtherContacts()
    {
        RaycastHit2D leftHit = Physics2D.BoxCast(collider.bounds.center, collider.size * 0.75f,
                               0f, Vector2.left, raycastDistance, layerMask);
        if (leftHit.collider) Left = true;
        else Left = false;

        RaycastHit2D rightHit = Physics2D.BoxCast(collider.bounds.center, collider.size * 0.75f,
                               0f, Vector2.right, raycastDistance, layerMask);
        if (rightHit.collider) Right = true;
        else Right = false;

        RaycastHit2D aboveHit = Physics2D.CapsuleCast(collider.bounds.center, collider.size,
                           CapsuleDirection2D.Vertical, 0f, Vector2.up, raycastDistance, layerMask);
        if (aboveHit.collider) Above = true;
        else Above = false;

    }

    private GroundType DetermineGroundType(Collider2D collider)
    {
        if (collider.TryGetComponent(out GroundEffector groundEffector))
        {
            return groundEffector.GroundType();
        }
        else
        {
            return GroundType.LevelGeometry;
        }
    }

    public void DisableGroundCheck()
    {
        IsOnGround = false;
        disableGroundCheck = true;
        StartCoroutine(EnableGroundCheck());
    }

    IEnumerator EnableGroundCheck()
    {
        yield return new WaitForSeconds(0.1f);
        disableGroundCheck = false;
    }    
}
