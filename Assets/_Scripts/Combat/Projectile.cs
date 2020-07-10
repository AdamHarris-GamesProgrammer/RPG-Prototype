using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileSpeed = 5.0f;
        [SerializeField] private Transform target = null;
        void Update()
        {
            if (target == null) return;

            transform.LookAt(GetAimLocation());

            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null) return target.position;

            return target.position + Vector3.up * targetCapsule.height / 2f;
        }
    }
}

