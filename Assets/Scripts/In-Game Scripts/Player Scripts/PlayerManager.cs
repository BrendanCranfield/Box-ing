using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[AddComponentMenu("Player Movement / Player Manager")]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    int playerLives = 3;
    [SerializeField]
    float gracePeriodAmount = 5;
    //[HideInInspector]
    public WeaponItems weaponItem;

    HealthSystem healthSystem;
    bool canBeAttacked;
    SpriteRenderer sprite;

    //[HideInInspector]
    public bool isDashing, lightAttack, heavyAttack;

    private void Start()
    {
        MultiplayerManager.multiplayer.AddToUsers(GetComponent<PlayerInput>());
        sprite = GetComponent<SpriteRenderer>();
        healthSystem = new HealthSystem(100);

        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        canBeAttacked = true;
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        //If health is changed. eg damage, healing, etc.

        //Can be used to show health bar or health percentage
        Debug.Log(healthSystem.GetHealth());
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        //If player dies
        playerLives -= 1;

        if (playerLives > 0 && gameObject.activeInHierarchy)
        {
            transform.position = new Vector3(Random.Range(-7, 7), 4, 0);
            canBeAttacked = false;
            StartCoroutine(GracePeriod(gracePeriodAmount));
        }
        else { gameObject.SetActive(false); }
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

    private void OnBecameInvisible()
    {
        //Temporary
        if(canBeAttacked) healthSystem.Damage(10000);
        else
        {
            Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minScreenBounds.x + 1, maxScreenBounds.x - 1), Mathf.Clamp(transform.position.y, minScreenBounds.y + 1, maxScreenBounds.y - 1), transform.position.z);
        }
    }

    public void HandleAttacking()
    {
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
}