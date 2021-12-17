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
    [HideInInspector] public bool attacking, grappling, canAttack;
    [SerializeField] LayerMask attackingLayerMask;
    PlatformerScript platformerScript;

    private void Start()
    {
        MultiplayerManager.multiplayer.AddToUsers(GetComponent<PlayerInput>());
        healthSystem = new HealthSystem(100);

        healthSystem.MaxLives = maxLives;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.canBeAttacked = true;
        canAttack = true;

        platformerScript = GetComponent<PlatformerScript>();
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
        Debug.Log($"{gameObject.name} Health: {healthSystem.GetHealthPercent}%");
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
        platformerScript.playerSprite.color = new Color(platformerScript.playerSprite.color.r, platformerScript.playerSprite.color.g, platformerScript.playerSprite.color.b, 0.15f);
        Debug.Log($"Grace Period: {gracePeriodAmount} seconds");
        yield return new WaitForSeconds(gracePeriodTime);
        Debug.Log("Grace Period Ended!");
        platformerScript.playerSprite.color = new Color(platformerScript.playerSprite.color.r, platformerScript.playerSprite.color.g, platformerScript.playerSprite.color.b, 1f);
        healthSystem.canBeAttacked = true;
    }

    #endregion

    #region Fighting

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) { attacking = true; }
    }

    public void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.started) { grappling = true; Debug.Log("Grappling"); }  //Grapples the enemy player while holding down the button.
        if (context.canceled) { grappling = false; Debug.Log("Stopped grapple"); }  //lets the enemy player go if button is released.
    }

    public void HandleAttacking()
    {
        if(attacking && canAttack)
        {
            //Needs to get radius from weapon data
            Collider2D[] enemies = Physics2D.OverlapCircleAll(platformerScript.lookObject.position, 1, attackingLayerMask);
            for (int i = 0; i < enemies.Length; i++)
            {
                if(enemies[i].CompareTag("Player") && !gameObject) { enemies[i].GetComponent<PlayerManager>().healthSystem.Damage(5); Debug.Log($"{enemies[i].name} has taken 5 damage"); }
            }
        }
    }

    IEnumerator AttackingCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.25f);
        canAttack = true;
    }

    public void HandleGrapple()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(platformerScript.lookObject.position, 1);
    }

    #endregion
}