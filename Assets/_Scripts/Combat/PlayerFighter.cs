using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Control;

namespace RPG.Combat
{
    public class PlayerFighter : Fighter
    {
        protected override void Update()
        {
            UpdateTimers();
        }

        protected override void UpdateTimers()
        {
            base.UpdateTimers();
        }

        public override void Attack(GameObject combatTarget)
        {
            target = combatTarget.transform;

            Debug.Log("Target Name: " + target.name);

            if (!IsInRange(target.position)) return;

            transform.LookAt(target);
            fighterAnimator.SetTrigger("attack");

            target.GetComponent<AIController>().Strafe();
        }
    }
}

