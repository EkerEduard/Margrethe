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

[System.Serializable] // Для отображения в инспекторе
public class Weapon
{
    public WeaponType weaponType;

    [Header("Shooting specifics")]
    public ShootType shootType;
    public int bulletsPerShot; // Колличество пуль за выстрел
    public float defaultFireRate; // Скорострельность по умолчанию 
    public float fireRate = 1.0f; // Пуль в секунду
    private float lastShootTime; // Время последней съемки

    [Header("Burst fire")]
    public bool burstAvalible; // Режим стрельбы 
    public bool burstActive; // Активация режима
    public int burstBulletsPerShot; // Колличество пуль на выстрел в режиме стрельбы 
    public float burstFireRate; // Скорострельность в режиме стрельбы
    public float burstFireDelay = 0.1f; // Задержка посде выстрела

    [Header("Magazine details")]
    public int bulletsInMagazine; // Пуль в магазине
    public int magazineCapacity; // Сколько патрон может быть в магазине
    public int totalReserveAmmo; // Общее колличество патронов для определенного оружия 

    [Header("Spread ")]
    public float baseSpread = 1.0f; // Стандартный\ базовый разброс
    public float currentSpread = 2.0f; // Текущий разброс пуль
    public float maximumSpread = 3.0f; // Максимальный разброс

    public float spreadIncreaseRate = 0.15f; // Увелечение разброса

    private float lastSpreadUpdateTime; // Время последнего обновления разброса
    private float spreadCooldown = 1.0f; // Перезарядка разброса

    [Range(1.0f, 2.0f)]
    public float reloadSpeed = 1.0f; // Как быстро персонаж перезаряжает оружие
    [Range(1.0f, 2.0f)]
    public float equipmentSpeed = 1.0f; // Как быстро персонаж меняет оружие
    [Range(2.0f, 12.0f)]
    public float gunDistance = 4.0f; // Дальность оружия
    [Range(3.0f, 8.0f)]
    public float cameraDistance = 6.0f; // Расстояние камеры до игрока в зависимости от выбранного оружия

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
