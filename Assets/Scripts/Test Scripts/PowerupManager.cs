using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct WeaponItems
{
    public string itemName;
    [Range(0.1f, 100)] public float dropChance;
    public GameObject weaponObject;
    public enum WeaponType { Melee, Ranged };
    public WeaponType weaponType;
}

[System.Serializable]
public struct PowerupItem
{
    public string itemName;
    [Range(0.1f, 100)] public float dropChance;
    public enum PowerupType { Strength, Speed, Falling };
    public PowerupType powerupType;
}

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager powerupManager;

    private void Awake()
    {
        powerupManager = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            ChooseRandomWeapon(true);
        }
    }

    public List<WeaponItems> weapons = new List<WeaponItems>();
    public List<PowerupItem> powerups = new List<PowerupItem>();

    public WeaponItems ChooseRandomWeapon(bool isRanged)
    {
        switch(isRanged)
        {
            case true:
                var rangedWeapons = weapons.OrderBy(i => i.weaponType == WeaponItems.WeaponType.Ranged).ToArray();
                var returnedRangedItem = rangedWeapons[0];

                for (int i = 0; i < rangedWeapons.Length; i++)
                {
                    if (Random.value <= rangedWeapons[i].dropChance / 100)
                    {
                        returnedRangedItem = rangedWeapons[i];
                    }
                }

                Debug.Log(returnedRangedItem.itemName);
                return returnedRangedItem;

            case false:
                var meleeWeapons = weapons.OrderBy(i => i.weaponType == WeaponItems.WeaponType.Melee).ToArray();
                var returnedMeleeItem = meleeWeapons[0];

                for (int i = 0; i < meleeWeapons.Length; i++)
                {
                    if (Random.value <= meleeWeapons[i].dropChance / 100)
                    {
                        returnedMeleeItem = meleeWeapons[i];
                    }
                }

                Debug.Log(returnedMeleeItem.itemName);
                return returnedMeleeItem;
        }
    }

    public PowerupItem ChooseRandomPowerup()
    {
        return powerups[Random.Range(0, powerups.Count)];
    }
}
