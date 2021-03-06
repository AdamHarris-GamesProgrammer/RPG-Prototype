﻿using UnityEngine;
using RPG.Resources;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileSpeed = 5.0f;
        [SerializeField] float projectileDamage = 4f;
        [SerializeField] bool isHoming = false;
        [SerializeField] float maxDuration = 15.0f;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 0.2f;

        [SerializeField] UnityEvent OnLaunch;
        [SerializeField] UnityEvent OnHit;

        Health target = null;

        bool isMoving = false;

        GameObject instigator = null;

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

        public void SetTarget(Health inTarget, GameObject instigator, float inDamage)
        {
            isMoving = true;
            target = inTarget;
            transform.LookAt(GetAimLocation());
            damage = inDamage + projectileDamage;

            this.instigator = instigator;

            OnLaunch.Invoke();

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
            if (other.CompareTag("Pickup")) return;
            if (other.CompareTag("IgnoreTrigger")) return;

            if(hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            isMoving = false;
            GetComponent<BoxCollider>().enabled = false;
            
            transform.parent = other.transform;
            if (GetComponentInChildren<TrailRenderer>())
            {
                GetComponentInChildren<TrailRenderer>().enabled = false;
            }

            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy, lifeAfterImpact);
            }

            Health targetHealth = other.gameObject.GetComponent<Health>();

            if (targetHealth == null) return;

            OnHit.Invoke();

            targetHealth.TakeDamage(instigator, damage, false);
           
        }
    }
}

