using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType { SideEquipAnimation, BackEquipAnimation };
public enum HoldType { CommonHold = 1, LowHold, HighHold };

public class Weapon_Model : MonoBehaviour
{
    public WeaponType weaponType;
    public EquipType equipAnimationType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform leftTargetPoint;
    public Transform leftHintPoint;
}
