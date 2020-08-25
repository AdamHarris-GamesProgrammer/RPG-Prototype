using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryExample.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] GameObject uiContainer = null;

        [Header("Gameplay Related Settings")]
        [SerializeField] bool pauseTime = false;
        [SerializeField] bool enablesMouse = false;

        // Start is called before the first frame update
        void Start()
        {
            uiContainer.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                uiContainer.SetActive(!uiContainer.activeSelf);

                bool active = uiContainer.activeSelf;

                if (pauseTime)
                {
                    if (active)
                    {
                        AudioListener.pause = true;
                        Time.timeScale = 0.0f;
                    }
                    else
                    {
                        AudioListener.pause = false;
                        Time.timeScale = 1.0f;
                    }
                }

                if (enablesMouse)
                {
                    if (active)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
        }
    }
}