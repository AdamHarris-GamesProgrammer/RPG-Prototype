using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;

namespace RPG.Control
{
    public class Chase : State
    {
        float chaseDistance, attackDistance;

        public Chase(PlayerController player, NPCController controller, float chaseDistanceIn, float attackDistanceIn) : base(controller, StateID.Chase, player)
        {
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
                controller.AggrevatedTimer += Time.fixedDeltaTime;

                if(controller.AggrevatedTimer > controller.AggrevationDuration)
                {
                    controller.Aggrevated = false;
                    controller.SetTransition(Transition.Deaggrevated);
                }
            }
        }

        public override void OnExit()
        {
            controller.AggrevatedTimer = 0.0f;
        }

        public override void OnEntry()
        {
            controller.AggrevatedTimer = 0.0f;
            playerController.aggrevatedEnemies.Add(controller);
        }

        public override void Act(Transform player, Transform npc)
        {
            controller.GetComponent<Mover>().MoveTo(player.position, 1.0f, false);
        }
    }
}

