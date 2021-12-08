using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class BoxingEvents
{
    public static PowerupEvent powerEvent = new PowerupEvent();
    public static AttackEvent attackEvent = new AttackEvent();
}

public class PowerupEvent : UnityEvent<PowerupEventData> { }
public class AttackEvent : UnityEvent<GameObject> { }

public class PowerupEventData
{
    public GameObject player;
    public GameObject powerup;

    public PowerupEventData(GameObject player, GameObject powerup)
    {
        this.player = player;
        this.powerup = powerup;
    }
}