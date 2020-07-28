using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Control;

namespace RPG.Combat
{
    public class PlayerFighter : Fighter
    {
        bool isAttacking = false;


        protected override void Update()
        {
            UpdateTimers();
        }

        protected override void UpdateTimers()
        {
            base.UpdateTimers();

            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                isAttacking = false;
            }

        }

        public override void Attack(GameObject combatTarget)
        {
            if (isAttacking) return;
            timeSinceLastAttack = 0.0f;


            target = combatTarget.transform;

            Debug.Log("Target Name: " + target.name);

            if (!IsInRange(target.position)) return;

            transform.LookAt(target);
            fighterAnimator.SetTrigger("attack");

            target.GetComponent<AIController>().Strafe();
        }
    }
}

