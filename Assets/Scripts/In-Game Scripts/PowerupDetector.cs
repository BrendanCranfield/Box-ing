using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PowerupDetector : MonoBehaviour
{
    CircleCollider2D circleCollider;
    PowerupManager powerupManager;

    [SerializeField] bool isRangedWeapon;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        powerupManager = PowerupManager.powerupManager;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Needs stuff here

            Destroy(gameObject);
        }
    }
}
