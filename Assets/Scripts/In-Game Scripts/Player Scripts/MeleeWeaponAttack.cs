using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponAttack : MonoBehaviour
{
    PlayerManager playerManager;
    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerManager.attacking && collision.CompareTag("Player") && !gameObject.transform.parent)
        {
            BoxingEvents.attackEvent.Invoke(collision.gameObject);
        }
    }
}
