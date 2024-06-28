using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20.0f; // Скорость по умолчанию, из которой выводится формула для массы.

    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady; // Готовность оружия

    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefab; // Префаб пули
    [SerializeField] private float bulletSpeed; // Скорость пули

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        Invoke("EquipStartingWeapon", 0.1f);
    }

    #region Slots management - Pickup\ Equip\ Drop\ Ready Weapon
    private void EquipStartingWeapon() => EquipWeapon(0);

    private void EquipWeapon(int i)
    {
        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("No slots avalible");
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnWeaponBackupModels();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
        {
            return;
        }

        weaponSlots.Remove(currentWeapon);

        EquipWeapon(0);
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready; // Готовность оружия к логическому значению
    public bool WeaponReady() => weaponReady; // Проверка готовности
    #endregion

    private void Shoot()
    {
        // невозможно стрелять пока оружие не готово 
        if (WeaponReady() == false)
        {
            return;
        }

        if (currentWeapon.CanShoot() == false)
        {
            return;
        }

        GameObject newBullet = ObjectPool.instance.GetBullet();

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;

        player.weaponVisuals.PlayFireAnimation();
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
        {
            direction.y = 0;
        }

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;
    public Weapon CurrentWeapon() => currentWeapon;

    public Weapon BackupWeapon()
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon != currentWeapon)
            {
                return weapon;
            }
        }

        return null;
    }

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControlls controlls = player.controlls;

        controlls.Character.Fire.performed += context => Shoot();

        controlls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controlls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        controlls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controlls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
    }
    #endregion
}
