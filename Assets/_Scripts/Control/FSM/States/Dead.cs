using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Control
{
    public class Dead : State
    {
        bool animPlayed;
        public Dead(PlayerController player, NPCController controller) : base(controller, StateID.Dead, player)
        {
            stateID = StateID.Dead;
        }

        public override void Reason(Transform player, Transform npc)
        {
        }

        public override void Act(Transform player, Transform npc)
        {
            if (!animPlayed)
            {
                controller.GetComponent<Animator>().SetTrigger("death");
                animPlayed = true;
            }
        }

        public override void OnEntry()
        {
            playerController.RemoveAI(controller);
        }
    }
}

