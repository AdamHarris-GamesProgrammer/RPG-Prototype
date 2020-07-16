using UnityEngine;
using RPG.Resources;

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
        [SerializeField] GameObject[] destroyOnHit;
        [SerializeField] float lifeAfterImpact = 0.2f;
        
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

            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy, lifeAfterImpact);
            }

            Health targetHealth = other.gameObject.GetComponent<Health>();

            if (targetHealth == null) return;

            targetHealth.TakeDamage(instigator, damage);
           
        }
    }
}

