using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;

namespace RPG.Control
{
    public class Suspicion : State
    {
        float suspicionTimer;
        float suspicionDuration;

        float chaseDistance;

        public Suspicion(PlayerController player, NPCController controller, float suspicionDurationIn, float chaseDistanceIn) : base(controller, StateID.Suspicion, player)
        {
            suspicionDuration = suspicionDurationIn;
            chaseDistance = chaseDistanceIn;
        }

        public override void OnEntry()
        {
            suspicionTimer = 0.0f;

            controller.GetComponent<Mover>().Cancel();

            Debug.Log("Suspicion State");
        }

        public override void OnExit()
        {
            suspicionTimer = 0.0f;
            Debug.Log("Exiting Suspicion State");
            playerController.RemoveAI(controller);
        }

        public override void Reason(Transform player, Transform npc)
        {
            if(suspicionTimer > suspicionDuration)
            {
               controller.Aggrevated = false;
               controller.SetTransition(Transition.SuspicionTimeUp);
            }

            if(Vector3.Distance(player.position, npc.position) < chaseDistance)
            {
                if (InFOV(player, npc))
                {
                    controller.Aggrevated = true;
                    controller.SetTransition(Transition.PlayerInChaseDistance);
                }
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            suspicionTimer += Time.fixedDeltaTime;
        }
    }
}

