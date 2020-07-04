using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Transform player;

        float rotX, rotY;

        public float mouseSensitivity = 10f;
        public float rotationSpeed = 5f;


        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
                rotY += Input.GetAxis("Mouse Y") * mouseSensitivity;

            }

            rotY = Mathf.Clamp(rotY, -20f, 5f);

            target.localRotation = Quaternion.Euler(rotY, rotX, 0f);

            //transform.LookAt(target);

            target.position = player.position;
        }
    }
}

