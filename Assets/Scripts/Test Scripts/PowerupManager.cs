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
    private List<WeaponItems> rangedWeapons = new List<WeaponItems>();
    private List<WeaponItems> meleeWeapons = new List<WeaponItems>();

    public List<WeaponItems> weapons = new List<WeaponItems>();
    public List<PowerupItem> powerups = new List<PowerupItem>();

    private void Start()
    {
        powerupManager = this;

        for (int i = 0; i < weapons.Count; i++)     //Loops through entire weapons list and separates them into ranged and melee weapons.
        {
            if (weapons[i].weaponType == WeaponItems.WeaponType.Ranged)
                rangedWeapons.Add(weapons[i]);
            else
                meleeWeapons.Add(weapons[i]);
        }
    }

    private void Update()
    {
        // Temporary use for debugging
        if(Input.GetKeyDown(KeyCode.T))
        {
            WeaponItems rangedItem = ChooseRandomWeapon(true);
            Debug.Log($"Ranged Weapon: {rangedItem.itemName}   :   {rangedItem.dropChance}");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            WeaponItems meleeItem = ChooseRandomWeapon(false);
            Debug.Log($"Melee Weapon: {meleeItem.itemName}   :   {meleeItem.dropChance}");
        }
    }

    public WeaponItems ChooseRandomWeapon(bool isRanged)
    {
        var returnedItem = weapons[Random.Range(0, weapons.Count)];

        switch(isRanged)    //Chooses random weapon based on if the weapon is ranged or not.
        {
            case true:
                for (int i = 0; i < rangedWeapons.Count; i++)
                {
                    if (Random.value <= rangedWeapons[i].dropChance / 100)  //Randomises a value and compares that to the drop chance.
                    {
                        returnedItem = rangedWeapons[i];
                    }
                }
                return returnedItem;

            case false:
                for (int i = 0; i < meleeWeapons.Count; i++)
                {
                    if (Random.value <= meleeWeapons[i].dropChance / 100)
                    {
                        returnedItem = meleeWeapons[i];
                    }
                }
                return returnedItem;
        }
    }

    public PowerupItem ChooseRandomPowerup()
    {
        return powerups[Random.Range(0, powerups.Count)];   //Basic powerup method to return a random powerup. Can be changed later if needed.
    }
}
