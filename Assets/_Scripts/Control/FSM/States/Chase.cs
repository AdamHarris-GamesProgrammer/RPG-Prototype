using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;

namespace RPG.Control
{
    public class Chase : State
    {
        public Chase(NPCController controller) : base(controller)
        {
            stateID = StateID.Chase;
        }

        public override void Reason(Transform player, Transform npc)
        {
            if(Vector3.Distance(player.position, npc.position) < 5.0f)
            {
                controller.PerformTransition(Transition.PlayerInAttackRange);
            }

            if(Vector3.Distance(player.position, npc.position) > 15.0f)
            {
                controller.PerformTransition(Transition.PlayerLeavesChaseDistance);
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            controller.GetComponent<Mover>().MoveTo(player.position, 1.0f, false);
        }
    }
}

