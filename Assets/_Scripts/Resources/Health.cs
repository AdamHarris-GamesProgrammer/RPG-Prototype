using UnityEngine;
using UnityEngine.Events;

using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using RPG.Control;
using System;
using RPG.Inventories;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        float maxHealth;
        [SerializeField] private float mHealth;
        [SerializeField] bool useBaseStats = true;
        [SerializeField] TakeDamageEvent takeDamage;


        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        public event Action OnHealthChanged;
        [SerializeField] public UnityEvent OnDeath;

        public bool isDead;
        [SerializeField] bool canBeDamaged = true;

        private Controller mController;
        private Equipment mEquipment;


        private void Awake()
        {
            mController = GetComponent<Controller>();
            if (mController == null)
            {
                Debug.LogError(gameObject.name + " controller is null.");
            }


            if (useBaseStats)
            {
                mHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
            maxHealth = mHealth;
            GetComponent<Experience>().OnLevelUp.AddListener(FillHealth);

            mEquipment = GetComponent<Equipment>();
        }

        void FixedUpdate()
        {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damageIn, bool isHeavyAttack)
        {
            if (!canBeDamaged) return;

            if (mController.IsStrafing) return;

            //Calculate Blocking Protection
            if (mController.IsBlocking)
            {
                float reduction = mController.BlockReduction;

                damageIn *= 1 - reduction;
                mController.BlockDamage(isHeavyAttack);
            }

            //Calculates Armor Protection
            int armor = 0;
            
            if(mEquipment != null)
            {
                armor = mEquipment.GetTotalArmor();
            }

            float damageToGoThrough = (damageIn / 100) * 10.0f;

            float leftOverDamage = damageIn - damageToGoThrough;

            float armorBlocks = 0.0f;
            if(armor > 0)
            {
                armorBlocks = (leftOverDamage / 100) * armor;
            }

            leftOverDamage = leftOverDamage - armorBlocks + damageToGoThrough;

            //Takes Damage
            mHealth = Mathf.Max(mHealth -= leftOverDamage, 0f);

            takeDamage.Invoke(leftOverDamage);

            transform.LookAt(instigator.transform);

            //Death Check
            if (mHealth == 0.0f)
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
            return mHealth / maxHealth;
        }

        public void FillHealth()
        {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health, GetComponent<Experience>().GetLevel());
            mHealth = maxHealth;
            OnHealthChanged();
        }

        public void FillHealth(float amount)
        {
            //Fills the health within the bounds specified
            mHealth = Mathf.Clamp(mHealth += amount, 0f, maxHealth);
            OnHealthChanged();
        }

        //Implements the ISaveable interface
        public object CaptureState()
        {
            return mHealth;
        }

        public void RestoreState(object state)
        {
            mHealth = (float)state;
            OnHealthChanged();

            if (mHealth <= 0)
            {
                DeathBehaviour();
            }
        }
    }
}

