using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;

namespace RPG.Control
{
    public class Chase : State
    {
        public Chase()
        {
            stateID = StateID.Chase;
        }


        public override void OnEntry()
        {

        }

        public override void OnExit()
        {

        }

        public override void Reason(Transform player, Transform npc)
        {
            if(Vector3.Distance(player.position, npc.position) < 5.0f)
            {
                npc.GetComponent<NPCController>().PerformTransition(Transition.PlayerInAttackRange);
            }

            if(Vector3.Distance(player.position, npc.position) > 15.0f)
            {
                npc.GetComponent<NPCController>().PerformTransition(Transition.PlayerLeavesChaseDistance);
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            npc.GetComponent<Mover>().MoveTo(player.position, 1.0f, false);
        }
    }
}

