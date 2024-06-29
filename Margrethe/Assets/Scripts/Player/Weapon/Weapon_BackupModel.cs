using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HangType { lowBackHang, BackHang, SideHang }

public class Weapon_BackupModel : MonoBehaviour
{
    public WeaponType weaponType;
    [SerializeField] private HangType hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);
    public bool HangTypeIs(HangType hangType) => this.hangType == hangType;
}
