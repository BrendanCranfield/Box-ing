using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Movement / Player Manager")]
public class PlayerManager : MonoBehaviour
{
    //[HideInInspector]
    public GameObject weaponItem;

    public void HandleAttacking()
    {
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