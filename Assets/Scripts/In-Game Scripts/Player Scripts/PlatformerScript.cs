using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[AddComponentMenu("Player Movement / Movement Script")]
public class PlatformerScript : MonoBehaviour
{
    /*  To add animations, simply add an animator component and set up a blend tree for movement.   */

    private new Rigidbody2D rigidbody;

    [Header("Grounded Detection")]
    public bool isGrounded = true;  //Check if grounded.
    [SerializeField] LayerMask ignoreLayers; //Layer mask to detect ONLY the ground.
    [SerializeField] Transform groundCheck; //Transform to check for ground objects.

    const float GroundedRadius = .2f;   //Sets ground radius.
    bool facingRight = true;    //Checks if facing right.
    private Vector3 velocity = Vector3.zero;    //For SmoothDamp in HandleMovement. (Requires velocity reference)
    Vector2 moveInput;  //Sets the movement direction of the player based on user inputs.


    [Header("Movement Stats")]
    [HideInInspector] public bool isDashing;    //Checks if the player is dashing.
    [HideInInspector] public bool isJumping;    //Checks if the player is jumping.
    public float fallingSpeed = 45f, jumpForce = 400f, movementSpeed = 5f;      //Sets speed variables for use in moving or powerups.
    [Range(0, .3f)] [SerializeField] float movementSmoothing = 0.05f;   //Used for making the movement smooth instead of jittery.
    [SerializeField] bool inAirControl;     //Checks if player can move in the air.

    IEnumerator dashCoroutine;  //Saves dashing coroutine for easy use.
    bool canDash = true;    //Checks if the player can dash.
    float normalGravityScale;   //Important for smoothing out dashing.

    public UnityEvent OnLanding;    //This creates a unity event that can play particles if the player has landed on the floor.

    private void Start()
    {
        if (TryGetComponent(out Rigidbody2D rb)) { rigidbody = rb; }
        else { rigidbody = GetComponentInChildren<Rigidbody2D>(); }

        normalGravityScale = rigidbody.gravityScale;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleFallingAndGrounded();
        HandleJumping();
        HandleDashing();
    }

    #region Movement

    public void OnPlayerMovement(InputAction.CallbackContext inputContext)  //Sets the movement direction.
    {
        moveInput = inputContext.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext inputContext)    //Sets the jump based on user input.
    {
        if (inputContext.performed) { isJumping = true; }
        else { isJumping = false; }
    }

    private void HandleMovement()
    {
        if (isGrounded || inAirControl)
        {
            Vector3 targetVelocity = new Vector3(moveInput.x * movementSpeed, rigidbody.velocity.y);
            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing);

            if (moveInput.x > 0 && !facingRight) { Flip(); }
            else if (moveInput.x < 0 && facingRight) { Flip(); }
        }
    }

    public void HandleJumping()
    {
        if (isJumping)
        {
            rigidbody.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }
        else return;
    }

    private void HandleFallingAndGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, new Vector2(transform.localScale.x, GroundedRadius), ignoreLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                if (colliders[i].CompareTag("Powerups")) return;
                isGrounded = true;
                if (!wasGrounded) { OnLanding.Invoke(); }
            }
        }

        if (!isGrounded)
        {
            rigidbody.AddForce(-Vector2.up * fallingSpeed);
            rigidbody.AddForce(moveInput * fallingSpeed / 7.5f);
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

    #region Dashing

    public void OnDash(InputAction.CallbackContext inputContext)
    {
        if (canDash) 
        { 
            if (dashCoroutine != null) { StopCoroutine(dashCoroutine); }
            dashCoroutine = Dash(0.1f, 1f);
            StartCoroutine(dashCoroutine);
        }
    }

    IEnumerator Dash(float dashDuration, float dashCooldown)
    {
        Vector2 originalVelocity = rigidbody.velocity;
        isDashing = true;
        canDash = false;
        rigidbody.gravityScale = 0;
        rigidbody.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rigidbody.gravityScale = normalGravityScale;
        rigidbody.velocity = originalVelocity;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void HandleDashing()
    {
        if(isDashing)
        {
            rigidbody.AddForce(new Vector2(moveInput.x * movementSpeed * 2, moveInput.y * movementSpeed), ForceMode2D.Impulse);
        }
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
