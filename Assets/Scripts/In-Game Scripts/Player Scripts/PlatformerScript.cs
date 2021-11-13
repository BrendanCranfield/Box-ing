using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), (typeof(Collider2D)))]
[AddComponentMenu("Player Movement / Platform Example")]
public class PlatformerScript : MonoBehaviour
{
    /*  To add animations, simply add an animator component and set up a blend tree for movement.   */

    private new Rigidbody2D rigidbody;

    [Header("Grounded Detection")]
    public bool isGrounded = true;
    [SerializeField]
    LayerMask ignoreLayers;
    [SerializeField]
    Transform groundCheck;

    const float GroundedRadius = .2f;
    bool facingRight = true;
    private Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public int jumpAmount;


    [Header("Movement Stats")]
    [HideInInspector]
    public int totalJumpAmount = 2;
    [HideInInspector]
    public bool isJumping;
    public float fallingSpeed = 45f, jumpForce = 400f, movementSpeed = 5f;
    [Range(0, .3f)]
    [SerializeField]
    float movementSmoothing = 0.05f;
    [SerializeField]
    bool inAirControl;
    [HideInInspector]
    public float horizontal, vertical;
    [HideInInspector]
    public Vector2 moveDirection;

    public UnityEvent OnLanding;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
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
            Vector3 targetVelocity = new Vector3(horizontal * movementSpeed, rigidbody.velocity.y);
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

    public void HandleJumping()
    {
        if (isJumping && jumpAmount < totalJumpAmount)
        {
            rigidbody.AddForce(new Vector2(0f, jumpForce));
            isGrounded = false;
            isJumping = false;
            jumpAmount += 1;
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
                {
                    OnLanding.Invoke();
                    jumpAmount = 0;
                }
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(isGrounded)
            Gizmos.color = Color.green;
        else if(!isGrounded)
            Gizmos.color = Color.red;

        Gizmos.DrawCube(groundCheck.position, new Vector2(transform.localScale.x, GroundedRadius));
    }
#endif
}
