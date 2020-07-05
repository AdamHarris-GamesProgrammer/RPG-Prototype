using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookCameraOverride : MonoBehaviour
{
    CinemachineFreeLook freeLookCam;

    float xValue;

    void Awake()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            xValue += Input.GetAxis("Mouse X");
            //rotY += Input.GetAxis("Mouse Y") * mouseSensitivity;

        }
        else
        {
            xValue = 0;
        }

        xValue = Mathf.Clamp(xValue, -1f,1f);

        Debug.Log("X Value: " + xValue);
        freeLookCam.m_XAxis.Value = -xValue;
    }
}
