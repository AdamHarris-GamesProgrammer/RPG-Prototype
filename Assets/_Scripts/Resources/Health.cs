using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        float maxHealth;
        [SerializeField] private float health;

        public bool isDead;

        private void Awake()
        {
            health = GetComponent<BaseStats>().GetStat(Stat.Health);
            maxHealth = health;
            GetComponent<Experience>().onLevelUp += FillHealth;
        }

        public void TakeDamage(GameObject instigator, float damageIn)
        {
            health -= damageIn;

            health = Mathf.Clamp(health, 0.0f, maxHealth);

            GetComponent<HealthBar>().UpdateBar();

            //print("Health: " + health);

            if (health <= 0.0f)
            {
                DeathBehaviour();
                instigator.GetComponent<Experience>().GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
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

        public float GetHealthPercentage()
        {
            return health / maxHealth;
        }

        public object CaptureState()
        {
            return health;
        }

        public void FillHealth()
        {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health, GetComponent<Experience>().GetLevel());
            health = maxHealth;
            GetComponent<HealthBar>().UpdateBar();
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

