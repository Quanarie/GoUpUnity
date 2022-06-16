using System.Collections;
using UnityEngine;
using GlobalTypes;
using System;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private LayerMask layerMask;

    private bool isOnGround;
    private bool disableGroundCheck;
    private GroundType groundType;

    public bool IsOnGround() => isOnGround;

    private Vector2 moveAmount;
    private Vector2 currentPosition;
    private Vector2 lastPosition;

    private new Rigidbody2D rigidbody;
    private new CapsuleCollider2D collider;

    private Vector2[] raycastPositions = new Vector2[3];
    private RaycastHit2D[] raycastHits = new RaycastHit2D[3];

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (moveAmount.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (moveAmount.x > 0) transform.localScale = new Vector3(1, 1, 1);

        lastPosition = rigidbody.position;
        currentPosition = lastPosition + moveAmount;
        rigidbody.MovePosition(currentPosition);
        moveAmount = Vector2.zero;

        if (!disableGroundCheck)
            CheckIfGrounded();
    }

    public void Move(Vector2 movement)
    {
        moveAmount += movement;
    }

    private void CheckIfGrounded()
    {
        Vector2 raycastOrigin = rigidbody.position - new Vector2(0, collider.size.y / 2f);

        raycastPositions[0] = raycastOrigin + Vector2.left * collider.size.x / 2f;
        raycastPositions[1] = raycastOrigin;
        raycastPositions[2] = raycastOrigin + Vector2.right * collider.size.x / 2f;

        DrawDebugRays(Vector2.down, Color.green);

        int numberOfGroundHits = 0;

        for (int i = 0; i < raycastPositions.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastPositions[i], Vector2.down, raycastDistance, layerMask);
            if (hit.collider)
            {
                raycastHits[i] = hit;
                numberOfGroundHits++;
            }
        }
        if (numberOfGroundHits > 0)
        {
            if (raycastHits[1].collider)
            {
                groundType = DetermineGrounType(raycastHits[1].collider);
            }
            else
            {
                for (int i = 0; i < raycastHits.Length; i++)
                {
                    if (raycastHits[i].collider)
                    {
                        groundType = DetermineGrounType(raycastHits[i].collider);
                    }
                }
            }

            isOnGround = true;
        }
        else
        {
            groundType = GroundType.none;
            isOnGround = false;
        }
    }

    private GroundType DetermineGrounType(Collider2D collider)
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

    private void DrawDebugRays(Vector2 direction, Color color)
    {
        for (int i = 0; i < raycastPositions.Length; i++)
        {
            Debug.DrawRay(raycastPositions[i], direction * raycastDistance, color);
        }
    }

    public void DisableGroundCheck()
    {
        isOnGround = false;
        disableGroundCheck = true;
        StartCoroutine("EnableGroundCheck");
    }

    IEnumerator EnableGroundCheck()
    {
        yield return new WaitForSeconds(0.1f);
        disableGroundCheck = false;
    }    
}
