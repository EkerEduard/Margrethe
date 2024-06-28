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

    public bool CanShoot()
    {
        return HaveEnoughBullets();
    }

    private bool HaveEnoughBullets()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
    }

    public bool CanReload()
    {
        // ���� ������� ����� �� ������ ��� ������������
        if(bulletsInMagazine == magazineCapacity)
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
}
