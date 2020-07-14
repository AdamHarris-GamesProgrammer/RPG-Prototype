using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileSpeed = 5.0f;
        [SerializeField] float projectileDamage = 4f;
        [SerializeField] bool isHoming = false;
        [SerializeField] float maxDuration = 10.0f;
        [SerializeField] float destructionTime = 1.0f;
        [SerializeField] GameObject hitEffect = null;

        [Header("Head and Tail properties")]
        [SerializeField] GameObject projectileHead = null;
        [SerializeField] GameObject projectileTail = null;
        [SerializeField] float headDestructionTime = 0.1f;
        [SerializeField] float tailDestructionTime = 0.2f;

        Health target = null;

        bool isMoving = false;

        float damage = 0;

        void Update()
        {
            if (!isMoving) return;
            if (target == null) return;

            if (isHoming && !target.isDead)
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        public void SetTarget(Health inTarget, float inDamage)
        {
            isMoving = true;
            target = inTarget;
            transform.LookAt(GetAimLocation());
            damage = inDamage + projectileDamage;

            Destroy(gameObject, maxDuration);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null) return target.transform.position;

            return target.transform.position + Vector3.up * targetCapsule.height / 2f;
        }


        private void OnTriggerEnter(Collider other)
        {
            if(hitEffect != null)
            {
                GameObject hitImpact = Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            isMoving = false;
            GetComponent<BoxCollider>().enabled = false;
            
            transform.parent = other.transform;

            if (GetComponentInChildren<TrailRenderer>())
            {
                GetComponentInChildren<TrailRenderer>().enabled = false;
            }

            if (projectileHead != null) Destroy(projectileHead, headDestructionTime);
            if (projectileTail != null) Destroy(projectileTail, tailDestructionTime);

            Health targetHealth = other.gameObject.GetComponent<Health>();

            if (targetHealth == null) return;

            targetHealth.TakeDamage(damage);
           
        }
    }
}

