using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player_WeaponVisuals : MonoBehaviour
{
    private Player player;
    private Animator anim;
    private bool isEquipingWeapon;

    [SerializeField] private Weapon_Model[] weaponModels;
    [SerializeField] private Weapon_BackupModel[] weaponBackupModels;

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
        weaponBackupModels = GetComponentsInChildren<Weapon_BackupModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public void PlayReloadAnimation()
    {
        if (isEquipingWeapon)
        {
            return;
        }

        float reloadSpeed = player.weapon.CurrentWeapon().reloadSpeed;

        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }

    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().equipAnimationType;

        float equipmentSpeed = player.weapon.CurrentWeapon().equipmentSpeed;

        leftHandIK.weight = 0f;
        ReduceRigWeight();
        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipType", ((float)equipType));
        anim.SetFloat("EquipSpeed", equipmentSpeed);

        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        isEquipingWeapon = busy;

        anim.SetBool("BusyEquipingWeapon", isEquipingWeapon);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = ((int)CurrentWeaponModel().holdType);

        SwitchOffWeaponModels();
        SwitchOffWeaponBackupModels();

        if (player.weapon.HasOnlyOneWeapon() == false)
        {
            SwitchOnWeaponBackupModels();
        }

        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void SwitchOffWeaponBackupModels()
    {
        foreach (Weapon_BackupModel backupModel in weaponBackupModels)
        {
            backupModel.gameObject.SetActive(false);
        }
    }

    public void SwitchOnWeaponBackupModels()
    {
        WeaponType weaponType = player.weapon.BackupWeapon().weaponType;

        foreach (Weapon_BackupModel backupModel in weaponBackupModels)
        {
            if (backupModel.weaponType == weaponType)
            {
                backupModel.gameObject.SetActive(true);
            }
        }
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
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

    #region Animation rigging methods
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

    public void MaximazeRigWeight() => shouldIncrease_RigWeight = true;
    public void MaximazeLeftHandWeight() => shouldIncrease_LeftHandIKWeight = true;

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().leftTargetPoint;
        Transform hintTransform = CurrentWeaponModel().leftHintPoint;

        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;

        leftHandIK_Hint.localPosition = hintTransform.localPosition;
        leftHandIK_Hint.localRotation = hintTransform.localRotation;
    }
    #endregion
}
