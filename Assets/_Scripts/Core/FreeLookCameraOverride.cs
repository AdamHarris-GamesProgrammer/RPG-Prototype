using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RPG.Core
{
    public class FreeLookCameraOverride : MonoBehaviour
    {
        CinemachineFreeLook freeLookCam;

        float xValue;

        void Awake()
        {
            freeLookCam = GetComponent<CinemachineFreeLook>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                //Get the mouse position on the screen
                float mouseXPos = Input.mousePosition.x;

                //if mouse position is on the right of the screen
                if (mouseXPos > Screen.width / 2)
                {
                    xValue = 1f; //rotate right
                }
                else
                {
                    xValue = -1f; //rotate left
                }
            }
            else
            {
                xValue = 0;
            }

            //Rotate camera on x value
            freeLookCam.m_XAxis.Value = xValue;
        }
    }
}

