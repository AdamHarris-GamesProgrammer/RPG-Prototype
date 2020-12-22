using UnityEngine;
using UnityEngine.Events;

using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using RPG.Control;
using System;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        float maxHealth;
        [SerializeField] private float health;
        [SerializeField] bool useBaseStats = true;
        [SerializeField] TakeDamageEvent takeDamage;


        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> {}

        public event Action OnHealthChanged;
        [SerializeField] public UnityEvent OnDeath;

        public bool isDead;
        [SerializeField] bool canBeDamaged = true;

        private void Awake()
        {
            if (useBaseStats)
            {
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
            maxHealth = health;
            GetComponent<Experience>().OnLevelUp.AddListener(FillHealth);
        }

        void FixedUpdate()
        {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damageIn, bool isHeavyAttack)
        {
            if (!canBeDamaged) return;

            Controller controller = gameObject.GetComponent<Controller>();
            if(controller != null)
            {
                if (controller.IsStrafing) return;

                if (controller.IsBlocking)
                {
                    float reduction = controller.BlockReduction;

                    damageIn *= 1 - reduction;
                    controller.BlockDamage(isHeavyAttack);
                }
            }

            health = Mathf.Max(health -= damageIn, 0f);

            takeDamage.Invoke(damageIn);

            transform.LookAt(instigator.transform);
            

            if (health == 0.0f)
            {
                DeathBehaviour();

                isDead = true;

                if (instigator.TryGetComponent(out Experience xp))
                {
                    xp.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
            }

            OnHealthChanged();
        }

        void DeathBehaviour()
        {
            if (isDead) return;

            OnDeath.Invoke();


            isDead = true;
            GetComponent<Animator>().SetTrigger("death");

            GetComponent<CapsuleCollider>().enabled = false;

            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public float GetHealthPercentage()
        {
            return health / maxHealth;
        }

        public void FillHealth()
        {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health, GetComponent<Experience>().GetLevel());
            health = maxHealth;
            OnHealthChanged();
        }

        public void FillHealth(float amount)
        {
            //Fills the health within the bounds specified
            health = Mathf.Clamp(health += amount, 0f, maxHealth);
            OnHealthChanged();
        }

        //Implements the ISaveable interface
        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            OnHealthChanged();

            if (health <= 0)
            {
                DeathBehaviour();
            }
        }
    }
}

