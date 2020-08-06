using UnityEngine;

namespace RPG.UI
{
    public class LookAtMainCamera : MonoBehaviour
    {
        Transform cameraTransform;
        void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        void FixedUpdate()
        {
            //transform.LookAt(Camera.main.transform);
            transform.forward = cameraTransform.forward;
        }
    }
}

