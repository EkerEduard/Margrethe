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

[System.Serializable] // ��� ����������� � ����������
public class Weapon
{
    public WeaponType weaponType;

    public int bulletsInMagazine; // ���� � ��������
    public int magazineCapacity; // ������� ������ ����� ���� � ��������
    public int totalReserveAmmo; // ����� ����������� �������� ��� ������������� ������ 

    [Range(1.0f, 2.0f)]
    public float reloadSpeed = 1.0f; // ��� ������ �������� ������������ ������
    [Range(1.0f, 2.0f)]
    public float equipmentSpeed = 1.0f; // ��� ������ �������� ������ ������

    [Space]
    public float fireRate = 1.0f; // ���� � �������
    private float lastShootTime; // ����� ��������� ������

    public bool CanShoot()
    {
        if (HaveEnoughBullets() && ReadyToFire())
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
    }

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
