using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyFighter : Fighter
    {
        //EDITOR PROPERTIES
        [Header("Heavy Attack Settings")]
        [Tooltip("Chance that the enemy will perform a heavy attack.")]
        [Range(0f, 1f)][SerializeField] private float _chanceForHeavyAttack = 0.2f;
        
        //OVERRIDE METHODS
        
        /// <summary>
        /// Overrides the base attack method to perform the enemies attack behavior
        /// </summary>
        /// <param name="combatTarget"></param>
        public override void Attack(GameObject combatTarget)
        {
            base.Attack(combatTarget);

            if (fighterHealth.isDead || target == null) return;

            //if the target is not in range
            if (IsInRangeOfWeapon(target.position))
            {
                //if the time since last attacked is greater than the cooldown
                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    //then attack
                    AttackBehaviour();


                    timeSinceLastAttack = 0.0f;
                }
            }
        }

        //PRIVATE METHODS 

        /// <summary>
        /// This method generates a value between 0 and 1 to see if a attack is heavy or not.
        /// </summary>
        /// <returns>Returns a bool saying if a attack is heavy</returns>
        private bool GenerateHeavyAttackChance()
        {
            bool result = false;

            if (Random.value < _chanceForHeavyAttack)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Performs the attack behavior for the enemy, mainly setting there animation triggers and looking at a opponent.
        /// </summary>
        private void AttackBehaviour()
        {
            //Checks if the equipped weapon has a heavy attack and if it does then generates a heavy attack chance
            if (equippedWeaponConfig.HasHeavyAttack && GenerateHeavyAttackChance())
            {
                //if the heavy attack went through then set the heavy attack trigger
                fighterAnimator.SetTrigger("heavyAttack");
            }
            else
            {
                //if the attack was not heavy then set the light attack trigger
                fighterAnimator.SetTrigger("lightAttack");
            }

            //Looks at the opponent
            transform.LookAt(target);
        }
    }
}

