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

[System.Serializable] // ƒл€ отображени€ в инспекторе
public class Weapon
{
    public WeaponType weaponType;

    public int bulletsInMagazine; // ѕуль в магазине
    public int magazineCapacity; // —колько патрон может быть в магазине
    public int totalReserveAmmo; // ќбщее колличество патронов дл€ определенного оружи€ 

    [Range(1.0f, 2.0f)]
    public float reloadSpeed = 1.0f; //  ак быстро персонаж перезар€жает оружие
    [Range(1.0f, 2.0f)]
    public float equipmentSpeed = 1.0f; //  ак быстро персонаж мен€ет оружие

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
        // ≈сли магазин полон не сможем его перезар€дить
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
        // ¬еренет патроны из магазина к общему количесвту патронов игрока дл€ данного оружи€

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
