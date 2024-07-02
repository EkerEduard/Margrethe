using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Aim : MonoBehaviour
{
    private Player player;
    private PlayerControlls controlls;

    [Header("Aim visual - laser")]
    [SerializeField] private LineRenderer aimLaser; // Этот компонент находится на weapon holder(дочерний компонент player)

    [Header("Aim control")]
    [SerializeField] private Transform aim; // Точка прицеливания

    [SerializeField] private bool isAimingPrecisly; // Точное прицеливание
    [SerializeField] private bool isLockingToTarget; // Захват цели

    [Header("Camera control")]
    [SerializeField] private Transform cameraTarget; // Цель камеры
    [Range(0.5f, 1.0f)]
    [SerializeField] private float minCameraDistance; // Минимальная дистанция камеры
    [Range(1.0f, 3.0f)]
    [SerializeField] private float maxCameraDistance; // Максимальная дистанция камеры
    [Range(3.0f, 5.0f)]
    [SerializeField] private float cameraSensetivity; // Чувствительность камеры при прицеливании

    [Space]
    [SerializeField] private LayerMask aimLayerMask; // Слой, по которому происходит прицеливание

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit; // Последний известный хит мыши (информация о пересечении луча с объектом)

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        // Переключение точного прицеливания
        if (Input.GetKeyDown(KeyCode.P))
        {
            isAimingPrecisly = !isAimingPrecisly;
        }

        // Переключение захвата цели
        if (Input.GetKeyDown(KeyCode.L))
        {
            isLockingToTarget = !isLockingToTarget;
        }

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        aimLaser.enabled = player.weapon.WeaponReady(); // Включаем лазер, если оружие готово к стрельбе

        if (aimLaser.enabled == false)
        {
            return;
        }

        Weapon_Model weaponModel = player.weaponVisuals.CurrentWeaponModel(); // Получаем текущую модель оружия

        weaponModel.transform.LookAt(aim); // Оружие смотрит на прицел
        weaponModel.gunPoint.LookAt(aim); // Точка выстрела смотрит на прицел

        Transform gunPoint = player.weapon.GunPoint(); // Получаем точку выстрела
        Vector3 laserDirection = player.weapon.BulletDirection(); // Получаем направление пули

        float laserTipLength = 0.5f; // Длина наконечника лазера
        float gunDistance = player.weapon.CurrentWeapon().gunDistance; // Дальность оружия

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance; // Конечная точка лазера

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point; // Если луч пересекает объект, конечная точка лазера становится точкой пересечения
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position); // Начальная точка лазера
        aimLaser.SetPosition(1, endPoint); // Конечная точка лазера
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength); // Добавляем наконечник лазера
    }

    private void UpdateAimPosition()
    {
        Transform target = Target(); // Получаем цель

        if (target != null && isLockingToTarget)
        {
            // Если у цели есть компонент Renderer, используем его центр в качестве точки прицеливания
            if (target.GetComponent<Renderer>() != null)
            {
                aim.position = target.GetComponent<Renderer>().bounds.center;
            }
            else
            {
                aim.position = target.position;
            }

            return;
        }

        aim.position = GetMouseHitInfo().point; // Обновляем позицию прицела на основе информации о хите мыши

        if (isAimingPrecisly == false)
        {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1.15f, aim.position.z); // Ставим прицел на уровне игрока
        }
    }

    public Transform Target()
    {
        Transform target = null;

        // Если пересечение луча с объектом, который имеет компонент Target, то устанавливаем его как цель
        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }

    public Transform Aim() => aim; // Возвращаем текущую позицию прицеливания
    public bool CanAimPrecisly() => isAimingPrecisly; // Проверяем, включено ли точное прицеливание

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput); // Создаем луч от камеры через позицию мыши

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo; // Обновляем последнюю известную информацию о хите мыши
            return hitInfo;
        }

        return lastKnownMouseHit; // Возвращаем последнюю известную информацию о хите мыши
    }

    #region Camera

    private void UpdateCameraPosition()
    {
        // Обновляем позицию камеры, плавно перемещая её к желаемой позиции
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private Vector3 DesieredCameraPosition()
    {
        // Определяем максимальную дистанцию камеры в зависимости от движения игрока
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point; // Желанная позиция камеры основывается на позиции хита мыши
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized; // Направление прицеливания
         
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition); // Расстояние до желанной позиции
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance); // Ограничиваем расстояние

        desiredCameraPosition = transform.position + aimDirection * clampedDistance; // Устанавливаем желанную позицию камеры
        desiredCameraPosition.y = transform.position.y + 1.15f; // Устанавливаем высоту камеры

        return desiredCameraPosition;
    }

    #endregion

    private void AssignInputEvents()
    {
        controlls = player.controlls;

        controlls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controlls.Character.Aim.canceled += controls => mouseInput = Vector2.zero;
    }
}
