using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    Tommygun,
    AutoRifle,
    Machinegun,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable] // ��� ����������� � ����������
public class Weapon
{
    public WeaponType weaponType;
    public ShootType shootType;

    public int bulletsPerShot { get; private set; } // ����������� ���� �� �������
    private float defaultFireRate; // ���������������� �� ��������� 
    public float fireRate = 1.0f; // ���� � �������
    private float lastShootTime; // ����� ���������� ��������

    [Header("Burst fire")]
    private bool burstAvalible; // �������� �� ����� �������
    public bool burstActive; // ������� �� ����� �������
    private int burstBulletsPerShot; // ���������� ���� �� ������� � ������ �������
    private float burstFireRate; // ���������������� � ������ ��������
    public float burstFireDelay { get; private set; } // �������� ����� ��������

    [Header("Magazine details")]
    public int bulletsInMagazine; // ���������� ���� � ��������
    public int magazineCapacity; // ����������� ��������
    public int totalReserveAmmo; // ����� ���������� �������� ��� ������� ������ 

    [Header("Spread ")]
    private float baseSpread = 1.0f; // �����������\ ������� �������
    private float maximumSpread = 3.0f; // ������������ �������
    private float currentSpread = 2.0f; // ������� ������� ����

    private float spreadIncreaseRate = 0.15f; // �������� ���������� ��������

    private float lastSpreadUpdateTime; // ����� ���������� ���������� ��������
    private float spreadCooldown = 1.0f; // ����������� ��������


    public float reloadSpeed { get; private set; } // ��� ������ �������� ������������ ������
    public float equipmentSpeed { get; private set; } // ��� ������ �������� ������ ������
    public float gunDistance { get; private set; } // ��������� ������
    public float cameraDistance { get; private set; } // ���������� ������ �� ������ � ����������� �� ���������� ������

    public Weapon(Weapon_Data weaponData)
    {
        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;

        burstAvalible = weaponData.burstAvalible;
        burstActive = weaponData.burstActive;
        burstBulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;

        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        defaultFireRate = fireRate;
    }

    #region Spread methods
    // ����� ��� ���������� �������� � ����������� ��������
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread); // ��������� �������� ��� ��������

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue); // ������� ��� ���������� ��������

        return spreadRotation * originalDirection; // ��������� ������� � ��������� �����������
    }

    // ����� ��� ���������� ��������
    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            // ���� ������ ���������� �������, �������� ������� � ��������
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread(); // ����� ��������� �������
        }

        lastSpreadUpdateTime = Time.time; // �������� ����� ���������� ���������� ��������
    }

    // ����� ��� ���������� ��������
    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread); // ��������� ������� � ���������� ��� ������������ ���������
    }
    #endregion

    #region Burst methods
    // ����� ��� ��������, ����������� �� ����� �������
    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            // ��� ��������� ���������� �������� �������� � 0
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    // ����� ��� ������������ ������ �������
    public void ToggleBurst()
    {
        if (burstAvalible == false)
        {
            return; // ���� ����� ������� ����������, ����� �� ������
        }

        burstActive = !burstActive; // ����������� ��������� ��������� ������ �������

        if (burstActive)
        {
            // ���� ����� ������� �������, �������� ��������� �������� �� ����� �������
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            // ����� ������� ��������� �������� � ���������� ������
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion

    // ����� ��� ��������, ����� �� ������ ��������
    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    // ����� ��� �������� ���������� � ��������
    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time; // �������� ����� ���������� ��������
            return true;
        }

        return false;
    }

    #region Reload methods
    // ����� ��� ��������, ����� �� ������������ ������
    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
        {
            return false; // ���� ������� �����, ������ ������������
        }

        if (totalReserveAmmo > 0)
        {
            return true;  // ���� ���� ������� � ������, ����� ������������
        }

        return false;
    }

    // ����� ��� ����������� ������
    public void RefillBullets()
    {
        //totalReserveAmmo += bulletsInMagazine; 
        // ������� ������� �� �������� � ������ ���������� �������� ������ ��� ������� ������

        int bulletsToReload = magazineCapacity; // ���������� ���� ��� �����������

        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo; // ���� �������� � ������ ������, ��� ����� ��� ������ �����������, ������������ �� ����������� ���������� ��������
        }

        totalReserveAmmo -= bulletsToReload; // ��������� ���������� �������� � ������
        bulletsInMagazine = bulletsToReload; // ��������� �������

        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0; // ���� �������� � ������ ������ 0, ���������� 0
        }
    }

    // ����� ��� ��������, ���� �� ���������� �������� ��� ��������
    private bool HaveEnoughBullets() => bulletsInMagazine > 0;
    #endregion
}
