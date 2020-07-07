using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using Unity.Collections;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField] [ReadOnly] private float health;

        public bool isDead;

        private void Start()
        {
            health = maxHealth;
        }

        public void TakeDamage(float damageIn)
        {
            health -= damageIn;

            health = Mathf.Clamp(health, 0.0f, maxHealth);

            print("Health: " + health);

            if (health <= 0.0f)
            {
                DeathBehaviour();
            }
        }

        void DeathBehaviour()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("death");

            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void Revive()
        {
            isDead = false;

            GetComponent<Animator>().SetTrigger("revive");

        }

        public object CaptureState()
        {
            Debug.Log("Health amount at saving: " + health);
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;

            if(health <= 0)
            {
                DeathBehaviour();
            }
            else
            {
                Revive();
            }

            Debug.Log("Health amount at loading: " + health);
        }
    }
}

