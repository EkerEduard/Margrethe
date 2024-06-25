using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20.0f; // Скорость по умолчанию, из которой выводится формула для массы.

    [SerializeField] private Weapon currentWeapon;

    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefab; // Префаб пули
    [SerializeField] private float bulletSpeed; // Скорость пули
    [SerializeField] private Transform gunPoint; // Точка создания пули

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.bulletsInMagazine = currentWeapon.totalReserveAmmo;
    }

    #region Slots management - Pickup\ Equip\ Drop Weapon
    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("No slots avalible");
            return;
        }

        weaponSlots.Add(newWeapon);
    }

    private void DropWeapon()
    {
        if (weaponSlots.Count <= 1)
        {
            return;
        }

        weaponSlots.Remove(currentWeapon);

        currentWeapon = weaponSlots[0];
    }
    #endregion

    private void Shoot()
    {
        if (currentWeapon.CanShoot() == false)
        {
            return;
        }

        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 5);
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
        {
            direction.y = 0;
        }

        weaponHolder.LookAt(aim);
        gunPoint.LookAt(aim);

        return direction;
    }

    public Weapon CurrentWeapon() => currentWeapon;
    public Transform GunPoint() => gunPoint;

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
            if (currentWeapon.CanReload())
            {
                player.weaponVisuals.PlayReloadAnimation();
            }
        };
    }
    #endregion
}
