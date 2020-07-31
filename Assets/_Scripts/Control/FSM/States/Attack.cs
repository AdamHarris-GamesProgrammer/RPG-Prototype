using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Combat;

namespace RPG.Control
{
    public class Attack : State
    {
        public Attack(NPCController controller) : base(controller)
        {
            stateID = StateID.Attack;
        }

        public override void Reason(Transform player, Transform npc)
        {
            controller.PerformTransition(Transition.PlayerLeavesAttackRange);
        }

        public override void Act(Transform player, Transform npc)
        {
            Fighter npcFighter = controller.GetComponent<Fighter>();

            if (npcFighter.IsInRangeOfWeapon(player.position))
            {
                npcFighter.Attack(player.gameObject);
            }
        }
    }
}

