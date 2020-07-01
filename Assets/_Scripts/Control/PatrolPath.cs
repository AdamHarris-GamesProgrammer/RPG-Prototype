using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if(i == 0)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                Gizmos.DrawSphere(GetWaypointPosition(i), 0.5f);

                Gizmos.color = Color.green;

                if (i == 0) continue;


                Gizmos.DrawLine(GetWaypointPosition(i - 1), GetWaypointPosition(i));
                if (i == transform.childCount - 1)
                {
                    Gizmos.DrawLine(GetWaypointPosition(i), GetWaypointPosition(0));
                }


            }
        }

        public Vector3 GetWaypointPosition(int i)
        {
            return transform.GetChild(i).position;
        }

        public int CycleWaypoint(int i)
        {
            if(i == transform.childCount)
            {
                return 0;
            }
            return i;
        }
    }
}

