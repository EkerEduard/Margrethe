using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_VisualController : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private Transform[] gunTransforms;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform tommygun;
    [SerializeField] private Transform machinegun;

    private Transform currentGun; // Текущее оружие

    [Header("Left Hand IK")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform leftHint;

    private void Start()
    {
        SwitchOn(pistol);

        anim = GetComponentInParent<Animator>();
    }

    //Временная мера
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
            SwitchAnimationLayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(revolver);
            SwitchAnimationLayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRifle);
            SwitchAnimationLayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotgun);
            SwitchAnimationLayer(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(rifle);
            SwitchAnimationLayer(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchOn(tommygun);
            SwitchAnimationLayer(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwitchOn(machinegun);
            SwitchAnimationLayer(4);
        }
    }

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;

        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHand_TargetTransform>().transform;
        Transform hintTransform = currentGun.GetComponentInChildren<LeftHand_HintTransform>().transform;

        leftHand.localPosition = targetTransform.localPosition;
        leftHand.localRotation = targetTransform.localRotation;

        leftHint.localPosition = hintTransform.localPosition;
        leftHint.localRotation = hintTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }
}
