using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Player Movement / Platform Example")]
public class PlatformerScript : MonoBehaviour
{
    /*  To add animations, simply add an animator component and set up a blend tree for movement.   */

    private new Rigidbody2D rigidbody;

    [Header("Grounded Detection")]
    [SerializeField]
    bool isGrounded = true;
    LayerMask ignoreLayers;
    [SerializeField]
    Transform groundCheck;

    const float GroundedRadius = .2f;
    bool facingRight = true;
    private Vector3 velocity = Vector3.zero;

    [Header("Movement Stats")]
    [HideInInspector]
    public bool isJumping;
    [SerializeField]
    float fallingSpeed = 45f, jumpForce = 400f;
    [Range(0, .3f)]
    [SerializeField]
    float movementSmoothing = 0.05f;
    [SerializeField]
    bool inAirControl;
    [HideInInspector]
    public float horizontal, vertical;
    [HideInInspector]
    public Vector2 moveDirection;

    [HideInInspector] //Temporarily Hide this
    public UnityEvent OnLanding;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        ignoreLayers = ~(1 << 8 | 1 << 11);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleFallingAndGrounded();
        HandleJumping();
    }

    #region Movement

    private void HandleMovement()
    {
        if (isGrounded || inAirControl)
        {
            Vector3 targetVelocity = new Vector3(horizontal * 10f, rigidbody.velocity.y);
            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing);

            if (horizontal > 0 && !facingRight)
            {
                Flip();
            }
            else if (horizontal < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    private void HandleJumping()
    {
        if (isGrounded && isJumping)
        {
            isGrounded = false;
            rigidbody.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }
    }

    private void HandleFallingAndGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, new Vector2(transform.localScale.x, GroundedRadius), ignoreLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                if (!wasGrounded)
                    OnLanding.Invoke();
            }
        }

        if(!isGrounded)
        {
            rigidbody.AddForce(-Vector2.up * fallingSpeed);
            rigidbody.AddForce(moveDirection * fallingSpeed / 7.5f);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    #endregion

    private void OnDrawGizmos()
    {
        if(isGrounded)
            Gizmos.color = Color.green;
        else if(!isGrounded)
            Gizmos.color = Color.red;

        Gizmos.DrawCube(groundCheck.position, new Vector2(transform.localScale.x, GroundedRadius));
    }
}
