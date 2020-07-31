using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Combat;

namespace RPG.Control
{
    public class Attack : State
    {
        float attackDistance;

        public Attack(NPCController controller, float attackDistanceIn) : base(controller)
        {
            stateID = StateID.Attack;

            attackDistance = attackDistanceIn;
        }

        public override void Reason(Transform player, Transform npc)
        {
            if(Vector3.Distance(player.position, npc.position) > attackDistance)
            {
                controller.SetTransition(Transition.PlayerLeavesAttackRange);
            }
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

