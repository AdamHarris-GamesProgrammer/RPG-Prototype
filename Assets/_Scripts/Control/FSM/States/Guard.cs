﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control 
{
    public class Guard : State
    {
        float dwellDuration;
        float dwellTimer;

        public Guard(float time)
        {
            dwellDuration = time;
            dwellTimer = 0.0f;

            stateID = StateID.Guard;
        }

        public override void Reason(Transform player, Transform npc)
        {
            if (Vector3.Distance(npc.position, player.position) < 15.0f)
            {
                npc.GetComponent<NPCController>().SetTransition(Transition.PlayerInChaseDistance);
            }

            if (dwellTimer > dwellDuration)
            {
                dwellTimer = 0.0f;

                npc.GetComponent<NPCController>().SetTransition(Transition.WaitTimeOver);
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

