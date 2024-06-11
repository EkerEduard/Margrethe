using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControlls controlls;

    private void Awake()
    {
        controlls = new PlayerControlls();
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
