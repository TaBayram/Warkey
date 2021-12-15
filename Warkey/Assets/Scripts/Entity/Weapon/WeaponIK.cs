using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIK : MonoBehaviour
{
    public const int iterations = 5;
    public Transform targetTransform;
    public Transform aimTransform;
    public float weight = 1.0f;

    public HumanBone[] humanBones;
    Transform[] boneTransforms;

    public float angleLimit = 90f;
    public float distanceLimit = 1.5f;

    public void BindTransforms(Transform targetTransform, Transform aimTransform) {
        this.targetTransform = targetTransform;
        this.aimTransform = aimTransform;
    }


    private void Start() {
        Animator animator = GetComponentInChildren<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < boneTransforms.Length; i++) {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
    }

    void LateUpdate()
    {
        if (targetTransform == null || aimTransform == null) return;

        Vector3 targetPosition = targetTransform.position;
        for(int i = 0; i < iterations; i++) {
            for (int y = 0; y < boneTransforms.Length; y++) {
                Transform bone = boneTransforms[y];
                float boneWeight = humanBones[y].weight * weight;
                AimAtTarget(bone, targetPosition);
            }
        }
        
    }

    private Vector3 GetTargetPosition() {
        Vector3 targetDirection = targetTransform.position - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0f;

        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if(targetAngle > angleLimit) {
            blendOut += (targetAngle - angleLimit) / 50f;
        }

        float targetDistance = targetDirection.magnitude;
        if(targetDistance < distanceLimit) {
            blendOut += distanceLimit - targetDistance;
        }

        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }


    private void AimAtTarget(Transform bone, Vector3 targetPosition) {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }
}

[System.Serializable]
public struct HumanBone
{
    public HumanBodyBones bone;
    public float weight;
}