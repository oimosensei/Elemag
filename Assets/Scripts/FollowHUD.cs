using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace raspberly.ovr
{
    public class FollowHUD : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float followMoveSpeed = 0.1f;
        [SerializeField] private float followRotateSpeed = 0.02f;
        [SerializeField] private float rotateSpeedThreshold = 0.9f;
        [SerializeField] private bool isImmediateMove = false;
        [SerializeField] private bool isLockX = false;
        [SerializeField] private bool isLockY = false;
        [SerializeField] private bool isLockZ = false;
        private Quaternion rot;
        private Quaternion rotDif;

        private void Start()
        {
            if (!target) target = Camera.main.transform;
            target = Player.instance.hmdTransform;
        }

        private void LateUpdate()
        {
            if (isImmediateMove) transform.position = target.position;
            else transform.position = Vector3.Lerp(transform.position, target.position, followMoveSpeed);

            rotDif = target.rotation * Quaternion.Inverse(transform.rotation);
            rot = target.rotation;
            if (isLockX) rot.x = 0;
            if (isLockY) rot.y = 0;
            if (isLockZ) rot.z = 0;
            if (rotDif.w < rotateSpeedThreshold) transform.rotation = Quaternion.Lerp(transform.rotation, rot, followRotateSpeed * 4);
            else transform.rotation = Quaternion.Lerp(transform.rotation, rot, followRotateSpeed);
        }

        //強制的に同期させたい時
        public void ImmediateSync(Transform targetTransform)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }
}