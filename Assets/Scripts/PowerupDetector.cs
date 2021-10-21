using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PowerupDetector : MonoBehaviour
{
    CircleCollider2D circleCollider;
    public WeaponItemManager weaponItemManager;

    public WeaponItems weapon;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        weapon = weaponItemManager.weaponItemsList[Random.Range(0, weaponItemManager.weaponItemsList.Count)];
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().weaponItem = weapon;
            Destroy(gameObject);
        }
    }
}
