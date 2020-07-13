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
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null) return target.transform.position;

            return target.transform.position + Vector3.up * targetCapsule.height / 2f;
        }


        private void OnTriggerEnter(Collider other)
        {
            isMoving = false;
            GetComponent<BoxCollider>().enabled = false;
            
            transform.parent = other.transform;

            GetComponentInChildren<TrailRenderer>().enabled = false;

            Destroy(this.gameObject, 1f);

            print("Trigger enter");
            Health targetHealth = other.gameObject.GetComponent<Health>();

            if (targetHealth == null) return;

            print("Target Health found");

            targetHealth.TakeDamage(damage);

           
        }
    }
}

