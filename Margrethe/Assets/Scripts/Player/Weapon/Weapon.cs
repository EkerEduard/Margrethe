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
    public ShootType shootType;

    public int bulletsPerShot { get; private set; } // Колличество пуль за выстрел
    private float defaultFireRate; // Скорострельность по умолчанию 
    public float fireRate = 1.0f; // Пуль в секунду
    private float lastShootTime; // Время последнего выстрела

    [Header("Burst fire")]
    private bool burstAvalible; // Доступен ли режим очереди
    public bool burstActive; // Активен ли режим очереди
    private int burstBulletsPerShot; // Количество пуль за выстрел в режиме очереди
    private float burstFireRate; // Скорострельность в режиме стрельбы
    public float burstFireDelay { get; private set; } // Задержка после выстрела

    [Header("Magazine details")]
    public int bulletsInMagazine; // Количество пуль в магазине
    public int magazineCapacity; // Вместимость магазина
    public int totalReserveAmmo; // Общее количество патронов для данного оружия 

    [Header("Spread ")]
    private float baseSpread = 1.0f; // Стандартный\ базовый разброс
    private float maximumSpread = 3.0f; // Максимальный разброс
    private float currentSpread = 2.0f; // Текущий разброс пуль

    private float spreadIncreaseRate = 0.15f; // Скорость увеличения разброса

    private float lastSpreadUpdateTime; // Время последнего обновления разброса
    private float spreadCooldown = 1.0f; // Перезарядка разброса


    public float reloadSpeed { get; private set; } // Как быстро персонаж перезаряжает оружие
    public float equipmentSpeed { get; private set; } // Как быстро персонаж меняет оружие
    public float gunDistance { get; private set; } // Дальность оружия
    public float cameraDistance { get; private set; } // Расстояние камеры до игрока в зависимости от выбранного оружия

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
    // Метод для применения разброса к направлению стрельбы
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread); // Случайное значение для разброса

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue); // Поворот для применения разброса

        return spreadRotation * originalDirection; // Применить поворот к исходному направлению
    }

    // Метод для обновления разброса
    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            // Если прошло достаточно времени, сбросить разброс к базовому
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread(); // Иначе увеличить разброс
        }

        lastSpreadUpdateTime = Time.time; // Обновить время последнего обновления разброса
    }

    // Метод для увеличения разброса
    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread); // Увеличить разброс и ограничить его максимальным значением
    }
    #endregion

    #region Burst methods
    // Метод для проверки, активирован ли режим очереди
    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            // Для дробовика установить задержку стрельбы в 0
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    // Метод для переключения режима очереди
    public void ToggleBurst()
    {
        if (burstAvalible == false)
        {
            return; // Если режим очереди недоступен, выйти из метода
        }

        burstActive = !burstActive; // Переключить состояние активации режима очереди

        if (burstActive)
        {
            // Если режим очереди активен, изменить параметры стрельбы на режим очереди
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            // Иначе вернуть параметры стрельбы к одиночному режиму
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion

    // Метод для проверки, может ли оружие стрелять
    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    // Метод для проверки готовности к выстрелу
    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time; // Обновить время последнего выстрела
            return true;
        }

        return false;
    }

    #region Reload methods
    // Метод для проверки, можно ли перезарядить оружие
    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
        {
            return false; // Если магазин полон, нельзя перезарядить
        }

        if (totalReserveAmmo > 0)
        {
            return true;  // Если есть патроны в запасе, можно перезарядить
        }

        return false;
    }

    // Метод для перезарядки оружия
    public void RefillBullets()
    {
        //totalReserveAmmo += bulletsInMagazine; 
        // Веренет патроны из магазина к общему количесвту патронов игрока для данного оружия

        int bulletsToReload = magazineCapacity; // Количество пуль для перезарядки

        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo; // Если патронов в запасе меньше, чем нужно для полной перезарядки, перезарядить до оставшегося количества патронов
        }

        totalReserveAmmo -= bulletsToReload; // Уменьшить количество патронов в запасе
        bulletsInMagazine = bulletsToReload; // Заполнить магазин

        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0; // Если патронов в запасе меньше 0, установить 0
        }
    }

    // Метод для проверки, есть ли достаточно патронов для выстрела
    private bool HaveEnoughBullets() => bulletsInMagazine > 0;
    #endregion
}
