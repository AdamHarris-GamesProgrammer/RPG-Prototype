using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;

namespace RPG.Control
{
    public class Chase : State
    {
        float chaseDistance, attackDistance;

        public Chase(NPCController controller, float chaseDistanceIn, float attackDistanceIn) : base(controller)
        {
            stateID = StateID.Chase;

            chaseDistance = chaseDistanceIn;
            attackDistance = attackDistanceIn;
        }

        public override void Reason(Transform player, Transform npc)
        {
            float distanceToPlayer = Vector3.Distance(player.position, npc.position);

            if (distanceToPlayer < attackDistance)
            {
                controller.SetTransition(Transition.PlayerInAttackRange);
            }
            else if(distanceToPlayer > chaseDistance)
            {
                controller.SetTransition(Transition.PlayerLeavesChaseDistance);
            }
        }

        public override void OnExit()
        {
            Debug.Log("Chase Exit");
        }

        public override void Act(Transform player, Transform npc)
        {
            controller.GetComponent<Mover>().MoveTo(player.position, 1.0f, false);
        }
    }
}

