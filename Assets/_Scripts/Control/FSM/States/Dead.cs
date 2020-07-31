using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Control
{
    public class Dead : State
    {
        bool animPlayed;
        public Dead()
        {
            stateID = StateID.Dead;
        }


        public override void OnEntry()
        {
        }

        public override void OnExit()
        {
        }

        public override void Reason(Transform player, Transform npc)
        {
        }

        public override void Act(Transform player, Transform npc)
        {
            if (!animPlayed)
            {
                npc.GetComponent<Animator>().SetTrigger("death");
                animPlayed = true;
            }
        }
    }
}

