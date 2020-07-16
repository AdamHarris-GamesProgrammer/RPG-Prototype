using UnityEngine;

namespace RPG.UI
{
    public class LookAtMainCamera : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}

