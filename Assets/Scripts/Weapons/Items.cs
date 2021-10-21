using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Items : ScriptableObject
{
    [Header("Item Stats")]
    public Image itemIcon;
    public string itemName;
    [TextArea] public string itemDescription;

    [Range(0, 100)] 
    public float dropChance;
}
