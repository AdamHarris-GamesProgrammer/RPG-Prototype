using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100.0f;
        private float health;

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
    }
}

