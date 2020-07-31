using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;

namespace RPG.Control
{
    public class Patrol : State
    {
        int currentWaypoint;

        float chaseDistance;

        public Patrol(NPCController controller, PatrolPath inPath, float chaseDistanceIn) : base(controller)
        {
            waypoints = inPath;
            stateID = StateID.Patrol;

            chaseDistance = chaseDistanceIn;
        }


        public override void Reason(Transform player, Transform npc)
        {
            //TODO: Link up chase distances etc
            if (Vector3.Distance(npc.position, player.position) < chaseDistance)
            {
                controller.SetTransition(Transition.PlayerInChaseDistance);
            }

            if (AtWaypoint(npc))
            {
                npc.GetComponent<Mover>().Cancel();

                controller.SetTransition(Transition.AtWaypoint);
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            //Set next position to current Waypoint
            Vector3 nextPosition = waypoints.GetWaypointPosition(currentWaypoint);


            if (AtWaypoint(npc))
            {
                currentWaypoint++;
                currentWaypoint = waypoints.CycleWaypoint(currentWaypoint);
            }

            //Set next position to current Waypoint
            nextPosition = waypoints.GetWaypointPosition(currentWaypoint);

            //TODO: Link patrolling speed fraction
            controller.GetComponent<Mover>().StartMoveAction(nextPosition, 1.0f, false);
        }

        private bool AtWaypoint(Transform npc)
        {
            float distanceToWaypoint = Vector3.Distance(npc.transform.position, waypoints.GetWaypointPosition(currentWaypoint));

            if (distanceToWaypoint <= 1.0f)
            {
                return true;
            }

            return false;
        }
    }
}

