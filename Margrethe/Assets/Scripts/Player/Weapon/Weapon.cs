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

[System.Serializable] // Для отображения в инспекторе
public class Weapon
{
    public WeaponType weaponType;

    public int bulletsInMagazine; // Пуль в магазине
    public int magazineCapacity; // Сколько патрон может быть в магазине
    public int totalReserveAmmo; // Общее колличество патронов для определенного оружия 

    [Range(1.0f, 2.0f)]
    public float reloadSpeed = 1.0f; // Как быстро персонаж перезаряжает оружие
    [Range(1.0f, 2.0f)]
    public float equipmentSpeed = 1.0f; // Как быстро персонаж меняет оружие

    [Space]
    public float fireRate = 1.0f; // Пуль в секунду
    private float lastShootTime; // Время последней съемки

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
        // Если магазин полон не сможем его перезарядить
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
        // Веренет патроны из магазина к общему количесвту патронов игрока для данного оружия

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
