using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20.0f; // �������� �� ���������, �� ������� ��������� ������� ��� �����.

    [SerializeField] private Weapon_Data defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady; // ���������� ������
    private bool isShooting;

    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefab; // ������ ����
    [SerializeField] private float bulletSpeed; // �������� ����

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

    private void Update()
    {
        if (isShooting)
        {
            Shoot();
        }
    }

    #region Slots management - Pickup\ Equip\ Drop\ Ready Weapon
    private void EquipStartingWeapon()
    {
        weaponSlots[0] = new Weapon(defaultWeaponData);

        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count)
        {
            return;
        }

        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
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

    public void SetWeaponReady(bool ready) => weaponReady = ready; // ���������� ������ � ����������� ��������
    public bool WeaponReady() => weaponReady; // �������� ����������
    #endregion

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShot)
            {
                SetWeaponReady(true);
            }
        }
    }

    private void Shoot()
    {
        // �������� ���������� ������
        if (WeaponReady() == false)
        {
            return;
        }

        // �������� ������ �� ������ ��������
        if (currentWeapon.CanShoot() == false)
        {
            return;
        }

        // ������������� ��������
        player.weaponVisuals.PlayFireAnimation();

        // �������� ���� ��������
        if (currentWeapon.shootType == ShootType.Single)
        {
            isShooting = false;
        }

        // ��������� ����� ����� �������� �����������
        if (currentWeapon.BurstActivated() == true)
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
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

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
            {
                return weapon;
            }
        }

        return null;
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControlls controlls = player.controlls;

        controlls.Character.Fire.performed += context => isShooting = true;
        controlls.Character.Fire.canceled += context => isShooting = false;

        controlls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controlls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controlls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controlls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controlls.Character.EquipSlot5.performed += context => EquipWeapon(4);
        controlls.Character.EquipSlot6.performed += context => EquipWeapon(5);
        controlls.Character.EquipSlot7.performed += context => EquipWeapon(6);

        controlls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controlls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controlls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurst();
    }
    #endregion
}
