using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[AddComponentMenu("Player Movement / Player Manager")]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] int maxLives = 3;
    [SerializeField] float gracePeriodAmount = 5;
    //[HideInInspector]
    public WeaponItems weaponItem;
    CircleCollider2D attackCollider;

    [HideInInspector] public HealthSystem healthSystem;
    [HideInInspector] public bool attacking;
    SpriteRenderer sprite;
    bool canBeAttacked;

    private void Awake()
    {
        BoxingEvents.attackEvent.AddListener(HandleAttacking);
    }

    private void Start()
    {
        MultiplayerManager.multiplayer.AddToUsers(GetComponent<PlayerInput>());
        sprite = GetComponent<SpriteRenderer>();
        healthSystem = new HealthSystem(100);
        attackCollider = GetComponentInChildren<CircleCollider2D>();

        healthSystem.MaxLives = maxLives;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        canBeAttacked = true;
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
            canBeAttacked = false;
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
        canBeAttacked = true;
    }

    #endregion

    #region Fighting

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) { attacking = true; Debug.Log("Attacking"); }
    }

    public void OnGrapple(InputAction.CallbackContext context)
    {

    }

    public void HandleAttacking(GameObject enemyPlayer)
    {
        enemyPlayer.GetComponent<PlayerManager>().healthSystem.Damage(10);
        if(canBeAttacked)
        {

        }
        //weaponItem.GetComponent<>()   //Get weapon enum from script and constructor

        /*
        if (weaponItem != null)
        {
            switch (currentWeapon)
            {
                case WeaponItems.WeaponType.Ranged: //Current weapon equipt is using ranged damage. eg bows, guns, etc.

                    break;

                case WeaponItems.WeaponType.Melee: //Current weapon equipt is a melee weapon. eg sword, knife, etc.

                    break;
            }
        }
        else //Using fists to attack
        {

        }
        */
    }

    #endregion
}