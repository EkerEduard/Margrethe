using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimationEvents : MonoBehaviour
{
    private Player_WeaponVisuals visualController;
    private Player_WeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<Player_WeaponVisuals>();
        weaponController = GetComponentInParent<Player_WeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximazeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();
    }

    public void ReturnRig()
    {
        visualController.MaximazeRigWeight();
        visualController.MaximazeLeftHandWeight();
    }


    public void WeaponGrabIsOver()
    {
        visualController.SetBusyGrabbingWeaponTo(false);
    }
}
