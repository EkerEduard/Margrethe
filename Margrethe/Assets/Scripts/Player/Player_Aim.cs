using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Aim : MonoBehaviour
{
    private Player player;
    private PlayerControlls controlls;

    [Header("Aim control")]
    [SerializeField] private Transform aim;

    [Header("Camera control")]
    [SerializeField] private Transform cameraTarget;
    [Range(0.5f, 1.0f)]
    [SerializeField] private float minCameraDistance; // Минимальная дистанция камеры
    [Range(1.0f, 3.0f)]
    [SerializeField] private float maxCameraDistance; // Максимальная дистанция камеры
    [Range(3.0f, 5.0f)]
    [SerializeField] private float cameraSensetivity; // Чувствительность к прицеливанию

    [Space]
    [SerializeField] private LayerMask aimLayerMask;

    private Vector2 aimInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        aim.position = GetMouseHitInfo().point;
        aim.position = new Vector3(aim.position.x, transform.position.y + 1.15f, aim.position.z);

        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private Vector3 DesieredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1.15f; // Прицел не уходит вверх или вниз

        return desiredCameraPosition;
    }

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }

    private void AssignInputEvents()
    {
        controlls = player.controlls;

        controlls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controlls.Character.Aim.canceled += controls => aimInput = Vector2.zero;
    }
}
