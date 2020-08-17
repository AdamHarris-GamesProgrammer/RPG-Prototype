using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Combat;

namespace RPG.Control
{
    public class Attack : State
    {
        float attackDistance;

        bool rangedEnemy;

        Fighter npcFighter;

        public Attack(PlayerController player, NPCController controller, float attackDistanceIn, bool rangedEnemyIn = false) : base(controller, StateID.Attack, player)
        {
            attackDistance = attackDistanceIn;
            rangedEnemy = rangedEnemyIn;
            npcFighter = controller.GetComponent<Fighter>();
        }

        public override void Reason(Transform player, Transform npc)
        {
            if (rangedEnemy)
            {
                if (!npcFighter.IsInRangeOfWeapon(player.position))
                {
                    controller.SetTransition(Transition.PlayerLeavesAttackRange);
                }
            }
            else
            {
                if (Vector3.Distance(player.position, npc.position) > attackDistance)
                {
                    controller.SetTransition(Transition.PlayerLeavesAttackRange);
                }
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            if (npcFighter.IsInRangeOfWeapon(player.position))
            {
                npcFighter.Attack(player.gameObject);
            }
        }

        public override void OnEntry()
        {
            playerController.aggrevatedEnemies.Add(controller);
        }
    }
}

