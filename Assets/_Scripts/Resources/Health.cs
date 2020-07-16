using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float maxHealth = 100.0f;
        [SerializeField] private float health;
        public float healthPoints { get { return health; }  set { health = value; } }
        public float totalHealthPoints {  get { return maxHealth; } }

        public bool isDead;


        private void Awake()
        {
            health = GetComponent<BaseStats>().GetHealth();
        }

        public void TakeDamage(float damageIn)
        {
            health -= damageIn;

            health = Mathf.Clamp(health, 0.0f, maxHealth);

            //print("Health: " + health);

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

            GetComponent<CapsuleCollider>().enabled = false;

            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            Debug.Log(gameObject.name + " health is: " + (float)state);
            health = (float)state;

            if(health <= 0)
            {
                DeathBehaviour();
            }
        }
    }
}

