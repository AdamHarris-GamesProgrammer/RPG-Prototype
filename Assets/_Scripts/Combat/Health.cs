using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100.0f;
        private float health;

        private void Start()
        {
            health = maxHealth;
        }

        public void TakeDamage(float damageIn)
        {
            health -= damageIn;

            health = Mathf.Clamp(health, 0.0f, maxHealth);

            print("Health: " + health);

            if(health <= 0.0f)
            {
                print(transform.gameObject.name + " is dead");
            }
        }
    }
}

