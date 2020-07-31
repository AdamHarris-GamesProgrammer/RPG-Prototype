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

        public Suspicion(NPCController controller, float suspicionDurationIn) : base(controller)
        {
            stateID = StateID.Suspicion;

            suspicionDuration = suspicionDurationIn;
        }

        public override void OnEntry()
        {
            suspicionTimer = 0.0f;

            controller.GetComponent<Mover>().Cancel();
        }

        public override void OnExit()
        {
            suspicionTimer = 0.0f;
        }

        public override void Reason(Transform player, Transform npc)
        {
            if(suspicionTimer > suspicionDuration)
            {
               controller.SetTransition(Transition.SuspicionTimeUp);
            }

            if(Vector3.Distance(player.position, npc.position) < 15.0f)
            {
                controller.SetTransition(Transition.PlayerInChaseDistance);
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            suspicionTimer += Time.fixedDeltaTime;
        }
    }
}

