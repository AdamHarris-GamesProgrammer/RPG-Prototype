using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class PlayerFighter : Fighter
    {
        //// Start is called before the first frame update
        //void Start()
        //{
        //    base.Start();
        //}

        // Update is called once per frame
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
        }
    }
}

