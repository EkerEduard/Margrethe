using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/ Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;

    [Header("Regular shot")]
    public WeaponType weaponType;
    public ShootType shootType;
    public int bulletsPerShot = 1;
    public float fireRate;

    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Header("Burst shot")]
    public bool burstAvalible;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = 0.1f;

    [Header("Weapon Spread")]
    public float baseSpread;
    public float maxSpread;

    public float spreadIncreaseRate = 0.15f;

    [Header("Weapon generics")]
    [Range(1.0f, 3.0f)]
    public float reloadSpeed = 1.0f;
    [Range(1.0f, 3.0f)]
    public float equipmentSpeed = 1.0f;
    [Range(4.0f, 8.0f)]
    public float gunDistance = 4.0f;
    [Range(4.0f, 8.0f)]
    public float cameraDistance = 6.0f;
}
