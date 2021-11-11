using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MultiplayerManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;
    public static MultiplayerManager multiplayer;
    public event EventHandler UserAdded;

    public List<PlayerInput> users = new List<PlayerInput>();

    private void Start()
    {
        multiplayer = this;
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    public void AddToUsers(PlayerInput player)
    {
        player.name = $"Player {users.Count + 1}";
        users.Add(player);
        if (UserAdded != null) { UserAdded(this, EventArgs.Empty); }
    }
}
