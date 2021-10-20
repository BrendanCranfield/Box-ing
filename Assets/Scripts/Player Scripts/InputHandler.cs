using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Movement / Input Handler")]
public class InputHandler : MonoBehaviour
{
    PlayerController playerControls;

    PlatformerScript platformExample;
    [HideInInspector]
    public Vector3 moveInput;

    private void Start()
    {
        if (TryGetComponent(out PlatformerScript example))
        {
            platformExample = example;
            Debug.Log("Platformer script found");
        }
        else
            Debug.Log("No scripts found");
    }

    private void OnEnable()
    {
        if(playerControls == null && platformExample != null)
        {
            playerControls = new PlayerController();
            playerControls.PlayerMovement.Movement.performed += inputContext =>
            {
                moveInput = inputContext.ReadValue<Vector2>();

                platformExample.horizontal = moveInput.x;
                platformExample.vertical = moveInput.y;
                platformExample.moveDirection = moveInput;

                if (moveInput.y > 0.5f)
                    platformExample.isJumping = true;
                else
                    platformExample.isJumping = false;
            };
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
