using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Camera distance")]
    [SerializeField] private bool canChangeCameraDistance; // ћожно ли мен€ть дистанцию камеры
    [SerializeField] private float distanceChangeRate; // —корость изменени€ дистанции камеры
    private float targetCameraDistance; // ÷елева€ дистанци€ камеры

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("You had more than one Camera Manager");
            Destroy(gameObject);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false)
        {
            return;
        }

        // ѕолучить текущую дистанцию камеры
        float currentDistance = transposer.m_CameraDistance;

        // ≈сли текуща€ дистанци€ почти равна целевой, выйти из метода
        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.01f)
        {
            return;
        }

        // ѕлавно изменить дистанцию камеры к целевой дистанции с заданной скоростью
        transposer.m_CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
}
