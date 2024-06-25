using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player_WeaponVisuals : MonoBehaviour
{
    private Player player;
    private Animator anim;
    private bool isGrabbingWeapon;

    [SerializeField] private Weapon_Model[] weaponModels;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncrease_RigWeight;
    private Rig rig;

    [Header("Left Hand IK")]
    [SerializeField] private float leftHandIK_WeaightIncreaseRate;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private Transform leftHandIK_Hint;
    private bool shouldIncrease_LeftHandIKWeight;

    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<Weapon_Model>(true);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public Weapon_Model CurrentWeaponModel()
    {
        Weapon_Model weaponModel = null;

        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }

        return weaponModel;
    }

    public void PlayReloadAnimation()
    {
        if (isGrabbingWeapon)
        {
            return;
        }

        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIK_WeaightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
            {
                shouldIncrease_LeftHandIKWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1)
            {
                shouldIncrease_RigWeight = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }

    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        leftHandIK.weight = 0f;
        ReduceRigWeight();
        anim.SetFloat("WeaponGrabType", ((float)grabType));
        anim.SetTrigger("WeaponGrab");

        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        isGrabbingWeapon = busy;

        anim.SetBool("BusyGrabbingWeapon", isGrabbingWeapon);
    }

    public void MaximazeRigWeight() => shouldIncrease_RigWeight = true;
    public void MaximazeLeftHandWeight() => shouldIncrease_LeftHandIKWeight = true;

    private void SwitchOn()
    {
        SwitchOffWeaponModels();

        CurrentWeaponModel().gameObject.SetActive(true);

        AttachLeftHand();
    }

    private void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().leftTargetPoint;
        Transform hintTransform = CurrentWeaponModel().leftHintPoint;

        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;

        leftHandIK_Hint.localPosition = hintTransform.localPosition;
        leftHandIK_Hint.localRotation = hintTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    //Временная мера
    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn();
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn();
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchOn();
            SwitchAnimationLayer(5);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwitchOn();
            SwitchAnimationLayer(4);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
    }
}
