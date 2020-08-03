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
        float speedFraction;
        float waypointTolerance;

        bool hasPatrolPoint = true;

        Vector3 position;

        public Patrol(NPCController controller, PatrolPath inPath, float chaseDistanceIn, float speedFractionIn, float waypointToleranceIn) : base(controller)
        {
            waypoints = inPath;


            stateID = StateID.Patrol;

            chaseDistance = chaseDistanceIn;
            speedFraction = speedFractionIn;
            waypointTolerance = waypointToleranceIn;
        }

        public Patrol(NPCController controller, bool hasPatrol, float chaseDistanceIn, Vector3 posIn) : base(controller)
        {
            chaseDistance = chaseDistanceIn;
            hasPatrolPoint = hasPatrol;

            position = posIn;
        }


        public override void Reason(Transform player, Transform npc)
        {
            if (Vector3.Distance(npc.position, player.position) < chaseDistance)
            {
                if (InFOV(player, npc))
                {
                    controller.Aggrevated = true;
                    controller.SetTransition(Transition.PlayerInChaseDistance);
                }
            }

            if (controller.Aggrevated)
            {
                controller.SetTransition(Transition.Aggrevated);
            }

            if (!hasPatrolPoint) return;
            if (AtWaypoint(npc))
            {
                npc.GetComponent<Mover>().Cancel();

                controller.SetTransition(Transition.AtWaypoint);
            }
        }

        public override void Act(Transform player, Transform npc)
        {
            if (hasPatrolPoint)
            {
                if (AtWaypoint(npc))
                {
                    currentWaypoint++;
                    currentWaypoint = waypoints.CycleWaypoint(currentWaypoint);

                    //Set next position to current Waypoint
                    Vector3 nextPosition = waypoints.GetWaypointPosition(currentWaypoint);

                    controller.GetComponent<Mover>().StartMoveAction(nextPosition, speedFraction, false);
                }
            }
            else
            {
                controller.GetComponent<Mover>().StartMoveAction(position, 1.0f, false);
            }

        }

        private bool AtWaypoint(Transform npc)
        {
            float distanceToWaypoint = Vector3.Distance(npc.transform.position, waypoints.GetWaypointPosition(currentWaypoint));

            if (distanceToWaypoint <= waypointTolerance)
            {
                return true;
            }

            return false;
        }
    }
}

