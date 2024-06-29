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

    [Header("Shooting specifics")]
    public ShootType shootType;
    public int bulletsPerShot; // ����������� ���� �� �������
    public float defaultFireRate; // ���������������� �� ��������� 
    public float fireRate = 1.0f; // ���� � �������
    private float lastShootTime; // ����� ��������� ������

    [Header("Burst fire")]
    public bool burstAvalible; // ����� �������� 
    public bool burstActive; // ��������� ������
    public int burstBulletsPerShot; // ����������� ���� �� ������� � ������ �������� 
    public float burstFireRate; // ���������������� � ������ ��������
    public float burstFireDelay = 0.1f; // �������� ����� ��������

    [Header("Magazine details")]
    public int bulletsInMagazine; // ���� � ��������
    public int magazineCapacity; // ������� ������ ����� ���� � ��������
    public int totalReserveAmmo; // ����� ����������� �������� ��� ������������� ������ 

    [Header("Spread ")]
    public float baseSpread = 1.0f; // �����������\ ������� �������
    public float currentSpread = 2.0f; // ������� ������� ����
    public float maximumSpread = 3.0f; // ������������ �������

    public float spreadIncreaseRate = 0.15f; // ���������� ��������

    private float lastSpreadUpdateTime; // ����� ���������� ���������� ��������
    private float spreadCooldown = 1.0f; // ����������� ��������

    [Range(1.0f, 2.0f)]
    public float reloadSpeed = 1.0f; // ��� ������ �������� ������������ ������
    [Range(1.0f, 2.0f)]
    public float equipmentSpeed = 1.0f; // ��� ������ �������� ������ ������
    [Range(2.0f, 12.0f)]
    public float gunDistance = 4.0f; // ��������� ������
    [Range(3.0f, 8.0f)]
    public float cameraDistance = 6.0f; // ���������� ������ �� ������ � ����������� �� ���������� ������

    #region Spread methods
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }
    #endregion

    #region Burst methods
    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (burstAvalible == false)
        {
            return;
        }

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    #region Reload methods
    public bool CanReload()
    {
        // ���� ������� ����� �� ������ ��� ������������
        if (bulletsInMagazine == magazineCapacity)
        {
            return false;
        }

        if (totalReserveAmmo > 0)
        {
            return true;
        }

        return false;
    }

    public void RefillBullets()
    {
        //totalReserveAmmo += bulletsInMagazine; 
        // ������� ������� �� �������� � ������ ���������� �������� ������ ��� ������� ������

        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }

    private bool HaveEnoughBullets() => bulletsInMagazine > 0;
    #endregion
}
