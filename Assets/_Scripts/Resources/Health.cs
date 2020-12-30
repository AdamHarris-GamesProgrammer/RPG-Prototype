using UnityEngine;
using UnityEngine.Events;

using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using RPG.Control;
using System;
using RPG.Inventories;
using TMPro;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        float _maxHealth;


        [SerializeField] private float _health;
        [SerializeField] bool useBaseStats = true;
        [SerializeField] TakeDamageEvent takeDamage;

        [SerializeField] TMP_Text _healthText;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        public event Action OnHealthChanged;
        [SerializeField] public UnityEvent OnDeath;

        public bool isDead;
        [SerializeField] bool canBeDamaged = true;

        private Controller _controller;
        private Equipment mEquipment;

        private BaseStats mBaseStats;


        private void Awake()
        {
            _controller = GetComponent<Controller>();
            if (_controller == null)
            {
                Debug.LogError(gameObject.name + " controller is null.");
            }

            mBaseStats = GetComponent<BaseStats>();

            if (useBaseStats)
            {
                _health = mBaseStats.GetStat(Stat.Health, mBaseStats.GetLevel());
            }
            _maxHealth = _health;
            GetComponent<Experience>().OnLevelUp.AddListener(FillHealth);

            mEquipment = GetComponent<Equipment>();
        }

        void FixedUpdate()
        {
            _maxHealth = mBaseStats.GetStat(Stat.Health, mBaseStats.GetLevel());

            if (_healthText)
            {
                _healthText.text = _maxHealth.ToString();
            }
        }

        public void TakeDamage(GameObject instigator, float damageIn, bool isHeavyAttack)
        {
            if (!canBeDamaged) return;

            if (_controller.IsStrafing) return;

            //Calculate Blocking Protection
            if (_controller.IsBlocking)
            {
                float reduction = _controller.BlockReduction;

                damageIn *= 1 - reduction;
                _controller.BlockDamage(isHeavyAttack);
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
            _health = Mathf.Max(_health -= leftOverDamage, 0f);

            takeDamage.Invoke(leftOverDamage);

            transform.LookAt(instigator.transform);

            //Death Check
            if (_health == 0.0f)
            {
                DeathBehaviour();

                isDead = true;

                if (instigator.TryGetComponent(out Experience xp))
                {
                    xp.GainExperience(mBaseStats.GetStat(Stat.ExperienceReward));
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
            return _health / _maxHealth;
        }

        public void FillHealth()
        {
            _maxHealth = mBaseStats.GetStat(Stat.Health, GetComponent<Experience>().GetLevel());
            _health = _maxHealth;
            OnHealthChanged();
        }

        public void FillHealth(float amount)
        {
            //Fills the health within the bounds specified
            _health = Mathf.Clamp(_health += amount, 0f, _maxHealth);
            OnHealthChanged();
        }

        //Implements the ISaveable interface
        public object CaptureState()
        {
            return _health;
        }

        public void RestoreState(object state)
        {
            _health = (float)state;
            OnHealthChanged();

            if (_health <= 0)
            {
                DeathBehaviour();
            }
        }
    }
}