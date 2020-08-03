using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control 
{
    public class Guard : State
    {
        float dwellDuration;
        float dwellTimer;

        float chaseDistance;

        public Guard(NPCController controller, float time, float chaseDistanceIn) : base(controller)
        {
            dwellDuration = time;
            dwellTimer = 0.0f;

            stateID = StateID.Guard;

            chaseDistance = chaseDistanceIn;
        }

        public override void Reason(Transform player, Transform npc)
        {
            if (Vector3.Distance(npc.position, player.position) < chaseDistance)
            {
                Vector3 rayDirection = player.position - npc.position;

                if (Vector3.Angle(rayDirection, npc.forward) < 45)
                {
                    controller.Aggrevated = true;
                    controller.SetTransition(Transition.PlayerInChaseDistance);
                }
            }

            if (dwellTimer > dwellDuration)
            {
                controller.SetTransition(Transition.WaitTimeOver);
            }


            if (controller.Aggrevated)
            {
                controller.SetTransition(Transition.Aggrevated);
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            dwellTimer += Time.fixedDeltaTime;
        }

        public override void OnEntry()
        {
            dwellTimer = 0.0f;
        }

        public override void OnExit()
        {
            dwellTimer = 0.0f;
        }
    }
}

