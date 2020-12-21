using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [RequireComponent(typeof(Pickup))]
    public class KeyboardPickup : MonoBehaviour, IInteractive
    {
        Pickup pickup;

        [SerializeField] GameObject pickupPrompt;

        public void Interact()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (pickup.CanBePickedUp())
                {
                    pickup.PickupItem();
                }
            }
        }

        public void ShowUI(bool isActive)
        {
            pickupPrompt.SetActive(isActive);
        }

        void Awake()
        {
            pickup = GetComponent<Pickup>();
        }
    }
}

