
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Player Movement / Input Handler")]
public class InputHandler : MonoBehaviour
{
    PlayerController playerControls;
    PlatformerScript platformExample;
    PlayerManager playerManager;

    [HideInInspector]
    public Vector2 moveInput;

    int dashCount;

    private void Start()
    {
        if (TryGetComponent(out PlatformerScript platformer)) { platformExample = platformer; }
        else { Debug.Log("Platform script not found"); }

        if(TryGetComponent(out PlayerManager manager)) { playerManager = manager; }
        else { Debug.Log("Player manager not found"); }
    }

    private void OnEnable()
    {
        if (playerControls == null) { playerControls = new PlayerController(); }
        playerControls.Enable();
    }

    public void OnPlayerMovement(InputAction.CallbackContext inputContext)
    {
        moveInput = inputContext.ReadValue<Vector2>();
        platformExample.horizontal = moveInput.x;
        platformExample.vertical = moveInput.y;
        platformExample.moveDirection = moveInput;

        if (moveInput.y > 0.5f) { platformExample.isJumping = true; }
        else { platformExample.isJumping = false; }
    }

    public void OnDash(InputAction.CallbackContext inputContext)
    {
        if (inputContext.performed)
        {
            //StartCoroutine(DashingTimer());
            Debug.Log($"Dashing");
        }
    }

    public void onJump(InputAction.CallbackContext inputContext)
    {
        playerManager.isJumping = true;
        Debug.Log($"Jumping");
    }

    public void OnLightAttack(InputAction.CallbackContext inputContext)
    {
        playerManager.lightAttack = true;
        Debug.Log($"Light");
    }

    public void OnHeavyAttack(InputAction.CallbackContext inputContext)
    {
        playerManager.heavyAttack = true;
        Debug.Log($"Heavy");
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void LateUpdate()
    {
        playerManager.isJumping = false;
        playerManager.isDashing = false;
        playerManager.lightAttack = false;
        playerManager.heavyAttack = false;
    }

    //Not currently in use
    IEnumerator DashingTimer()
    {
        platformExample.movementSpeed = 10;
        playerManager.isDashing = true;

        yield return new WaitForSeconds(0.5f);

        platformExample.movementSpeed = 5;
        playerManager.isDashing = false;
    }
}
