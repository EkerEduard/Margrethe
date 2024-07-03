using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControlls controlls { get; private set; } // Только для чтения
    public Player_Aim aim { get; private set; }
    public Player_Movement movement { get; private set; }
    public Player_WeaponController weapon { get; private set; }
    public Player_WeaponVisuals weaponVisuals { get; private set; }
    public Player_Interaction interaction { get; private set; }

    private void Awake()
    {
        controlls = new PlayerControlls();
        aim = GetComponent<Player_Aim>();
        movement = GetComponent<Player_Movement>();
        weapon = GetComponent<Player_WeaponController>();
        weaponVisuals = GetComponent<Player_WeaponVisuals>();
        interaction = GetComponent<Player_Interaction>();
    }

    // Acitve
    private void OnEnable()
    {
        controlls.Enable();
    }

    // Inactive
    private void OnDisable()
    {
        controlls.Disable();
    }
}
