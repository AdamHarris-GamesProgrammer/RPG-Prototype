using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Combat;

namespace RPG.Control
{
    public class Attack : State
    {
        public Attack()
        {
            stateID = StateID.Attack;
        }


        public override void OnEntry()
        {

        }

        public override void OnExit()
        {

        }

        public override void Reason(Transform player, Transform npc)
        {
            npc.GetComponent<NPCController>().PerformTransition(Transition.PlayerLeavesAttackRange);
        }

        public override void Act(Transform player, Transform npc)
        {
            Fighter npcFighter = npc.GetComponent<Fighter>();

            if (npcFighter.IsInRangeOfWeapon(player.position))
            {
                npcFighter.Attack(player.gameObject);
            }
        }
    }
}

