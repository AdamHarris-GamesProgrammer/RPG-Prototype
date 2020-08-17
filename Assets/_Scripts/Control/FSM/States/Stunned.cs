using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class Stunned : State
    {
        float stunDuration;
        float stunTimer;

        public Stunned(PlayerController player, NPCController controller, float stunDurationIn) : base(controller, StateID.Stunned, player)
        {
            stunDuration = stunDurationIn;
        }

        public override void OnEntry()
        {
            stunTimer = 0.0f;
            controller.GetComponent<Mover>().Enabled = false;
            //controller.GetComponent<Mover>().Cancel();
            controller.GetComponent<Animator>().SetTrigger("stun");
            controller.GetComponent<Animator>().SetBool("stunned", true);
            controller.Stunned = true;
            Debug.Log("Stun Enter");
        }

        public override void OnExit()
        {
            controller.GetComponent<Mover>().Enabled = true;
            controller.GetComponent<Animator>().SetBool("stunned", false);
            controller.Stunned = false;
            Debug.Log("Stun Exit");
        }

        public override void Act(Transform player, Transform npc)
        {
            stunTimer += Time.fixedDeltaTime;
        }

        public override void Reason(Transform player, Transform npc)
        {
            if(stunTimer > stunDuration)
            {
                controller.SetTransition(Transition.StunOver);
            }
        }
    }
}

