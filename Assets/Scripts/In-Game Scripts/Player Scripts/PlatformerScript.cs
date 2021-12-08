using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[AddComponentMenu("Player Movement / Movement Script")]
public class PlatformerScript : MonoBehaviour
{
    /*  To add animations, simply add an animator component and set up a blend tree for movement.   */

    new Rigidbody2D rigidbody;
    PlayerInput playerInput;
    Camera main;
    SpriteRenderer playerSprite;

    [SerializeField] GameObject landingEffect;
    [SerializeField] Transform lookObject;

    [Header("Grounded Detection")]
    public bool isGrounded = true;  //Check if grounded.
    [SerializeField] LayerMask ignoreLayers; //Layer mask to detect ONLY the ground.
    [SerializeField] Transform groundCheck; //Transform to check for ground objects.

    const float GroundedRadius = .2f;   //Sets ground radius.
    bool facingRight = true;    //Checks if facing right.
    private Vector3 velocity = Vector3.zero;    //For SmoothDamp in HandleMovement. (Requires velocity reference)
    Vector2 moveInput;  //Sets the movement direction of the player based on user inputs.

    [SerializeField] int maxJumpCount = 2;
    int currentJumpCount;
    public int JumpCount { get { return currentJumpCount - 1; } private set { if(currentJumpCount <= 0) { currentJumpCount = 0; } else { currentJumpCount = value - 1; } } }

    [Header("Movement Stats")]
    [HideInInspector] public bool isDashing;    //Checks if the player is dashing.
    public float fallingSpeed = 45f, jumpForce = 400f, movementSpeed = 5f;      //Sets speed variables for use in moving or powerups.
    [Range(0, .3f)] [SerializeField] float movementSmoothing = 0.05f;   //Used for making the movement smooth instead of jittery.
    [SerializeField] bool inAirControl;     //Checks if player can move in the air.

    IEnumerator dashCoroutine;  //Saves dashing coroutine for easy use.
    bool canDash = true;    //Checks if the player can dash.
    float normalGravityScale;   //Important for smoothing out dashing.

    public UnityEvent OnLanding;    //This creates a unity event that can play particles if the player has landed on the floor.

    Keyboard keyboard;
    Gamepad gamepad;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        normalGravityScale = rigidbody.gravityScale;
        JumpCount = maxJumpCount;
        playerInput = GetComponent<PlayerInput>();
        main = Camera.main;
        playerSprite = GetComponentInChildren<SpriteRenderer>();

        Debug.Log(playerInput.currentControlScheme);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleLook();
        HandleDashing();
        HandleJumping();
        HandleFallingAndGrounded();
    }

    #region Look Around
    Vector2 playerView;
    public void Look(InputAction.CallbackContext inputContext) { playerView = inputContext.ReadValue<Vector2>(); }   //Sets the look direction of the player.

    private void HandleLook()
    {
        switch(playerInput.currentControlScheme)
        {
            case "PC":
                //Using mouse
                var mousePosition = main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 lookDirection = mousePosition - transform.position;
                float pcAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90;
                lookObject.localRotation = Quaternion.AngleAxis(pcAngle, Vector3.forward);
                
                break;

            case "Gamepad":
                //Using controller
                float controllerAngle = Mathf.Atan2(playerView.y, playerView.x) * Mathf.Rad2Deg - 90;
                if (playerView.sqrMagnitude > 0) { lookObject.localRotation = Quaternion.AngleAxis(controllerAngle, Vector3.forward); }
                break;
        }
    }

    #endregion

    #region Movement
    bool isJumping;
    public void OnPlayerMovement(InputAction.CallbackContext inputContext) { moveInput = inputContext.ReadValue<Vector2>(); }   //Sets the movement direction.
    public void OnJump(InputAction.CallbackContext inputContext) { if (inputContext.performed) { isJumping = true; }    //Sets the jump based on user input.
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
        if(isJumping)
        {
            if (JumpCount > 0 && rigidbody != null || isGrounded && rigidbody != null)
            {
                JumpCount -= 1;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, 0);
                rigidbody.AddForce(new Vector2(0f, jumpForce * 10));
                isJumping = false;
            }
        }
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
                if(currentJumpCount != maxJumpCount) { currentJumpCount = maxJumpCount; }
                if (!wasGrounded) 
                { 
                    OnLanding.Invoke();
                    StartCoroutine(OnLandingEffects());
                }
            }
        }

        IEnumerator OnLandingEffects()
        {
            GameObject effect = Instantiate(landingEffect, transform.position, Quaternion.identity) as GameObject;
            effect.transform.position = new Vector2(transform.position.x, transform.position.y - 0.43f);
            effect.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(0.5f);
            Destroy(effect);
            StopCoroutine(OnLandingEffects());
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

        playerSprite.flipX = facingRight;
    }

    #endregion

    #region Dashing
    public void OnDash(InputAction.CallbackContext inputContext)
    {
        if (canDash && gameObject.activeInHierarchy) 
        { 
            if (dashCoroutine != null) { StopCoroutine(dashCoroutine); }
            dashCoroutine = Dash(0.1f, 0.5f);
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

    void HandleDashing() { if(isDashing) { rigidbody.AddForce(new Vector2(moveInput.x * movementSpeed * 1.5f, 0), ForceMode2D.Impulse); } }
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
