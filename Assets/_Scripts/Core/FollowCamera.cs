using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        private Transform target;

        private void Awake()
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}

