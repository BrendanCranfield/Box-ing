using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[AddComponentMenu("Player Movement / Player Manager")]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] int maxLives = 3;
    [SerializeField] float gracePeriodAmount = 5;

    [HideInInspector] public HealthSystem healthSystem;
    [HideInInspector] public bool attacking, grappling;
    SpriteRenderer sprite;

    private void Start()
    {
        MultiplayerManager.multiplayer.AddToUsers(GetComponent<PlayerInput>());
        sprite = GetComponent<SpriteRenderer>();
        healthSystem = new HealthSystem(100);

        healthSystem.MaxLives = maxLives;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.canBeAttacked = true;
    }

    private void LateUpdate()
    {
        attacking = false;
    }

    #region Health & Death

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        //If health is changed. eg damage, healing, etc.

        //Can be used to show health bar or health percentage
        Debug.Log($"Player Health: {healthSystem.GetHealthPercent}%");
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        //If player dies
        if (healthSystem.Lives > 0 && gameObject.activeInHierarchy)
        {
            transform.position = new Vector3(Random.Range(-7, 7), 4, 0);
            healthSystem.canBeAttacked = false;
            StartCoroutine(GracePeriod(gracePeriodAmount));
        }
        else { GetComponent<PlatformerScript>().enabled = false; }
    }

    IEnumerator GracePeriod(float gracePeriodTime)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.15f);
        Debug.Log($"Grace Period: {gracePeriodAmount} seconds");
        yield return new WaitForSeconds(gracePeriodTime);
        Debug.Log("Grace Period Ended!");
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        healthSystem.canBeAttacked = true;
    }

    #endregion

    #region Fighting

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) { attacking = true; Debug.Log("Attacking"); }
    }

    public void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.started) { grappling = true; Debug.Log("Grappling"); }  //Grapples the enemy player while holding down the button.
        if (context.canceled) { grappling = false; Debug.Log("Stopped grapple"); }  //lets the enemy player go if button is released.
    }

    public void HandleAttacking()
    {

    }

    public void HandleGrapple()
    {

    }

    #endregion
}