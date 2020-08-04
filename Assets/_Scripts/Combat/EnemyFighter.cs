using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyFighter : Fighter
    {
        [Header("Heavy Attack Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceForHeavyAttack = 0.2f;
        
        public override void Attack(GameObject combatTarget)
        {
            base.Attack(combatTarget);

            if (fighterHealth.isDead || target == null) return;

            //checks if the target is in range
            bool inRange = IsInRangeOfWeapon(target.position);

            //if the target is not in range
            if (inRange)
            {
                //if the time since last attacked is greater than the cooldown
                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    //then attack
                    GenerateHeavyAttackChance();
                    AttackBehaviour();


                    timeSinceLastAttack = 0.0f;
                }
            }
        }

        private void GenerateHeavyAttackChance()
        {
            if (UnityEngine.Random.value < chanceForHeavyAttack)
            {
                heavyAttack = true;
                //Debug.Log("Enemy Heavy Attack");
            }
            else
            {
                heavyAttack = false;
                //Debug.Log("Enemy Light Attack");
            }
        }

        private void AttackBehaviour()
        {
            if (heavyAttack && equippedWeapon.HasHeavyAttack)
            {
                fighterAnimator.SetTrigger("heavyAttack");
            }
            else
            {
                fighterAnimator.SetTrigger("lightAttack");
            }
            transform.LookAt(target);
        }
    }
}

