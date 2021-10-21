using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items / Weapons")]
public class WeaponItems : Items
{
    [Header("Weapon Stats"), Space(20)]
    public GameObject weaponObject;

    public enum WeaponType { Melee, Ranged };
    public WeaponType weaponType;
}
