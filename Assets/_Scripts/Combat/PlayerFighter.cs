using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Control;

namespace RPG.Combat
{
    public class PlayerFighter : Fighter
    {
        bool isAttacking = false;

        protected override void UpdateTimers()
        {
            base.UpdateTimers();

            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                isAttacking = false;
            }

        }

        public override void Attack(GameObject combatTarget, bool isHeavyAttack)
        {
            if (isAttacking) return;
            timeSinceLastAttack = 0.0f;

            target = combatTarget.transform;

            if (!IsInRangeOfWeapon(target.position)) return;

            transform.LookAt(target);

            if (heavyAttack)
            {
                fighterAnimator.SetTrigger("heavyAttack");
            }
            else
            {
                fighterAnimator.SetTrigger("lightAttack");
            }
        }
    }
}

